using System.Net;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace Gpm.CacheStorage
{
    using Common;
    using Common.Util;

    public enum CacheRequestType
    { 
        ALWAYS,
        FIRSTPLAY,
        ONCE,
        LOCAL,
    }

    [Serializable]
    public class CachePackage
    {
        public const string PACKAGE_NAME = "CacheStoragePackage";

        [SerializeField]
        public List<CacheInfo> cacheStorage = new List<CacheInfo>();

        [SerializeField]
        internal int lastIndex = 0;

        [SerializeField]
        internal long cachedSize = 0;

        [SerializeField]
        internal long removeCacheSize = 0;

        [SerializeField]
        private List<int> spaceIdx = new List<int>();

        [SerializeField]
        public List<CacheInfo> removeCache = new List<CacheInfo>();

        private List<CacheInfo> requestCache = new List<CacheInfo>();

        private bool dirty = false;

        public DateTime lastRemoveTime;
        
        private string cachePath;


        public void OnAfterDeserialize()
        {
            foreach (var info in cacheStorage)
            {
                info.storage = this;
            }

            foreach (var info in removeCache)
            {
                info.storage = this;
            }
        }

        internal CacheInfo GetCacheInfo(string url)
        {
            foreach (CacheInfo cachInfo in cacheStorage)
            {
                if (cachInfo.url.Equals(url) == true)
                {
                    return cachInfo;           
                }
            }

            return null;
        }

        internal CacheInfo GetRequestCache(string url)
        {
            foreach (var rq in requestCache)
            {
                if (rq.url.Equals(url) == true)
                {
                    return rq;
                }
            }

            return null;
        }

        public CacheInfo RequestLocal(string url, Action<GpmCacheResult> onResult)
        {
            CacheInfo info = GetCacheInfo(url);

            if (onResult != null)
            {
                byte[] datas = null;
                if (info != null &&
                    IsValidCacheData(info) == true)
                {
                    datas = GetCacheData(info);
                }
                onResult(new GpmCacheResult(info, datas, false));
            }

            if (info != null)
            {
                info.lastAccess = DateTime.UtcNow.Ticks;
                GpmCacheStorage.UpdatePackage();
            }   

            return info;
        }

        public CacheInfo Request(string url, CacheRequestType requestType, double reRequestTime, Action<GpmCacheResult> onResult)
        {
            CacheInfo info = GetRequestCache(url);
            if(info != null)
            {
                if (onResult != null)
                {
                    info.callback += onResult;
                }

                info.lastAccess = DateTime.UtcNow.Ticks;
                GpmCacheStorage.UpdatePackage();

                return info;
            }

            bool useCache = true;
            info = GetCacheInfo(url);
            if (info == null)
            {
                info = new CacheInfo(this, url);
                useCache = false;
            }

            if (onResult != null)
            {
                info.callback += onResult;
            }
            info.lastAccess = DateTime.UtcNow.Ticks;
            GpmCacheStorage.UpdatePackage();

            System.Action<byte[], bool> OnData = (datas, updateData) =>
            {
                info.SendResult(datas, updateData);
            };

            if (requestType == CacheRequestType.LOCAL ||
                Application.internetReachability == NetworkReachability.NotReachable)
            {
                byte[] datas = null;
                if (IsValidCacheData(info) == true)
                {
                    datas = GetCacheData(info);
                }

                OnData(datas, false);
                return info;
            }

            if (useCache == true &&
                requestType != CacheRequestType.ALWAYS &&
                info.NeedRequest() == false)
            {
                bool useLocalCache = false;
                if (requestType == CacheRequestType.FIRSTPLAY)
                {
                    if (info.requestInPlay == true)
                    {
                        useLocalCache = true;
                    }
                }
                else if (requestType == CacheRequestType.ONCE)
                {
                    useLocalCache = true;
                }

                if (reRequestTime == 0)
                {
                    reRequestTime = GpmCacheStorage.GetReRequestTime();
                }
                if (info.CheckReRequest(reRequestTime) == true)
                {
                    useLocalCache = true;
                }

                if (useLocalCache == true)
                {
                    if (IsValidCacheData(info) == true)
                    {
                        byte[] datas = GetCacheData(info);
                        if (datas != null)
                        {
                            OnData(datas, false);
                            return info;
                        }
                    }

                    useCache = false;
                }
            }

            info.state = CacheInfo.State.REQUEST;
            requestCache.Add(info);

            GpmWebRequest request = new GpmWebRequest();
            if (useCache == true)
            {
                if (string.IsNullOrEmpty(info.eTag) == false)
                {
                    request.SetRequestHeader("If-None-Match", info.eTag);
                }
                if (string.IsNullOrEmpty(info.lastModified) == false)
                {
                    request.SetRequestHeader("If-Modified-Since", info.lastModified.GetValue().ToUniversalTime().ToString("r"));
                }
            }

            info.requestTime = DateTime.UtcNow;
            info.requestInPlay = true;
            request.Get(url, (requestResult) =>
            {
                info.state = CacheInfo.State.NONE;
                requestCache.Remove(info);

                if (requestResult.isSuccess == true)
                {
                    if (requestResult.responseCode == (long)HttpStatusCode.NotModified)
                    {
                        byte[] cachedDatas = null;
                        if (IsValidCacheData(info) == true)
                        {
                            cachedDatas = GetCacheData(info);
                        }
                        
                        if (cachedDatas != null)
                        {
                            info.state = CacheInfo.State.CACHED;
                            
                            Dictionary<string, string> cacheControlElements = CacheControl.GetElements(requestResult.request.GetResponseHeader(HttpElement.CACHE_CONTROL));

                            CacheControl cacheControl = null;
                            bool noStore = false;
                            if (cacheControlElements.Count > 0)
                            {
                                noStore = cacheControlElements.ContainsKey(HttpElement.NO_STORE) == true;
                                cacheControl = new CacheControl(cacheControlElements);

                                info.cacheControl = cacheControl;
                            }

                            string expires = requestResult.request.GetResponseHeader(HttpElement.EXPIRES);
                            if (string.IsNullOrEmpty(expires) == false)
                            {
                                info.expires = expires;
                                info.expires = info.expires.GetValue().ToUniversalTime();
                            }

                            string age = requestResult.request.GetResponseHeader(HttpElement.AGE);
                            if (string.IsNullOrEmpty(age) == false)
                            {
                                info.age = age;
                            }

                            string date = requestResult.request.GetResponseHeader(HttpElement.DATE);
                            if (string.IsNullOrEmpty(date) == false)
                            { 
                                info.date = date;
                                info.date = info.date.GetValue().ToUniversalTime();
                            }

                            info.responseTime = DateTime.UtcNow;
                            info.CaculateCacheInfo();

                            OnData(cachedDatas, false);
                        }
                        else
                        {
                            info.eTag = string.Empty;
                            Request(url, CacheRequestType.ALWAYS, 0, null);
                        }
                    }
                    else if (requestResult.responseCode == (long)HttpStatusCode.OK)
                    {
                        info.responseTime = DateTime.UtcNow;

                        CacheControl cacheControl = null;
                        Dictionary<string, string> cacheControlElements = CacheControl.GetElements(requestResult.request.GetResponseHeader(HttpElement.CACHE_CONTROL));

                        bool noStore = false;
                        if (cacheControlElements.Count > 0)
                        {
                            noStore = cacheControlElements.ContainsKey(HttpElement.NO_STORE) == true;
                            cacheControl = new CacheControl(cacheControlElements);
                        }

                        bool updateData = true;
                        byte[] datas = requestResult.request.downloadHandler.data;

                        if (noStore == true)
                        {
                            if (info.IsCached() == true)
                            {
                                RemoveCacheData(info);
                            }
                        }
                        else
                        {
                            if (info.IsCached() == true)
                            {
                                if(info.contentLength == datas.Length)
                                {
                                    byte[] cachedDatas = null;
                                    if (IsValidCacheData(info) == true)
                                    {
                                        cachedDatas = GetCacheData(info);
                                    }

                                    if (ByteArraysEqual(cachedDatas, datas) == true)
                                    {
                                        updateData = false;
                                    }
                                }
                                if (updateData == true)
                                {
                                    RemoveCacheData(info);
                                }
                            }

                            info.cacheControl = cacheControl;

                            info.eTag = requestResult.request.GetResponseHeader(HttpElement.ETAG);
                            info.lastModified = requestResult.request.GetResponseHeader(HttpElement.LAST_MODIFIED);

                            info.expires = requestResult.request.GetResponseHeader(HttpElement.EXPIRES);
                            info.expires = info.expires.GetValue().ToUniversalTime();

                            info.age = requestResult.request.GetResponseHeader(HttpElement.AGE);
                            info.date = requestResult.request.GetResponseHeader(HttpElement.DATE);
                            info.date = info.date.GetValue().ToUniversalTime();

                            info.CaculateCacheInfo();

                            info.contentLength = requestResult.request.GetResponseHeader(HttpElement.CONENT_LENGTH);
                            if (datas != null)
                            {
                                info.contentLength = datas.Length;
                                if (updateData == true)
                                {
                                    AddCacheData(info, datas);

                                }
                                else
                                {
                                    GpmCacheStorage.UpdatePackage();
                                }
                            }
                        }
                        
                        OnData(datas, updateData);
                    }
                    else
                    {
                        OnData(null, false);
                    }
                }
                else
                {
                    OnData(null, false);
                }
            });
            return info;
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2)
            {
                return true;
            }
            if (b1 == null || b2 == null)
            {
                return false;
            }
            if (b1.Length != b2.Length)
            {
                return false;
            }
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsCached(CacheInfo info)
        {
            return info.index > 0;
        }

        internal string GetCacheDataPath(CacheInfo info)
        {
            if (IsCached(info) == true)
            {
                return Path.Combine(GetCachePath(), info.index.ToString());
            }

            return "";
        }

        public void SaveCacheData(CacheInfo info, byte[] data)
        {
            if (Directory.Exists(GetCachePath()) == false)
            {
                Directory.CreateDirectory(GetCachePath());
            }
            string filePath = GetCacheDataPath(info);

            File.WriteAllBytes(filePath, data);
        }

        public bool IsValidCacheData(CacheInfo info)
        {
            string filePath = GetCacheDataPath(info);

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == true &&
                fileInfo.Length == info.contentLength)
            {
                return true;
            }

            return false;
        }

        public byte[] GetCacheData(CacheInfo info)
        {
            string filePath = GetCacheDataPath(info);

            return File.ReadAllBytes(filePath);
        }

        public string GetCacheData(CacheInfo info, System.Text.Encoding encoding = null)
        {
            byte[] data = GetCacheData(info);

            if (encoding == null)
            {
                encoding = System.Text.Encoding.Default;
            }

            return encoding.GetString(data);
        }

        public void AddCacheData(CacheInfo info, byte[] datas)
        {
            long maxCount = GpmCacheStorage.GetMaxCount();
            if (maxCount > 0)
            {
                SecuringStorageCount(1);
            }

            long maxSize = GpmCacheStorage.GetMaxSize();
            if(maxSize > 0)
            {
                SecuringStorage(maxSize, datas.LongLength);
            }

            if (spaceIdx.Count > 0)
            {
                info.index = spaceIdx[0];
                spaceIdx.RemoveAt(0);
            }
            else
            {
                info.index = ++lastIndex;
            }

            cachedSize += info.contentLength;
            info.state = CacheInfo.State.CACHED;

            SaveCacheData(info, datas);

            cacheStorage.Add(info);

            GpmCacheStorage.UpdatePackage();
        }

        public void CacheSort()
        {
            cacheStorage.Sort();
        }

        public bool IsDirty()
        {
            return dirty;
        }

        public void SetDirty(bool dirty = true)
        {
            this.dirty = dirty;
        }

        public void SecuringStorageLastAccess(double unusedTime)
        {
            List<CacheInfo> newExpired = new List<CacheInfo>();
            for (int i = 0; i < cacheStorage.Count; i++)
            {
                if (cacheStorage[i].IsLastAccessPeriod(unusedTime) == true)
                {
                    newExpired.Add(cacheStorage[i]);
                }
            } 

            for (int i = 0; i < newExpired.Count; i++)
            {
                if (RemoveCacheData(newExpired[i]) == false)
                {
                    break;
                }
            }
        }

        public void SecuringStorageCount(int addCount = 0)
        {
            long maxCount = GpmCacheStorage.GetMaxCount();
            if (maxCount <= 0)
            {
                return;
            }

            if (cacheStorage.Count + addCount > maxCount)
            {
                CacheSort();
            }

            while (cacheStorage.Count + addCount > maxCount)
            {
                if (RemoveCacheData(cacheStorage.Last<CacheInfo>(), true) == false)
                {
                    break;
                }
            }
        }


        public void SecuringStorage(long maxSize, long addSize = 0)
        {
            if(maxSize == 0)
            {
                return;
            }

            if (cachedSize + removeCacheSize + addSize > maxSize)
            {
                if (removeCache.Count > 0)
                {
                    while (removeCache.Count > 0 &&
                            cachedSize + removeCacheSize + addSize > maxSize)
                    {
                        DeleteRemoveCache();
                    }

                    if(removeCache.Count == 0)
                    {
                        removeCacheSize = 0;
                    }
                }

                if (cacheStorage.Count > 0)
                {
                    CacheSort();

                    while (cacheStorage.Count > 0 &&
                            cachedSize + addSize > maxSize)
                    {
                        if (RemoveCacheData(cacheStorage.Last<CacheInfo>(), true) == false)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public bool RemoveCacheData(CacheInfo info, bool immediately = false)
        {
            if (info.IsCached() == false)
            {
                return true;
            }

            if (cacheStorage.Remove(info) == true)
            {
                if (immediately == true)
                {
                    DeleteCacheData(info);
                }
                else
                {
                    CacheInfo removeInfo = new CacheInfo(info);
                    removeInfo.state = CacheInfo.State.REMOVE;
                    removeCache.Add(removeInfo);

                    removeCacheSize += removeInfo.contentLength;
                }

                cachedSize -= info.contentLength;

                GpmCacheStorage.UpdatePackage();

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DeleteCacheData(CacheInfo info)
        {
            try
            {
                if (info.IsCached() == true)
                {
                    string filePath = GetCacheDataPath(info);
                    File.Delete(filePath);
                    spaceIdx.Add(info.index);
                    spaceIdx.Sort();

                    while (spaceIdx.Count > 0 &&
                            spaceIdx[spaceIdx.Count - 1] >= lastIndex)
                    {
                        spaceIdx.RemoveAt(spaceIdx.Count - 1);
                        lastIndex--;
                    }

                    GpmCacheStorage.UpdatePackage();
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            return false;
        }


        public void Remove()
        {
            Directory.Delete(GetCachePath());

            lastIndex = 0;
            cachedSize = 0;
            cacheStorage.Clear();
            spaceIdx.Clear();
        }

        public void RemoveAll()
        {
            foreach (CacheInfo info in cacheStorage)
            {
                try
                {
                    string filePath = GetCacheDataPath(info);
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            lastIndex = 0;
            cachedSize = 0;
            cacheStorage.Clear();
            spaceIdx.Clear();

            GpmCacheStorage.UpdatePackage();
        }

        public void Update()
        {
            if (CanRemove() == true)
            {
                DeleteRemoveCache();
            }
        }

        public CacheInfo DeleteRemoveCache()
        {
            int removeIndex = 0;
            for (int idx = 0; idx < removeCache.Count; idx++)
            {
                if (removeCache[removeIndex].index < removeCache[idx].index)
                {
                    removeIndex = idx;
                }
            }

            CacheInfo removeInfo = null;

            if(removeIndex < removeCache.Count)
            {
                removeInfo = removeCache[removeIndex];

                DeleteCacheData(removeInfo);
                removeCache.RemoveAt(removeIndex);
                removeCacheSize -= removeInfo.contentLength;

                lastRemoveTime = DateTime.UtcNow;

                GpmCacheStorage.UpdatePackage();
            }

            return removeInfo;
        }

        public bool CanRemove()
        {   
            if (removeCache.Count > 0)
            {
                double removePeriodTime = GpmCacheStorage.GetRemoveCycle();
                if (removePeriodTime > 0)
                {
                    if ((DateTime.UtcNow - lastRemoveTime).TotalSeconds > removePeriodTime)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public string GetCachePath()
        {
            if (string.IsNullOrEmpty(cachePath) == true)
            {
                cachePath = Path.Combine(Application.persistentDataPath, GpmCacheStorage.NAME);
#if UNITY_IOS
                UnityEngine.iOS.Device.SetNoBackupFlag(cachePath);
#endif
            }

            return cachePath;
        }

        private static string PackagePath()
        {
            return Path.Combine(GpmCacheStorage.GetCachePath(), PACKAGE_NAME);
        }        

        public static CachePackage Load()
        {
            CachePackage cachePackage = null;

            string path = PackagePath();
            if (File.Exists(path) == true)
            {
                try
                {
                    cachePackage = GpmJsonMapper.ToObject<CachePackage>(File.ReadAllText(path));

                    cachePackage.OnAfterDeserialize();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }

            return cachePackage;
        }

        public void Save()
        {
            if(Directory.Exists(GetCachePath()) == false)
            {
                Directory.CreateDirectory(GetCachePath());
            }
            
            File.WriteAllText(PackagePath(), GpmJsonMapper.ToJson(this));
        }
    }
}
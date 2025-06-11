using System;
using System.Collections;
using Gpm.Common;
using Gpm.Common.Util;

namespace Gpm.CacheStorage
{
    public static class GpmCacheStorage
    {
        public const string NAME = "GpmCacheStorage";
        public const string VERSION = "1.1.2";

        private static CacheStorageConfig cacheConfig;
        private static CachePackage cachePackage = new CachePackage();

        public static long updateTime = 0;

        public static CacheStorageConfig Config
        {
            get
            {
                if (cacheConfig == null)
                {
                    LoadConfig();
                }

                return cacheConfig;
            }
        }

        public static CachePackage Package
        {
            get
            {
                if (cachePackage == null)
                {
                    LoadPackage();
                }

                return cachePackage;
            }
        }


        public static event Action onChangeCache;

        public static int GetCacheCount()
        {
            return Package.cacheStorage.Count;
        }

        public static long GetCacheSize()
        {
            return Package.cachedSize;
        }

        public static void ClearCache()
        {
            Package.RemoveAll();
        }

        public static void SetMaxCount(int count = 0, bool applyStorage = true)
        {
            Config.SetMaxCount(count);

            if (applyStorage == true &&
                count > 0)
            {
                Package.SecuringStorageCount();
            }
        }

        public static int GetMaxCount()
        {
            return Config.GetMaxCount();
        }

        public static void SetMaxSize(long size = 0, bool applyStorage = true)
        {
            Config.SetMaxSize(size);

            if (applyStorage == true &&
                size > 0)
            {
                Package.SecuringStorage(size);
            }
        }

        public static long GetMaxSize()
        {
            return Config.GetMaxSize();
        }

        public static void SetReRequestTime(double value)
        {
            Config.SetReRequestTime(value);
        }

        public static double GetReRequestTime()
        {
            return Config.GetReRequestTime();
        }

        public static void SetUnusedPeriodTime(double value)
        {
            Config.SetUnusedPeriodTime(value);
        }

        public static double GetUnusedPeriodTime()
        {
            return Config.GetUnusedPeriodTime();
        }

        public static void SetRemoveCycle(double value)
        {
            Config.SetRemoveCycle(value);
        }

        public static double GetRemoveCycle()
        {
            return Config.GetRemoveCycle();
        }

        public static long GetRemoveCacheSize()
        {
            return Package.removeCacheSize;
        }

        public static CacheRequestType GetCacheRequestType()
        {
            return Config.GetCacheRequestType();
        }

        public static void SetCacheRequestType(CacheRequestType value)
        {
            Config.SetCacheRequestType(value);
        }

        static GpmCacheStorage()
        {
            initialize();
        }

        public static string GetCachePath()
        {
            return Package.GetCachePath();
        }

        public static CacheInfo Request(string url, Action<GpmCacheResult> onResult)
        {
            return Request(url, Config.GetCacheRequestType(), onResult);
        }

        public static CacheInfo Request(string url, CacheRequestType requestType, Action<GpmCacheResult> onResult)
        {
            return Request(url, requestType, 0, onResult);
        }

        public static CacheInfo Request(string url, double reRequestTime, Action<GpmCacheResult> onResult)
        {
            return Request(url, Config.GetCacheRequestType(), reRequestTime, onResult);
        }

        public static CacheInfo Request(string url, CacheRequestType requestType, double reRequestTime, Action<GpmCacheResult> onResult)
        {
            return Package.Request(url, requestType, reRequestTime, onResult);
        }

        public static CacheInfo RequestHttpCache(string url, Action<GpmCacheResult> onResult)
        {
            return Request(url, CacheRequestType.ALWAYS, onResult);
        }

        public static CacheInfo RequestLocalCache(string url, Action<GpmCacheResult> onResult)
        {
            return Package.RequestLocal(url, onResult);
        }

        public static CacheInfo GetCachedTexture(string url, Action<CachedTexture> onResult)
        {
            CacheInfo info = Package.GetCacheInfo(url);
            if (info != null)
            {
                CachedTexture cachedTexture = CachedTextureManager.Get(info);
                if (cachedTexture != null)
                {
                    onResult(cachedTexture);
                    return info;
                }
            }

            return RequestLocalCache(url, (result) =>
            {
                if (result.IsSuccess() == true)
                {
                    onResult(CachedTextureManager.Cache(result.Info, false, false, result.Data));
                }
                else
                {
                    onResult(null);
                }
            });
        }

        public static CacheInfo RequestTexture(string url, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, Config.GetCacheRequestType(), 0, false, onResult);
        }

        public static CacheInfo RequestTexture(string url, bool preLoad, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, Config.GetCacheRequestType(), 0, preLoad, onResult);
        }

        public static CacheInfo RequestTexture(string url, double reRequestTime, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, Config.GetCacheRequestType(), reRequestTime, false, onResult);
        }

        public static CacheInfo RequestTexture(string url, double reRequestTime, bool preLoad, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, Config.GetCacheRequestType(), reRequestTime, preLoad, onResult);
        }

        public static CacheInfo RequestTexture(string url, CacheRequestType requestType, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, requestType, 0, false, onResult);
        }

        public static CacheInfo RequestTexture(string url, CacheRequestType requestType, bool preLoad, Action<CachedTexture> onResult)
        {
            return RequestTexture(url, requestType, 0, preLoad, onResult);
        }

        public static CacheInfo RequestTexture(string url, CacheRequestType requestType, double reRequestTime, bool preLoad, Action<CachedTexture> onResult)
        {
            CacheInfo info = Package.GetCacheInfo(url);
            if (info != null)
            {
                if (info.NeedRequest(reRequestTime) == false)
                {
                    CachedTexture cachedTexture = CachedTextureManager.Get(info);
                    if (cachedTexture != null)
                    {
                        if (cachedTexture.requested == true)
                        {
                            onResult(cachedTexture);

                            info.lastAccess = DateTime.UtcNow.Ticks;
                            GpmCacheStorage.UpdatePackage();

                            return info;
                        }
                    }
                }
                else if (preLoad == true)
                {
                    CachedTexture cachedTexture = CachedTextureManager.Get(info);
                    if (cachedTexture != null)
                    {
                        onResult(cachedTexture);

                        info.lastAccess = DateTime.UtcNow.Ticks;
                        GpmCacheStorage.UpdatePackage();
                    }
                    else
                    {
                        if (Package.IsValidCacheData(info) == true)
                        {
                            byte[] datas = Package.GetCacheData(info);

                            onResult(CachedTextureManager.Cache(info, false, false, datas));

                            info.lastAccess = DateTime.UtcNow.Ticks;
                            GpmCacheStorage.UpdatePackage();
                        }   
                    }
                }
            }

            bool subRequest = false;
            if (Package.GetRequestCache(url) != null)
            {
                subRequest = true;
            }

            info = Request(url, requestType, (result) =>
            {
                if (result.IsSuccess() == true)
                {
                    CachedTexture resultTexture = null;
                    if (subRequest == true)
                    {
                        resultTexture = CachedTextureManager.Get(info);
                    }

                    if (resultTexture == null)
                    {
                        resultTexture = CachedTextureManager.Cache(result.Info, true, result.UpdateData, result.Data);
                    }

                    onResult(resultTexture);
                }
                else
                {
                    onResult(null);
                }
            });

            return info;
        }

        internal static void initialize()
        {
            LoadAll();
            
            ManagedCoroutine.Start(UpdateRoutine());
        }

        internal static CacheStorageConfig LoadConfig()
        {
            cacheConfig = CacheStorageConfig.Load();
            if (cacheConfig == null)
            {
                cacheConfig = new CacheStorageConfig();
            }
            return cacheConfig;
        }

        internal static CachePackage LoadPackage()
        {
            cachePackage = CachePackage.Load();
            if (cachePackage == null)
            {
                cachePackage = new CachePackage();
            }

            return cachePackage;
        }

        public static void LoadAll()
        {
            GpmJsonMapper.RegisterExporter<StringToValue<int>>((sv, w) => w.Write(sv.GetText()));
            GpmJsonMapper.RegisterImporter<string, StringToValue<int>>(value => new StringToValue<int>(value));
            GpmJsonMapper.RegisterImporter<int, StringToValue<int>>(value => value > 0 ? new StringToValue<int>(value) : null);

            GpmJsonMapper.RegisterExporter<StringToValue<DateTime>>((sv, w) => w.Write(sv.GetValue().Ticks));
            GpmJsonMapper.RegisterImporter<int, StringToValue<DateTime>>(value => value > 0 ? new StringToValue<DateTime>(new DateTime(value)) : null);
            GpmJsonMapper.RegisterImporter<long, StringToValue<DateTime>>(value => value > 0 ? new StringToValue<DateTime>(new DateTime(value)) : null);
            GpmJsonMapper.RegisterImporter<string, StringToValue<DateTime>>(value => new StringToValue<DateTime>(value));

            LoadConfig();
            LoadPackage();

            updateTime = DateTime.UtcNow.Ticks;
        }

        internal static void UpdatePackage(bool immediately = false)
        {
            if (immediately == true)
            {
                SavePackage();
            }
            else
            {
                if (Package.IsDirty() == false)
                {
                    Package.SetDirty(true);
                }
            }
        }

        private static IEnumerator UpdateRoutine()
        {
            while (cachePackage != null)
            {
                updateTime = DateTime.UtcNow.Ticks;

                Package.Update();

                AutoDeleteUnusedCache();

                if (Package.IsDirty() == true)
                {
                    SavePackage();

                    Package.SetDirty(false);
                }

                yield return null;
            }
            
        }

        internal static void SavePackage()
        {
            Package.Save();

            if (onChangeCache != null)
            {
                onChangeCache();
            }
        }

        internal static void AutoDeleteUnusedCache()
        {
            if (Config.GetUnusedPeriodTime() > 0 &&
                Config.GetRemoveCycle() > 0)
            {
                Package.SecuringStorageLastAccess(Config.GetUnusedPeriodTime());
            }
        }
    }
}
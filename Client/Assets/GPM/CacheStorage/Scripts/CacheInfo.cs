using UnityEngine;
using System;

namespace Gpm.CacheStorage
{
    [Serializable]
    public class CacheInfo : IComparable<CacheInfo>
    {
        private const long SECONDS_PER_DAY = 86400;
        public enum State
        {
            NONE = 0,
            REQUEST,
            CACHED,
            EXPIRED,
            REMOVE,
        }

        [NonSerialized]
        internal CachePackage storage;

        [SerializeField]
        public string url;

        [SerializeField]
        public string eTag;

        [SerializeField]
        public StringToValue<DateTime> lastModified;

        [SerializeField]
        public long lastAccess;

        [SerializeField]
        public StringToValue<DateTime> expires;

        [SerializeField]
        public StringToValue<DateTime> requestTime;

        [SerializeField]
        public StringToValue<DateTime> responseTime;

        [SerializeField]
        public StringToValue<int> age;

        [SerializeField]
        public StringToValue<DateTime> date;

        [SerializeField]
        public CacheControl cacheControl;

        [SerializeField]
        public StringToValue<int> contentLength;

        [SerializeField]
        private int initialAge = 0;

        [SerializeField]
        private int freshnessLifetime = 0;

        [SerializeField]
        internal State state;

        [SerializeField]
        internal int index;

        internal bool requestInPlay = false;

        private long lastCheckTime = 0;

        private bool lastCheckAccessWeek = false;
        private bool lastCheckAccessMonth = false;

        internal event Action<GpmCacheResult> callback;

        public CacheInfo()
        {

        }

        public CacheInfo(CacheInfo info)
        {
            this.storage = info.storage;
            this.url = info.url;
            this.eTag = info.eTag;
            this.lastModified = info.lastModified;
            this.lastAccess = info.lastAccess;
            this.expires = info.expires;
            this.requestTime = info.requestTime;
            this.responseTime = info.responseTime;
            this.age = info.age;
            this.date = info.date;
            this.cacheControl = info.cacheControl;
            this.contentLength = info.contentLength;
            this.initialAge = info.initialAge;
            this.freshnessLifetime = info.freshnessLifetime;
            this.state = info.state;
            this.index = info.index;
            this.requestInPlay = info.requestInPlay;
            this.lastCheckTime = info.lastCheckTime;
            this.lastCheckAccessWeek = info.lastCheckAccessWeek;
            this.lastCheckAccessMonth = info.lastCheckAccessMonth;
        }

        public CacheInfo(CachePackage storage, string url)
        {
            this.storage = storage;
            this.url = url;
        }

        public CacheInfo(int index)
        {
            this.index = index;
        }
        
        public int CompareTo(CacheInfo other)
        {
            bool lastAccessMonth = IsLastAccessMonth();
            if (lastAccessMonth != other.IsLastAccessMonth())
            {
                if (lastAccessMonth == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            bool lastAccessWeek = IsLastAccessWeek();
            if (lastAccessWeek != other.IsLastAccessWeek())
            {
                if (lastAccessWeek == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            bool expired = IsExpired();
            if (expired != other.IsExpired())
            {
                if (expired == true)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return -other.index.CompareTo(index);
        }

        public bool IsCached()
        {
            return index > 0;
        }

        public int GetSpaceID()
        {
            return this.index;
        }

        private int FromToSeconds(DateTime from, DateTime to)
        {
            return (int)(to - from).TotalSeconds;
        }

        public int GetCurrentAge()
        {
            int residentTime = FromToSeconds(date, DateTime.UtcNow);
            return initialAge + residentTime;
        }

        public bool NeedRequest(double reRequestTime = 0)
        {
            if (IsAlways() == true)
            {
                return true;
            }

            if (reRequestTime == 0)
            {
                reRequestTime = GpmCacheStorage.GetReRequestTime();
            }
            
            if (CheckReRequest(reRequestTime) == true)
            {
                return true;
            }

            return IsExpired();
        }

        public void CheckState()
        {
            if (lastCheckTime != GpmCacheStorage.updateTime)
            {
                lastCheckTime = GpmCacheStorage.updateTime;

                lastCheckAccessWeek = IsLastAccessPeriod(SECONDS_PER_DAY * 7);
                lastCheckAccessMonth = IsLastAccessPeriod(SECONDS_PER_DAY * 30);

                if (state != State.EXPIRED &&
                    IsFresh() == false)
                {
                    state = State.EXPIRED;
                }
            }
        }

        public bool IsExpired()
        {
            CheckState();
            return state == State.EXPIRED;
        }

        public bool IsLastAccessWeek()
        {
            CheckState();

            return lastCheckAccessWeek;
        }

        public bool IsLastAccessMonth()
        {
            CheckState();

            return lastCheckAccessMonth;
        }

        public bool IsLastAccessPeriod(double periodSecond)
        {
            return DateTime.UtcNow.Ticks - lastAccess > TimeSpan.TicksPerSecond * periodSecond;
        }

        public double GetPastTimeFromLastAccess()
        {
            return (DateTime.UtcNow.Ticks - lastAccess) / TimeSpan.TicksPerSecond;
        }

        public double GetPastTimeFromResponse()
        {
            if( responseTime != null &&
                responseTime.IsValid() == true)
            {
                return (DateTime.UtcNow - responseTime).Ticks / TimeSpan.TicksPerSecond;
            }

            return 0;
        }

        public bool CheckReRequest(double reRequestTime)
        {
            if( reRequestTime > 0 &&
                responseTime != null &&
                responseTime.IsValid() == true)
            {
                return (DateTime.UtcNow - responseTime).Ticks > TimeSpan.TicksPerSecond * reRequestTime;
            }

            return false;
        }

        public bool IsFresh()
        {
            if(freshnessLifetime > 0)
            {
                return freshnessLifetime > GetCurrentAge();
            }
            else
            {
                return false;
            }
        }

        public bool IsAlways()
        {
            if (freshnessLifetime == 0)
            {
                return true;
            }

            if( string.IsNullOrEmpty(eTag) == true &&
                (lastModified == null || 
                 lastModified.IsValid() == false))
            { 
                return true;
            }

            if (cacheControl != null)
            {
                if (cacheControl.noCache == true)
                {
                    return true;
                }
                else if (cacheControl.maxAge.IsValid() == true)
                {
                    if (cacheControl.maxAge == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public double GetRemainRequestTime()
        {
            double remainTime = 0;

            int freshnessLifetime = CaculateFreshnessLifeTime();
            if (freshnessLifetime > 0)
            {
                remainTime = freshnessLifetime - GetCurrentAge();
                if (remainTime <= 0)
                {
                    return 0;
                }

                double reRequestTime = GpmCacheStorage.GetReRequestTime();
                if (reRequestTime > 0)
                {
                    double passTime = GetPastTimeFromResponse();
                    double remainReReqeustTime = reRequestTime - passTime;
                    if (remainReReqeustTime <= 0)
                    {
                        return 0;
                    }

                    if (remainTime >= remainReReqeustTime)
                    {
                        remainTime = remainReReqeustTime;
                    }
                }
            }

            return remainTime;
        }

        public void CaculateCacheInfo()
        {
            CaculateInitialAge();
            CaculateFreshnessLifeTime();
        }

        public int CaculateInitialAge()
        {
            int apparentAge = Math.Max(0, FromToSeconds(date, responseTime));

            int responseDelay = FromToSeconds(requestTime, responseTime);
            int correctedAgeValue = age + responseDelay;

            initialAge = Math.Max(apparentAge, correctedAgeValue);
            return initialAge;
        }

        public int CaculateFreshnessLifeTime()
        {
            freshnessLifetime = 0;

            if (cacheControl != null &&
                cacheControl.maxAge.IsValid() == true)
            {
                freshnessLifetime = cacheControl.maxAge;
            }
            else if (expires != null)
            {
                if (date != null)
                {
                    freshnessLifetime = FromToSeconds(date, expires);
                }
                else
                {
                    freshnessLifetime = FromToSeconds(responseTime, expires);
                }
            }
            return freshnessLifetime;
        }

        internal void SendResult(byte[] datas, bool updateData)
        {
            callback?.Invoke(new GpmCacheResult(this, datas, updateData));
            callback = null;
        }
    }
}
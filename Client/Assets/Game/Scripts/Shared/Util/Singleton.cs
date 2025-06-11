using System;
using UnityEditor.Rendering;
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    protected static object _instanceLock = new object();
    protected static volatile T _instance;
    public static T Instance {
        get {
            lock (_instanceLock) {
                if (null == _instance)
                    _instance = new T();
            }
            return _instance;
        }
    }

}

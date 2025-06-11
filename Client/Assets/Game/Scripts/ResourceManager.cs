using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class ResourceManager : MonoBehaviour {
    private const string logger = nameof(ResourceManager);
    private Dictionary<string, object> Resources = new Dictionary<string, object>();
    public static ResourceManager m_instance;
    public static ResourceManager Instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType(typeof(ResourceManager)) as ResourceManager;
                if (m_instance == null) {
                    var go = new GameObject(logger);
                    m_instance = go.AddComponent<ResourceManager>();
                }

                DontDestroyOnLoad(m_instance);
                m_instance.Reserve();
            }

            return m_instance;
        }
    }

    private void Awake()
    {

    }

    private void Reserve()
    {
    }

    private void Update()
    {

    }

    public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(string assetPath, Action<TObject> succeed, Action<string> failed = null) where TObject : UnityEngine.Object
    {
        var asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(assetPath);
        //asyncOperationHandle
        asyncOperationHandle.Completed += op => {
            if (op.Status == AsyncOperationStatus.Failed) {
                GameLogger.Log.Error(logger, $"Resource Load Failed Path({assetPath})");
                failed?.Invoke($"Resource Load Failed Path({assetPath})");
            } else if (op.Status == AsyncOperationStatus.Succeeded) {
                if (Instance.Resources.TryGetValue(assetPath, out var resource) == false) {
                    resource = op.Result;
                    Instance.Resources.Add(assetPath, resource);
                }

                succeed.Invoke((TObject)resource);
            }

        };
        return asyncOperationHandle;
    }

    public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(string assetPath, Action<TObject, object[]> action, params object[] param) where TObject : UnityEngine.Object
    {
        var asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(assetPath);
        //asyncOperationHandle
        asyncOperationHandle.Completed += op => {
            if (op.Status == AsyncOperationStatus.Failed) {
                GameLogger.Log.Error(logger, $"Resource Load Failed Path({assetPath})");
            } else if (op.Status == AsyncOperationStatus.Succeeded) {
                if (Instance.Resources.TryGetValue(assetPath, out var resource) == false) {
                    resource = op.Result;
                    Instance.Resources.Add(assetPath, resource);
                }

                action.Invoke((TObject)resource, param);
            }

        };
        return asyncOperationHandle;
    }

    public static void Load<TObject>(string assetPath, Action<TObject> action) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath)) {
            GameLogger.Log.Error(logger, "Path Is Null Or Empty");
            return;
        }

        if (Instance.Resources.TryGetValue(assetPath, out var resource)) {
            action.Invoke((TObject)resource);
            return;
        }

        var asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(assetPath);
        //asyncOperationHandle
        asyncOperationHandle.Completed += op => {
            if (op.Status == AsyncOperationStatus.Failed) {
                GameLogger.Log.Error(logger, $"Resource Load Failed Path({assetPath})");
            } else if (op.Status == AsyncOperationStatus.Succeeded) {
                if (Instance.Resources.TryGetValue(assetPath, out var resource) == false) {
                    resource = op.Result;
                    Instance.Resources.Add(assetPath, resource);
                }

                action?.Invoke((TObject)resource);
            }
        };
    }

    public static TObject Load<TObject>(string assetPath) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath)) {
            GameLogger.Log.Error(logger, "Path Is Null Or Empty");
            return null;
        }

        if (Instance.Resources.TryGetValue(assetPath, out var resource)) {
            return (TObject)resource;
        }

        return null;
    }

    public static void LoadInstantiate<TObject>(string assetPath, Action<TObject> action) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath)) {
            GameLogger.Log.Error(logger, "Path Is Null Or Empty");
            return;
        }

        if (Instance.Resources.TryGetValue(assetPath, out var resource)) {
            var tobj = Instantiate((TObject)resource);
            tobj.name = assetPath;
            action.Invoke(tobj);
            return;
        }

        var asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(assetPath);
        //asyncOperationHandle
        asyncOperationHandle.Completed += op => {
            if (op.Status == AsyncOperationStatus.Failed) {
                GameLogger.Log.Error(logger, $"Resource Load Failed Path({assetPath})");
            } else if (op.Status == AsyncOperationStatus.Succeeded) {
                if (Instance.Resources.TryGetValue(assetPath, out var resource) == false) {
                    resource = op.Result;
                    Instance.Resources.Add(assetPath, resource);
                }

                var tobj = Instantiate((TObject)resource);
                tobj.name = assetPath;
                action?.Invoke(tobj);
            }
        };
    }

    public static void LoadInstantiate<TObject>(string assetPath, Action<TObject, object[]> action, params object[] param) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath)) {
            GameLogger.Log.Error(logger, "Path Is Null Or Empty");
            return;
        }

        if (Instance.Resources.TryGetValue(assetPath, out var resource)) {
            var tobj = Instantiate((TObject)resource);
            action.Invoke(tobj, param);
            return;
        }

        var asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(assetPath);
        //asyncOperationHandle
        asyncOperationHandle.Completed += op => {
            if (op.Status == AsyncOperationStatus.Failed) {
                GameLogger.Log.Error(logger, $"Resource Load Failed Path({assetPath})");
            } else if (op.Status == AsyncOperationStatus.Succeeded) {
                if (Instance.Resources.TryGetValue(assetPath, out var resource) == false) {
                    resource = op.Result;
                    Instance.Resources.Add(assetPath, resource);
                }

                var tobj = Instantiate((TObject)resource);
                action?.Invoke(tobj, param);
            }
        };
    }

    public static void LoadInstantiate<TObject>(string assetPath, Transform parent, Action<TObject> action) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath)) {
            GameLogger.Log.Error(logger, "Path Is Null Or Empty");
            return;
        }

        if (Instance.Resources.TryGetValue(assetPath, out var resource)) {
            var tobj = Instantiate((TObject)resource, parent);
            action.Invoke(tobj);
            return;
        }

        var asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(assetPath);
        //asyncOperationHandle
        asyncOperationHandle.Completed += op => {
            if (op.Status == AsyncOperationStatus.Failed) {
                GameLogger.Log.Error(logger, $"Resource Load Failed Path({assetPath})");
            } else if (op.Status == AsyncOperationStatus.Succeeded) {
                if (Instance.Resources.TryGetValue(assetPath, out var resource) == false) {
                    resource = op.Result;
                    Instance.Resources.Add(assetPath, resource);
                }

                var tobj = Instantiate((TObject)resource, parent);
                action?.Invoke(tobj);
            }
        };
    }

    public static TObject LoadInstantiate<TObject>(string assetPath) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetPath)) {
            GameLogger.Log.Error(logger, "Path Is Null Or Empty");
            return null;
        }

        if (Instance.Resources.TryGetValue(assetPath, out var resource)) {
            var tobj = Instantiate((TObject)resource);
            return tobj;
        }

        return null;
    }

    public static void UnloadAsset(string assetName)
    {
        Addressables.Release(assetName);
    }
}

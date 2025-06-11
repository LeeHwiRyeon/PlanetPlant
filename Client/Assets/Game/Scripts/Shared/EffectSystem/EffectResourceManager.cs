using Cysharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectResourceManager : MonoBehaviour {
    public enum State {
        Nothing,
        Waiting,
        Loading,
        Completed,
        NotFound
    }

    public class EffectPreset {
        public State State = State.Nothing;
        public int Priority = 0;
        public GameObject GameObject = null;
        public string AssetName = null;
        public Coroutine Coroutine = null;
    }

    public bool IsReserveLoading => CheckReserve();

    private readonly Dictionary<string, EffectPreset> m_effectPresets = new Dictionary<string, EffectPreset>(100, System.StringComparer.Ordinal);
    private readonly List<EffectPreset> m_reserveList = new List<EffectPreset>(50);
    private readonly Queue<string> m_effectWaitingQueue = new Queue<string>(20);
    private readonly List<string> m_removePresetNames = new List<string>(50);

    public State GetPreset(string key, out GameObject go, int priority = 0)
    {
        go = null;

        if (string.IsNullOrEmpty(key)) {
            return State.NotFound;
        }

        m_effectPresets.TryGetValue(key, out var preset);
        if (preset == null) {
            preset = new EffectPreset();
            m_effectPresets.Add(key, preset);
        }
        preset.Priority = Mathf.Max(preset.Priority, priority);

        if (preset.State == State.Nothing) {
            preset.State = State.Waiting;
            if (preset.Coroutine != null) {
                StopCoroutine(preset.Coroutine);
            }
            m_effectWaitingQueue.Enqueue(key);
        }

        go = preset.GameObject;
        return preset.State;
    }

    public void Reserve(string key, int priority = 0)
    {
        if (string.IsNullOrEmpty(key) || key.Equals("None", StringComparison.OrdinalIgnoreCase)) {
            return;
        }

        m_effectPresets.TryGetValue(key, out var preset);
        if (preset == null) {
            preset = new EffectPreset();
            m_effectPresets.Add(key, preset);
        }
        preset.Priority = Mathf.Max(preset.Priority, priority);

        if (preset.State == State.Nothing) {
            preset.State = State.Waiting;
            if (preset.Coroutine != null) {
                StopCoroutine(preset.Coroutine);
            }
            m_reserveList.Add(preset);
            m_effectWaitingQueue.Enqueue(key);
        }
    }

    private IEnumerator LoadEffectAsync(string assetName)
    {
        m_effectPresets.TryGetValue(assetName, out var preset);
        if (preset == null) {
            yield break;
        }

        GameObject go = null;
        var request = ResourceManager.LoadAssetAsync<GameObject>(assetName, (obj) => {
            preset.AssetName = assetName;
            go = obj;
        });

        yield return request;

        if (go == null) {
            preset.State = State.NotFound;
            preset.Coroutine = null;
            Debug.LogError(ZString.Format("[Effect][NotFound] {0}", assetName));
            yield break;
        }

        preset.GameObject = go;
        preset.State = State.Completed;
        preset.Coroutine = null;
        Debug.Log(ZString.Format("[Effect][Loaded] {0}", assetName));
    }

    public void Unload(string key, int priority = 0)
    {
        if (string.IsNullOrEmpty(key)) {
            return;
        }

        m_effectPresets.TryGetValue(key, out var preset);
        if (preset == null) {
            return;
        }
        if (preset.Priority > priority) {
            return;
        }

        ResetPreset(preset);
        m_effectPresets.Remove(key);
    }

    public void Clear(int priority = 0)
    {
        m_reserveList.Clear();

        foreach (var pair in m_effectPresets) {
            var preset = pair.Value;
            if (preset == null) {
                continue;
            }
            if (preset.Priority > priority) {
                continue;
            }

            ResetPreset(preset);
            m_removePresetNames.Add(pair.Key);
        }

        foreach (var key in m_removePresetNames) {
            m_effectPresets.Remove(key);
        }
        m_removePresetNames.Clear();
    }

    private bool CheckReserve()
    {
        for (var i = 0; i < m_reserveList.Count; i++) {
            var preset = m_reserveList[i];
            if (preset == null) {
                m_reserveList.RemoveAt(i--);
            } else if (preset.State != State.Waiting && preset.State != State.Loading) {
                m_reserveList.RemoveAt(i--);
            }
        }

        return m_reserveList.Count > 0;
    }

    private void ResetPreset(EffectPreset preset)
    {
        if (preset == null) {
            return;
        }

        if (preset.Coroutine != null) {
            StopCoroutine(preset.Coroutine);
            preset.Coroutine = null;
        }

        preset.State = State.Nothing;
        preset.GameObject = null;
        if (string.IsNullOrEmpty(preset.AssetName) == false) {
            ResourceManager.UnloadAsset(preset.AssetName);
            preset.AssetName = null;
        }
    }

    private void Update()
    {
        var dequeueCount = 0;
        foreach (var key in m_effectWaitingQueue) {
            m_effectPresets.TryGetValue(key, out var preset);
            if (preset == null || preset.State == State.Completed || preset.State == State.NotFound) {
                dequeueCount++;
                continue;
            }
            if (preset.State == State.Loading) {
                break;
            }

            preset.State = State.Loading;
            preset.Coroutine = StartCoroutine(LoadEffectAsync(key));
            dequeueCount++;
            break;
        }

        if (dequeueCount < m_effectWaitingQueue.Count) {
            for (var i = 0; i < dequeueCount; i++) {
                m_effectWaitingQueue.Dequeue();
            }
        } else {
            m_effectWaitingQueue.Clear();
        }

        CheckReserve();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class EffectSystem {
    private static EffectSystem m_instance;
    public static EffectSystem Instance {
        get {
            if (m_instance == null || m_EffectsRoot == null) {
                m_instance = new EffectSystem();
                m_instance.Init();
            }
            return m_instance;
        }
    }

    public class EffectInstance {
        public GameObject GameObject = null;
        public Transform Transform = null;
        public Transform[] Transforms = null;
        public EffectLifeManager LifeManager = null;
        public Animator[] Animators = null;
        public Renderer[] Renderers = null;
        public bool Reusable = true;
        public bool AttachOnlyPos = false;
        public Vector3 FixedRotation = Vector3.zero;
        public float UnusedTime = 0.0f;

        public bool IsPlaying {
            get {
                if (GameObject == null) {
                    return false;
                }
                return GameObject.activeInHierarchy;
            }
        }
    }

    private class EffectInstanceGroup {
        public readonly List<EffectInstance> instanceList = new List<EffectInstance>(4);
    }

    private readonly Dictionary<string, EffectInstanceGroup> m_effectInstances = new Dictionary<string, EffectInstanceGroup>(20, System.StringComparer.Ordinal);
    private static GameObject m_EffectsRoot = null;
    private EffectResourceManager m_effectResourceManager = null;
    private const float m_keepTime = 30.0f;

    public bool IsReserveLoading {
        get {
            if (m_effectResourceManager == null) {
                return false;
            }

            return m_effectResourceManager.IsReserveLoading;
        }
    }

    private void Init()
    {
        if (m_EffectsRoot == null) {
            m_EffectsRoot = new GameObject("Effects");
            Object.DontDestroyOnLoad(m_EffectsRoot);
        }

        if (m_effectResourceManager == null) {
            m_effectResourceManager = m_EffectsRoot.AddComponent<EffectResourceManager>();
            // TODO: 임시처리 나중에 오브젝트 생성시점에 사용하는 이펙트 캐싱하게 변경 예정

        }
    }

    public void Update(float deltaTime)
    {
        foreach (var pair in m_effectInstances) {
            var group = pair.Value;
            if (group == null) {
                continue;
            }

            for (var index = group.instanceList.Count - 1; index >= 0; index--) {
                var instance = group.instanceList[index];
                if (instance == null) {
                    continue;
                }
                if (instance.GameObject == null) {
                    group.instanceList.RemoveAt(index);
                    continue;
                }
                if (instance.Reusable == false) {
                    continue;
                }
                if (instance.GameObject.activeInHierarchy) {
                    continue;
                }

                instance.UnusedTime += deltaTime;
                if (instance.UnusedTime > m_keepTime) {
                    DestroyRenderers(instance.Renderers);
                    Object.Destroy(instance.GameObject);
                    group.instanceList.RemoveAt(index);
                }
            }
        }
    }

    public void LateUpdate(float deltaTime)
    {
        foreach (var pair in m_effectInstances) {
            var group = pair.Value;
            if (group == null) {
                continue;
            }

            for (var index = group.instanceList.Count - 1; index >= 0; index--) {
                var instance = group.instanceList[index];
                if (instance == null) {
                    continue;
                }
                if (instance.AttachOnlyPos == false) {
                    continue;
                }
                if (instance.GameObject == null) {
                    continue;
                }
                if (instance.GameObject.activeInHierarchy == false) {
                    continue;
                }
                if (instance.Transform == null) {
                    continue;
                }
                instance.Transform.eulerAngles = instance.FixedRotation;
            }
        }
    }

    public void Return()
    {
        foreach (var effectInstanceGroup in m_effectInstances.Values) {
            foreach (var effectInstance in effectInstanceGroup.instanceList) {
                Return(effectInstance, true);
            }
        }
    }

    public void Return(EffectInstance effectInstance, bool immediate = false)
    {
        if (m_EffectsRoot == null) {
            return;
        }
        if (effectInstance == null) {
            return;
        }
        if (effectInstance.GameObject == null) {
            return;
        }
        if (effectInstance.Transform == null) {
            return;
        }

        effectInstance.Reusable = true;

        if (effectInstance.Animators != null) {
            foreach (var t in effectInstance.Animators) {
                t.Rebind();
            }
        }

        if (effectInstance.Transform.parent != m_EffectsRoot.transform) {
            var activeHierarchy = effectInstance.GameObject.activeInHierarchy;
            effectInstance.Transform.SetParent(m_EffectsRoot.transform);
            effectInstance.GameObject.SetActive(activeHierarchy);
        }

        if (effectInstance.LifeManager) {
            effectInstance.LifeManager.Stop(immediate);
        }
    }

    public void Reserve(string effectName, int priority = 0)
    {
        if (m_effectResourceManager == null) {
            return;
        }

        m_effectResourceManager.Reserve(effectName, priority);
    }

    public EffectInstance Create(string effectName, Vector3 position, Vector3 rotation, Vector3 scale, bool attach, Transform target, int layer, bool attachOnlyPos = false, bool autoReturn = true)
    {
        if (m_effectResourceManager == null) {
            return null;
        }

        m_effectResourceManager.GetPreset(effectName, out var preset);
        if (preset == null) {
            return null;
        }

        m_effectInstances.TryGetValue(effectName, out var effectInstanceGroup);
        if (effectInstanceGroup == null) {
            effectInstanceGroup = new EffectInstanceGroup();
            m_effectInstances[effectName] = effectInstanceGroup;
        }

        EffectInstance effectInstance = null;
        for (var i = 0; i < effectInstanceGroup.instanceList.Count; i++) {
            var currentInstance = effectInstanceGroup.instanceList[i];
            if (currentInstance.GameObject == null) {
                continue;
            }
            if (currentInstance.Reusable == false || currentInstance.GameObject.activeInHierarchy ||
                currentInstance.GameObject.activeSelf) {
                continue;
            }

            currentInstance.GameObject.SetActive(true);
            currentInstance.UnusedTime = 0.0f;
            effectInstance = currentInstance;
            break;
        }

        if (effectInstance == null) {
            effectInstance = new EffectInstance {
                GameObject = Object.Instantiate(preset.gameObject)
            };
            effectInstance.Transform = effectInstance.GameObject.transform;
            effectInstance.Transforms = effectInstance.GameObject.GetComponentsInChildren<Transform>();
            effectInstance.LifeManager = effectInstance.GameObject.AddComponent<EffectLifeManager>();
            effectInstance.Animators = effectInstance.GameObject.GetComponentsInChildren<Animator>();
            effectInstance.Renderers = effectInstance.GameObject.GetComponentsInChildren<Renderer>();
            effectInstance.Transform.SetParent(m_EffectsRoot.transform);
            effectInstanceGroup.instanceList.Add(effectInstance);
        }

        effectInstance.GameObject.layer = layer;
        if (effectInstance.Transforms != null) {
            foreach (var tr in effectInstance.Transforms) {
                tr.gameObject.layer = layer;
            }
        }

        AttachToTarget(effectInstance, position, rotation, scale, attach, target, attachOnlyPos);

        if (effectInstance.LifeManager) {
            effectInstance.LifeManager.Duration = 0.0f;
            effectInstance.LifeManager.AutoReturn(autoReturn);
            effectInstance.LifeManager.ClearTrail();
        }

        if (effectInstance.Animators != null) {
            foreach (var animator in effectInstance.Animators) {
                animator.enabled = true;
            }
        }

        return effectInstance;
    }

    public void AttachToTarget(EffectInstance effectInstance, Vector3 position, Vector3 rotation, Vector3 scale, bool attach = false, Transform target = null, bool attachOnlyPos = false)
    {
        if (effectInstance == null) {
            return;
        }
        if (effectInstance.Transform == null) {
            return;
        }

        effectInstance.AttachOnlyPos = attachOnlyPos;
        if (target) {
            effectInstance.Transform.SetParent(target, false);
            effectInstance.Transform.localPosition = position;
            if (attachOnlyPos) {
                effectInstance.Transform.eulerAngles = rotation;
                effectInstance.FixedRotation = rotation;
            } else {
                effectInstance.Transform.localEulerAngles = rotation;
            }
            effectInstance.Transform.localScale = scale;
            if (attach == false) {
                effectInstance.Transform.SetParent(m_EffectsRoot.transform);
            }
        } else {
            effectInstance.Transform.position = position;
            effectInstance.Transform.eulerAngles = rotation;
            effectInstance.Transform.localScale = scale;
        }
    }

    public void Clear(int priority = 0)
    {
        foreach (var pair in m_effectInstances) {
            var group = pair.Value;
            if (group == null) {
                continue;
            }

            foreach (var instance in group.instanceList) {
                if (instance == null) {
                    continue;
                }

                instance.Animators = null;
                DestroyRenderers(instance.Renderers);
                instance.Renderers = null;
                if (instance.GameObject) {
                    Object.Destroy(instance.GameObject);
                }
            }
            group.instanceList.Clear();
        }

        if (m_effectResourceManager != null) {
            m_effectResourceManager.Clear(priority);
        }
    }

    private void DestroyRenderers(Renderer[] renderers)
    {
        if (renderers != null) {
            foreach (var r in renderers) {
                if (r.materials != null) {
                    foreach (var m in r.materials) {
                        Object.Destroy(m);
                    }
                }

                Object.Destroy(r);
            }
        }
    }
}

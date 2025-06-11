using System.Collections.Generic;
using UnityEngine;

public class EffectLifeManager : MonoBehaviour {
    public float Duration = 0.0f;
    private bool m_first = true;
    private bool m_fadeOut = false;
    private float m_elapsedTime = 0.0f;
    private bool m_autoReturn;
    private List<ParticleSystem> m_particleSystems = null;
    private bool[] m_particleSystemLoopingOption;
    private Animator[] m_animators = null;
    private TrailRenderer[] m_trailRenderers = null;
    private MeshRenderer[] m_MeshRenderers = null;

    private void OnEnable()
    {
        if (m_first) {
            var particleSystems = GetComponentsInChildren<ParticleSystem>();
            if (particleSystems != null && particleSystems.Length > 0) {
                m_particleSystems = new List<ParticleSystem>(particleSystems.Length);
                foreach (var particleSystem in particleSystems) {
                    var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                    if (renderer == null) {
                        continue;
                    }
                    if (renderer.enabled == false) {
                        continue;
                    }
                    m_particleSystems.Add(particleSystem);
                }
            }

            m_animators = GetComponentsInChildren<Animator>();
            m_trailRenderers = GetComponentsInChildren<TrailRenderer>();
            m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
            m_first = false;
        }

        ClearTrail();

        if (m_MeshRenderers != null) {
            foreach (var meshRenderer in m_MeshRenderers) {
                meshRenderer.gameObject.SetActive(true);
            }
        }

        m_elapsedTime = 0.0f;
        m_fadeOut = false;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy == false) {
            return;
        }

        var isPlaying = false;
        m_elapsedTime += Time.deltaTime;
        var stop = (m_fadeOut || (0.0f < Duration && Duration <= m_elapsedTime));

        if (m_particleSystems != null) {
            foreach (var particleSystem in m_particleSystems) {
                if (particleSystem.gameObject.activeInHierarchy == false) {
                    continue;
                }
                if (particleSystem.main.maxParticles == 0) {
                    continue;
                }
                if (particleSystem.isPlaying == false) {
                    continue;
                }

                isPlaying = true;
                if (stop == false) {
                    continue;
                }
                if (particleSystem.main.loop == false) {
                    continue;
                }
                if (particleSystem.isEmitting == false) {
                    continue;
                }
                particleSystem.Stop();
            }
        }

        foreach (var animator in m_animators) {
            if (animator.gameObject.activeInHierarchy == false) {
                continue;
            }

            var isAnimatorPlaying = true;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.loop && stop) {
                animator.StopPlayback();
                isAnimatorPlaying = false;
            } else if (stateInfo.normalizedTime < 0.0f || 1.0f < stateInfo.normalizedTime) {
                isAnimatorPlaying = false;
            }
            isPlaying |= isAnimatorPlaying;
        }

        if (m_trailRenderers != null) {
            foreach (var trail in m_trailRenderers) {
                if (m_elapsedTime < trail.time) {
                    isPlaying = true;
                    break;
                }
            }
        }

        if (m_MeshRenderers != null) {
            foreach (var meshRenderer in m_MeshRenderers) {
                if (meshRenderer == null) {
                    continue;
                }
                if (stop == false) {
                    continue;
                }
                meshRenderer.gameObject.SetActive(false);
            }
        }

        m_fadeOut = false;
        if (isPlaying == false && m_autoReturn) {
            gameObject.SetActive(false);
        }
    }

    public void SetDurationTime(float time)
    {
        Duration = time;
    }

    public void Stop(bool immediate)
    {
        m_autoReturn = true;
        if (immediate) {
            gameObject.SetActive(false);
        } else {
            m_fadeOut = true;
        }
    }

    public void SetParticleLooping(bool active)
    {
        var first = false;
        if (m_particleSystemLoopingOption == null) {
            first = true;
            m_particleSystemLoopingOption = new bool[m_particleSystems.Count];
        }

        for (var i = 0; i < m_particleSystems.Count; i++) {
            var main = m_particleSystems[i].main;
            if (first) {
                m_particleSystemLoopingOption[i] = main.loop;
            }
            main.loop = active;
        }
    }

    public void OnDisable()
    {
        if (m_particleSystems != null && m_particleSystemLoopingOption != null) {
            for (var i = 0; i < m_particleSystems.Count; i++) {
                var main = m_particleSystems[i].main;
                main.loop = m_particleSystemLoopingOption[i];
            }
        }
    }

    public void AutoReturn(bool autoReturn)
    {
        m_autoReturn = autoReturn;
    }

    public void ClearTrail()
    {
        if (m_trailRenderers != null) {
            foreach (var trail in m_trailRenderers) {
                trail.Clear();
            }
        }
    }
}

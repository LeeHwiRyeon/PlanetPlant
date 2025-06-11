using UnityEngine;
using Util;

public static class GameUtils {

    public static void PlayHitEffect(this GameObject go, int collidedMantlePiece, Transform transform)
    {
        switch (collidedMantlePiece) {
            default: {
                    var index = RAND.RangeInt(0, 2);
                    SoundManager.Instance.PlayOneShot(index == 0 ? FixedResourceNames.stuckground_1 : FixedResourceNames.stuckground_2);
                    EffectSystem.Instance.Create(FixedResourceNames.Hit_Ground, new Vector3(0, -0.4f, 0), new Vector3(0, 0, 180), Vector3.one, true, transform, go.layer);
                }
                break;

            case 1: //물소리
            case 17: {
                    SoundManager.Instance.PlayOneShot(FixedResourceNames.watersplash_1);
                    EffectSystem.Instance.Create(FixedResourceNames.Hit_Water, new Vector3(0, -0.4f, 0), new Vector3(0, 0, 180), Vector3.one, true, transform, go.layer);
                }
                break;
            case 10:
                break;
        }
    }

    public static void PlayHitEffect(this GameObject go, string effectName, string soundName, Transform transform)
    {
        SoundManager.Instance.PlayOneShot(soundName);
        EffectSystem.Instance.Create(effectName, new Vector3(0, -0.4f, 0), new Vector3(0, 0, 0), Vector3.one, true, transform, go.layer);
    }
}
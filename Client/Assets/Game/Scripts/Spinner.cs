using Cysharp.Text;
using UnityEngine;

public class Spinner : MonoBehaviour {
    public float speed;
    public bool ClearStop;
    private int index = 1;
    private int MaxIndex;
    private const float frameDelay = 0.06f;
    private float t;
    public int Star;
    private void Update()
    {
        if (ClearStop && (PlanetManager.Inst.Planet == null || PlanetManager.Inst.Planet.IsEndGame)) {
            if (Star > 0) {
                transform.rotation = Quaternion.identity;
                t += Time.deltaTime;
                if (frameDelay < t) {
                    t = 0;
                    var str = "ClearThreeStar";
                    MaxIndex = 120;
                    if (Star == 2) {
                        str = "ClearTwoStars";
                    } else if (Star == 1) {
                        str = "ClearOneStar";
                        MaxIndex = 60;
                    }

                    var i = ZString.Format("{0}{1}", str, index);
                    ResourceManager.Load<Sprite>(i, (sprite) => {
                        PlanetManager.Inst.Planet.PlanetSprite = sprite;
                        PlanetManager.Inst.Planet.ApplyShaderSettings();
                        index++;
                        if (index > MaxIndex) {
                            index = 1;
                        }
                    });
                }
            }
            return;
        }

        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}

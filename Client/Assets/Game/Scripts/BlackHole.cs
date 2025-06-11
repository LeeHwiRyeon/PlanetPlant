using UnityEngine;

public class BlackHole : MonoBehaviour {
    public Vector2 PullVlaue;

    private void Start()
    {
        PlanetManager.Inst.Planet.BlackHole += PullVlaue;
    }
}

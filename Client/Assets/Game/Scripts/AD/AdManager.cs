using UnityEngine;

public class AdManager : MonoBehaviour {
    public static AdManager _inst;

    public static AdManager Inst {
        get {
            if (_inst == null) {
                var go = new GameObject("ADManager");
                _inst = go.AddComponent<AdManager>();
            }
            return _inst;
        }
    }

    private void Update()
    {

    }
}

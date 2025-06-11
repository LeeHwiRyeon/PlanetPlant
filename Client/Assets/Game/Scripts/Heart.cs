using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour {
    public Image 하트;

    public void Init()
    {
        if (GameManager.Inst.OptionInfo.광고제거) {
            하트.color = Color.white;
        }
    }

    public void 광고제거구매()
    {
        if (GameManager.Inst.Unlock == false) {
            return;
        }

        GameManager.Inst.Unlock = !GameManager.Inst.Unlock;
        하트.color = Color.white;
        GameManager.Inst.OptionInfo.광고제거 = true;
    }
}

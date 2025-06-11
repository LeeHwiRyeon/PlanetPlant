using Cysharp.Text;
using Gpm.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanetScrollItemData : InfiniteScrollData {
    public PlanetInfo Info;
}

public class PlanetStageScrollItem : InfiniteScrollItem, IPointerClickHandler {
    private PlanetScrollItemData itemData;
    public Image[] Star;
    public TextMeshProUGUI Title;
    public Image Lock;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        itemData = scrollData as PlanetScrollItemData;

        Title.SetText(itemData.Info.Num);
        var i = 0;
        for (; i < itemData.Info.Star; i++) {
            var i1 = i;
            ResourceManager.Load<Sprite>("star1", (sprite) => {
                Star[i1].sprite = sprite;
            });
        }

        for (; i < 3; i++) {
            var i1 = i;
            ResourceManager.Load<Sprite>("star2", (sprite) => {
                Star[i1].sprite = sprite;
            });
        }

        if (GameManager.Inst.Unlock || itemData.Info.Num == 1) {
            Title.gameObject.SetActive(true);
            Lock.gameObject.SetActive(false);
        } else {
            var prevInfo = PlanetManager.Inst.GetPlanetInfo(itemData.Info.Num - 1);
            var stageLock = prevInfo?.Star == 0;
            Title.gameObject.SetActive(stageLock == false);
            Lock.gameObject.SetActive(stageLock);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Lock.gameObject.activeSelf) {
            return;
        }

        LobbyUI.Inst.ShowPopup_GameStart2(itemData);
        SoundManager.Instance.PlayOneShot("ClickSound");
        GameLogger.Log.Info("test", "클릭!", GameLogger.LogColor.Lime);
    }
}


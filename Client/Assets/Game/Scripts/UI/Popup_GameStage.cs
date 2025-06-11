using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_GameStage : MonoBehaviour {
    public TextMeshProUGUI Title;
    public Image[] Star;
    public LobbyUI LobbyUI;
    private PlanetScrollItemData m_planetScrollItemData;
    public void Init(LobbyUI lobbyUI, PlanetScrollItemData planetScrollItemData)
    {
        LobbyUI = lobbyUI;
        m_planetScrollItemData = planetScrollItemData;
        Title.text = planetScrollItemData.Info.StageName;

        var i = 0;
        for (; i < m_planetScrollItemData.Info.Star; i++) {
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
    }

    public void GameStart()
    {
        LobbyUI.GameStart(m_planetScrollItemData.Info.Name);
    }

    public void Close()
    {
        LobbyUI.ShowPopup_GameStart();
    }

    public void NextStage()
    {
        if (m_planetScrollItemData.Info.Star <= 0) {
            return;
        }

        var info = LobbyUI.Popup_PlanetStageList.GetInfo(m_planetScrollItemData.Info.NextIndex);
        if (info == null) {
            return;
        }
        Init(LobbyUI, info);
    }
}

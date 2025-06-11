using Gpm.Ui;
using UnityEngine;

public class LobbyUI : MonoBehaviour {
    private static LobbyUI m_inst;
    public static LobbyUI Inst {
        get {
            if (m_inst == null) {
                m_inst = FindObjectOfType<LobbyUI>(true);
            }
            return m_inst;
        }
    }
    // 0 메뉴
    // 1 스테이지 스크롤
    // 2 스테이지 시작
    // 3 옵션
    public MultiLayout MultiLayout;
    public UIController UIController;
    public Popup_PlanetStageList Popup_PlanetStageList;
    public Popup_GameStage Popup_GameStage;
    public Popup_Option Popup_Option;

    private void Awake()
    {
        m_inst = this;
    }

    private void OnEnable()
    {
        SetLayout(0);
        Time.timeScale = 1;
    }

    public void SetLayout(int i)
    {
        MultiLayout.SelectLayout(i);
    }

    public void ShowMenu()
    {
        SetLayout(0);
    }

    public void ContinueTheGame()
    {
        GameManager.Inst.PlanetManager.ContinueTheGame();
        UIController.SetLayout(0);
        SetLayout(0);
    }

    public void GameStart(string name)
    {
        GameManager.Inst.PlanetManager.CreatePlanet(name, true);
        UIController.SetLayout(0);
        SetLayout(0);
    }

    public void ShowPopup_GameStart()
    {
        Popup_PlanetStageList.Init();
        SetLayout(1);
    }

    public void ShowPopup_GameStart2(PlanetScrollItemData planetScrollItemData)
    {
        Popup_GameStage.Init(this, planetScrollItemData);
        SetLayout(2);
    }

    public void ShowPopup_Setting()
    {
        SetLayout(3);
        Popup_Option.Init(this);
    }

    public void Exit()
    {
        Application.Quit();
    }
}

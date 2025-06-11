using Gpm.Ui;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {
    private static InGameUI m_inst;
    public static InGameUI Inst {
        get {
            if (m_inst == null) {
                m_inst = FindObjectOfType<InGameUI>(true);
            }
            return m_inst;
        }
    }
    public UIController UIController;
    public MultiLayout MultiLayout;
    public TextMeshProUGUI StageName;
    public Animator Animator;
    public Popup_Option Popup_Option;
    public List<Image> Stars;
    public int 탄창위치;
    public int 메뉴;

    private void Awake()
    {
        m_inst = this;
    }

    public void ShowOption()
    {
        Stop();
    }

    private void OnEnable()
    {
        Play();
        Animator.Play("PlayStageName");
        Popup_Option.Init(null);
        탄창위치 = GameManager.Inst.OptionInfo.탄창위치Right ? 0 : 4;
        메뉴 = GameManager.Inst.OptionInfo.탄창위치Right ? 1 : 5;
        MultiLayout.SelectLayout(탄창위치);
    }

    public void Stop()
    {
        Animator.Play("ShowStageName");
        MultiLayout.SelectLayout(메뉴);
        Time.timeScale = 0;
    }

    public void Play()
    {
        Animator.Play("HideStageName");
        MultiLayout.SelectLayout(탄창위치);
        Time.timeScale = 1;
    }

    public void ClearStage(int star)
    {
        MultiLayout.SelectLayout(2);
        ShowStar(star);
    }

    public void ClearFailed(int star)
    {
        MultiLayout.SelectLayout(3);
        ShowStar(star);
    }

    private void ShowStar(int star)
    {
        int i = 0;
        for (; i < star; i++) {
            var i1 = i;
            ResourceManager.Load<Sprite>("star1", (sprite) => {
                Stars[i1].sprite = sprite;
            });
        }

        for (; i < 3; i++) {
            var i1 = i;
            ResourceManager.Load<Sprite>("star2", (sprite) => {
                Stars[i1].sprite = sprite;
            });
        }
    }

    public void 리스트로()
    {
        UIController.SetLayout(1);
        UIController.LobbyUI.ShowPopup_GameStart();
        Time.timeScale = 1;
    }

    public void 다음스테이지로()
    {
        GameManager.Inst.PlanetManager.NextPlanet();
        MultiLayout.SelectLayout(탄창위치);
        Time.timeScale = 1;
        Animator.Play("PlayStageName", 0, 0);
    }

    public void 다시하기()
    {
        GameManager.Inst.PlanetManager.ContinueTheGame();
        MultiLayout.SelectLayout(탄창위치);
        Time.timeScale = 1;
        Animator.Play("PlayStageName", 0, 0);
    }

    public void 효과음설정()
    {
        Popup_Option.SetEfxFx();
    }

    public void 배경음설정()
    {
        Popup_Option.SetBgm();
    }

    public void GotoMenu()
    {
        UIController.SetLayout(1);
        UIController.LobbyUI.ShowMenu();
    }

    public void 탄창위치변경()
    {
        GameManager.Inst.OptionInfo.탄창위치Right = !GameManager.Inst.OptionInfo.탄창위치Right;
        탄창위치 = GameManager.Inst.OptionInfo.탄창위치Right ? 0 : 4;
        메뉴 = GameManager.Inst.OptionInfo.탄창위치Right ? 1 : 5;
        MultiLayout.SelectLayout(메뉴);
    }

    public void SetStageName(string name)
    {
        StageName.text = name;
    }
}

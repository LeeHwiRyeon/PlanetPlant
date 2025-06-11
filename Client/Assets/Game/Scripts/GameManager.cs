using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager m_inst;
    public static GameManager Inst {
        get {
            if (m_inst == null) {
                m_inst = FindObjectOfType<GameManager>(true);
            }
            return m_inst;
        }
    }
    public bool Unlock;
    public TextMeshProUGUI ScoreText;
    public PlanetManager PlanetManager;
    public OptionInfo OptionInfo;
    public List<string> BgmList;
    public Heart Heart;
    public string Policy = "https://mr-rathole501.blogspot.com/2023/04/privacy-policy.html";
    public string Terms = "https://mr-rathole501.blogspot.com/2023/04/terms-conditions.html";

    private void Awake()
    {
        m_inst = this;
        Load();

        for (var i = 1; i < 121; i++) {
            ResourceManager.Load<Sprite>("ClearThreeStar" + i, (sprite) => {

            });
        }

        for (var i = 1; i < 121; i++) {
            ResourceManager.Load<Sprite>("ClearTwoStars" + i, (sprite) => {

            });
        }

        for (var i = 1; i < 61; i++) {
            ResourceManager.Load<Sprite>("ClearOneStar" + i, (sprite) => {

            });
        }

    }

    private void Start()
    {
        PlanetManager.Inst.CreatePlanet(PlanetManager.Inst.LastPlanet, false);
    }

    private void OnApplicationPause()
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void Load()
    {
        var optionInfoStr = PlayerPrefs.GetString("OptionInfo");
        if (string.IsNullOrEmpty(optionInfoStr) == false) {
            OptionInfo = JsonUtility.FromJson<OptionInfo>(optionInfoStr);
            SoundManager.Instance.IsBgmOn = OptionInfo.Bgm;
            SoundManager.Instance.IsEfxOn = OptionInfo.EfxFx;
            Heart.Init();
        }

        if (OptionInfo.First) {
            OptionInfo.First = false;
            OptionInfo.PlayCount = OptionInfo.MaxPlayCount;
        }

        if (OptionInfo.PolicyAgreement == false) {
            SimpleGDPR.ShowDialog(new TermsOfServiceDialog().
                SetTermsOfServiceLink(Terms).
                SetPrivacyPolicyLink(Policy),
                OnMenuClosed);
        }

    }

    private void OnMenuClosed()
    {
        OptionInfo.PolicyAgreement = true;
        Save();
    }

    private void Save()
    {
        PlanetManager.Save();
        var optionInfo = JsonUtility.ToJson(OptionInfo);
        PlayerPrefs.SetString("OptionInfo", optionInfo);
    }
}

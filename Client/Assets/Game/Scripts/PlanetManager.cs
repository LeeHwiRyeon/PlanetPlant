using Cysharp.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[Serializable]
public class PlanetInfo {
    public string Name;
    public int Star;

    [NonSerialized]
    private int m_nextIndex;
    public int NextIndex {
        get {
            if (m_nextIndex == 0) {
                // 숫자가 아닌 문자를 나타내는 정규식
                const string pattern = @"[^\d]";
                // 문자열에서 숫자 부분만 추출
                var result = Regex.Replace(Name, pattern, "");
                // 추출된 숫자 부분을 정수로 변환
                m_nextIndex = int.Parse(result);
            }
            return m_nextIndex;
        }
    }

    [NonSerialized]
    private string m_stageName;
    public string StageName {
        get {
            if (string.IsNullOrEmpty(m_stageName)) {
                // 숫자가 아닌 문자를 나타내는 정규식
                const string pattern = @"[^\d]";
                // 문자열에서 숫자 부분만 추출
                var result = Regex.Replace(Name, pattern, "");
                if (string.IsNullOrEmpty(result)) {
                    return null;
                }
                // 추출된 숫자 부분을 정수로 변환
                var num = int.Parse(result);
                m_stageName = ZString.Format("Stage\n{0}", num);
            }
            return m_stageName;
        }
    }

    [NonSerialized]
    private int m_num;
    public int Num {
        get {
            if (m_num == 0) {
                // 숫자가 아닌 문자를 나타내는 정규식
                const string pattern = @"[^\d]";
                // 문자열에서 숫자 부분만 추출
                var result = Regex.Replace(Name, pattern, "");
                // 추출된 숫자 부분을 정수로 변환
                m_num = int.Parse(result);
            }
            return m_num;
        }
    }
}

public partial class PlanetManager : MonoBehaviour {
    public static PlanetManager Inst;
    public Planet Planet;
    public Cannon Cannon;

    public Dictionary<int, PlanetInfo> PlanetInfosById = new Dictionary<int, PlanetInfo>();
    public Dictionary<string, PlanetInfo> PlanetInfosByString = new Dictionary<string, PlanetInfo>();
    public TextMeshProUGUI Count;

    public int MaxPlanet = 60;
    private void Awake()
    {
        Inst = this;
        Load();
    }

    public void Init()
    {
        // 클리어 정보 세팅
        try {
            for (var i = 0; i < MaxPlanet; i++) {
                var index = i + 1;
                var pname = ZString.Format("Planet_{0}", index);
                if (PlanetInfosById.ContainsKey(index) == false || PlanetInfosByString.ContainsKey(pname) == false) {
                    var item = new PlanetInfo {
                        Name = pname,
                        Star = 0,
                    };
                    PlanetInfosById.Add(index, item);
                    PlanetInfosByString.Add(item.Name, item);
                }
            }
        } catch (Exception e) {
            PlayerPrefs.DeleteAll();
            PlanetInfosById.Clear();
            PlanetInfosByString.Clear();
            for (var i = 0; i < MaxPlanet; i++) {
                var index = i + 1;
                var item = new PlanetInfo {
                    Name = ZString.Format("Planet_{0}", index),
                    Star = 0,
                };
                PlanetInfosById.Add(index, item);
                PlanetInfosByString.Add(item.Name, item);
            }
        }
    }

    public void Save()
    {
        var planetInfosJson = JsonConvert.SerializeObject(PlanetInfosById.Values);
        PlayerPrefs.SetString("PlanetInfos", planetInfosJson);
        PlayerPrefs.SetString("LastPlanet", LastPlanet);
    }

    public void Load()
    {
        var planetInfosJson = PlayerPrefs.GetString("PlanetInfos");
        if (string.IsNullOrEmpty(planetInfosJson) == false) {
            try {
                var PlanetInfos = JsonConvert.DeserializeObject<List<PlanetInfo>>(planetInfosJson);
                for (var i = 0; i < PlanetInfos.Count; i++) {
                    var planetInfo = PlanetInfos[i];
                    if (PlanetInfosById.ContainsKey(planetInfo.Num) == false || PlanetInfosByString.ContainsKey(planetInfo.Name) == false) {
                        PlanetInfosById.Add(planetInfo.Num, planetInfo);
                        PlanetInfosByString.Add(planetInfo.Name, planetInfo);
                    }
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
        LastPlanet = PlayerPrefs.GetString("LastPlanet", "Planet_1");

        Init();
    }

    public PlanetInfo GetPlanetInfo(string planetName)
    {
        PlanetInfosByString.TryGetValue(planetName, out var info);
        return info;
    }
    public PlanetInfo GetPlanetInfo(int index)
    {
        PlanetInfosById.TryGetValue(index, out var info);
        return info;
    }

    public void CreatePlanet(string currentPlanetName, bool ad)
    {
        ResourceManager.LoadInstantiate<GameObject>(currentPlanetName, (go) => {
            LastPlanet = currentPlanetName;
            if (Planet != null) {
                Destroy(Planet.gameObject);
                Planet = null;
            }

            if (PlanetInfosByString.TryGetValue(LastPlanet, out var info)) {
                Planet = go.GetComponent<Planet>();
                Planet.Init(this, info, Cannon);
            }
        });

        if (ad) {
            GameManager.Inst.OptionInfo.PlayCount--;
            GameLogger.Log.Info("PlanetManager", $"플레이 카운트:{GameManager.Inst.OptionInfo.PlayCount}");
            if (GameManager.Inst.OptionInfo.PlayCount <= 0) {
                //if (AdManager.Inst.IsRewardedAdLoaded()) {
                //    AdManager.Inst.ShowRewardedAd(() => {
                //        GameLogger.Log.Info("PlanetManager", "RewardedAd 광고 성공");
                //    }, (error) => {
                //        GameLogger.Log.Info("PlanetManager", $"RewardedAd 광고 실패({error})");
                //    }, () => {
                //        GameLogger.Log.Info("PlanetManager", "RewardedAd 광고 완료");
                //        GameManager.Inst.OptionInfo.PlayCount = OptionInfo.MaxPlayCount;
                //        Save();
                //    });
                //} else if (AdManager.Inst.IsInterstitialLoaded()) {
                //    AdManager.Inst.ShowInterstitial(() => {
                //        GameLogger.Log.Info("PlanetManager", "Interstitial 광고 성공");
                //        GameManager.Inst.OptionInfo.PlayCount = OptionInfo.MaxPlayCount;
                //        Save();
                //    }, (error) => {
                //        GameLogger.Log.Info("PlanetManager", $"Interstitial 광고 실패({error})");
                //    });
                //} else {
                //    GameLogger.Log.Info("PlanetManager", "광고 불러오기 실패");
                //}
            }
        }
    }

    public void ContinueTheGame()
    {
        CreatePlanet(LastPlanet, true);
    }

    public void NextPlanet()
    {
        // 숫자가 아닌 문자를 나타내는 정규식
        const string pattern = @"[^\d]";
        // 문자열에서 숫자 부분만 추출
        var result = Regex.Replace(LastPlanet, pattern, "");
        // 추출된 숫자 부분을 정수로 변환
        var num = int.Parse(result) + 1;
        LastPlanet = ZString.Format("Planet_{0}", num);
        CreatePlanet(LastPlanet, true);
    }

    public void Fire()
    {
        if (Planet == null) {
            return;
        }
        Planet.Fire();
    }

    public void CheckClear()
    {
        if (Planet == null) {
            return;
        }
        Planet.CheckClear();
    }
}


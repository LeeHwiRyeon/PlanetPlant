using Gpm.Ui;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIController : MonoBehaviour {
    public UIController Inst;
    // 1 인게임
    // 2 로비
    public MultiLayout MultiLayout;
    public LobbyUI LobbyUI;
    public InGameUI InGameUI;
    private List<int> m_availableIndexes = new List<int>();

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        SetLayout(1);
    }

    public void SetLayout(int index)
    {
        MultiLayout.SelectLayout(index);
        if (index == 0) {
            SoundManager.Instance.StopBGM();
            PlanetManager.Inst.Cannon.gameObject.SetActive(true);
        } else if (index == 1) {
            PlayRandomBGM();
            if (PlanetManager.Inst.Cannon != null) {
                PlanetManager.Inst.Cannon.gameObject.SetActive(false);
            }
        }
    }

    private void PlayRandomBGM()
    {
        var bgmList = GameManager.Inst.BgmList;
        if (m_availableIndexes.Count == 0) {
            // 모든 BGM이 재생되었으므로 인덱스를 초기화합니다.
            m_availableIndexes.AddRange(Enumerable.Range(0, GameManager.Inst.BgmList.Count));
        }

        var randomIndex = UnityEngine.Random.Range(0, m_availableIndexes.Count);
        var selectedIndex = m_availableIndexes[randomIndex];
        // 선택한 인덱스를 사용할 수 있는 인덱스 목록에서 제거합니다.
        m_availableIndexes.RemoveAt(randomIndex);

        var bgm = bgmList[selectedIndex];
        SoundManager.Instance.PlayBGM(bgm, endCallback: PlayRandomBGM);
    }
}

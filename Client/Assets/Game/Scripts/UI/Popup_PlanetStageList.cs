using Gpm.Ui;
using System.Collections.Generic;
using UnityEngine;

public class Popup_PlanetStageList : MonoBehaviour {
    public List<PlanetScrollItemData> Items = new List<PlanetScrollItemData>();
    public InfiniteScroll ScrollList;
    public void Init()
    {
        Items.Clear();

        foreach (var info in GameManager.Inst.PlanetManager.PlanetInfosById.Values) {
            var itme = new PlanetScrollItemData {
                Info = info,
            };
            Items.Add(itme);
        }
    }

    void OnEnable()
    {
        if (Items.Count <= 0) {
            return;
        }
        ScrollList.Clear();
        foreach (var itme in Items) {
            ScrollList.InsertData(itme);
        }
    }

    public PlanetScrollItemData GetInfo(int index)
    {
        if (Items.Count <= index) {
            return null;
        }

        return Items[index];
    }
}
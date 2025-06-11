using Cysharp.Text;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour {
    public List<Plant> PlantPool;

    private int m_index;
    public int RemainingPlants => PlantPool.Count - m_index;

    public Transform 발사대1;
    public Transform 발사대2;
    public Transform 나무생성위치;
    public Transform 나무장전위치;
    public Transform 발사폭발위치;
    public Vector3 StartPos;

    public List<Image> 탄창;
    public float CheckClearWaitTime = 1f;
    public float ElapsedTime = 0;

    private void Awake()
    {

        EffectSystem.Instance.Reserve(FixedResourceNames.Hit_Meteor);
        EffectSystem.Instance.Reserve(FixedResourceNames.Attack_Cannon);
        EffectSystem.Instance.Reserve(FixedResourceNames.Hit_Ground);
        EffectSystem.Instance.Reserve(FixedResourceNames.Hit_Water);
        EffectSystem.Instance.Reserve(FixedResourceNames.Hit_Lava);
        StartPos = transform.position;
    }

    private void OnEnable()
    {
        transform.position = StartPos * 1.5f;
        transform.DOMove(StartPos, 0.1f);
    }

    public void Init(List<GameObject> items)
    {
        ElapsedTime = 0;
        if (PlantPool != null) {
            EffectSystem.Instance.Return();
            for (var i = 0; i < PlantPool.Count; i++) {
                DestroyImmediate(PlantPool[i].gameObject);
            }
            PlantPool.Clear();
        } else {
            PlantPool = new List<Plant>(items.Count);
        }

        m_index = 0;
        for (var i = 0; i < items.Count; i++) {
            var item = items[i];
            var go = Instantiate(item, transform.position, Quaternion.identity);
            var plant = go.GetComponent<Plant>();
            PlantPool.Add(plant);
            go.name = ZString.Format("{0}_{1}", go.name, i);
            go.transform.parent = transform;
            go.transform.position = 나무장전위치.position;
        }

        Reload(0);
    }


    public void OnUpdate(bool clear)
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space)) {
            Fire();
        }
#endif

        if (clear) {
            return;
        }

        ElapsedTime += Time.deltaTime;
        if (CheckClearWaitTime > ElapsedTime) {
            return;
        }

        ElapsedTime = 0;
        PlanetManager.Inst.CheckClear();
    }


    public void Fire()
    {
        if (m_index >= PlantPool.Count) {
            return;
        }

        SoundManager.Instance.PlayOneShot(FixedResourceNames.cannon_1);
        var plant = PlantPool[m_index];
        plant.Fire();

        EffectSystem.Instance.Create(FixedResourceNames.Attack_Cannon, Vector3.zero, Vector3.zero, Vector3.one, true, 발사폭발위치.transform, gameObject.layer);
        Reload(m_index + 1);
        m_index++;
        ElapsedTime = 0;
    }

    public void Reload(int index)
    {
        if (PlantPool == null || index >= PlantPool.Count) {
            return;
        }

        var item = PlantPool[index];
        item.transform.parent = transform;
        var move = item.transform.DOMove(나무생성위치.position, 0.15f);
        ReloadTreeBox(index + 1);
    }

    private void ReloadTreeBox(int start)
    {
        var i = 0;
        for (; i < 4; i++) {
            if (start + i >= PlantPool.Count) {
                break;
            }
            탄창[i].gameObject.SetActive(true);
            탄창[i].sprite = PlantPool[start + i].Tree.sprite;
        }

        for (; i < 4; i++) {
            탄창[i].gameObject.SetActive(false);
        }
    }
}

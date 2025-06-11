using Cysharp.Text;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[Serializable]
public class MantlePiece {
    public int 기본타일 = 0;
    public int 화산폭팔변경 = 0;
}

public class Planet : MonoBehaviour {
    private PlanetManager m_stageManager;
    public Transform Mantle;
    public Transform Setellites;
    public Cannon Cannon;
    public SpriteRenderer PlanetSpriteRenderer;
    public Sprite PlanetSprite;
    public ClearConditions ClearConditions;
    public List<GameObject> Plants;
    public List<MantlePiece> MantlePieces = new List<MantlePiece>();
    public Animator MantleAnimator;

    private Collider2D MantleCollider2D;
    public PlanetInfo Info;
    public bool IsEndGame { get; private set; }
    private bool m_checkCount;
    public Vector2 BlackHole { get; set; }

    public void Init(PlanetManager stageManager, PlanetInfo info, Cannon cannon)
    {
        m_stageManager = stageManager;
        Info = info;
        Cannon = cannon;
        Cannon.Init(Plants);
        ApplyShaderSettings();

        MantleCollider2D = gameObject.GetComponent<Collider2D>();
        MantleAnimator = Mantle.GetComponent<Animator>();
    }

    public void Start()
    {
        InGameUI.Inst.SetStageName(Info.StageName);
    }

    private void OnEnable()
    {
        if (PlanetManager.Inst == null) {
            return;
        }

        PlanetManager.Inst.Count.gameObject.SetActive(true);
        PlanetManager.Inst.Count.SetText(ClearConditions.ClearCount);
    }

    private void Update()
    {
        if (Cannon == null) {
            return;
        }

        Cannon.OnUpdate(IsEndGame);
        if (m_checkCount) {
            SetCount();
        }
    }

    public void Restart()
    {

    }

    public void CheckCount()
    {
        m_checkCount = true;
    }

    public void SetCount()
    {

        var clearCount = 0;
        var deadCount = 0;
        for (var i = 0; i < Cannon.PlantPool.Count; i++) {
            var p = Cannon.PlantPool[i];
            if (p.PlantState == PlantState.Idle) {
                clearCount++;
            }

            if (p.PlantState == PlantState.Dead) {
                deadCount++;
            }
        }

        var count = ClearConditions.ClearCount - clearCount;
        if (count < 0) {
            count = 0;
        }
        PlanetManager.Inst.Count.SetText(count);
    }

    public void CheckClear()
    {
        var clearCount = 0;
        var deadCount = 0;

        for (var i = 0; i < Cannon.PlantPool.Count; i++) {
            var p = Cannon.PlantPool[i];
            if (p.PlantState == PlantState.Idle) {
                clearCount++;
            }

            if (p.PlantState == PlantState.Dead) {
                deadCount++;
            }
        }

        var count = ClearConditions.ClearCount - clearCount;
        if (count < 0) {
            count = 0;
        }

        PlanetManager.Inst.Count.SetText(count);
        if (Cannon.RemainingPlants == 0 || clearCount >= ClearConditions.ClearCount) {
            PlanetManager.Inst.Count.gameObject.SetActive(false);
            var star = 0;
            if (deadCount <= ClearConditions.Start3) {
                star = 3;
                SoundManager.Instance.PlayOneShot(FixedResourceNames.win_3);
                var spinner = Mantle.GetComponent<Spinner>();
                spinner.Star = star;
                MantleAnimator.Play("Clear");
            } else if (deadCount <= ClearConditions.Start2) {
                star = 2;
                SoundManager.Instance.PlayOneShot(FixedResourceNames.win_2);
                var spinner = Mantle.GetComponent<Spinner>();
                spinner.Star = star;
                MantleAnimator.Play("Clear");
            } else if (clearCount >= ClearConditions.ClearCount) {
                star = 1;
                SoundManager.Instance.PlayOneShot(FixedResourceNames.win_1);
                var spinner = Mantle.GetComponent<Spinner>();
                spinner.Star = star;
                MantleAnimator.Play("Clear");
            } else {
                InGameUI.Inst.ClearFailed(star);
                SoundManager.Instance.PlayOneShot(FixedResourceNames.lose_1);
                IsEndGame = true;
                return;
            }

            FirebaseAnalytics.LogEvent("ClearStageEvent", Info.StageName, star);
            if (Info.Star < star) {
                Info.Star = star;
            }

            IsEndGame = true;
            InGameUI.Inst.ClearStage(star);
        }
    }

    public void Fire()
    {
        Cannon.Fire();
    }

    public int GetCollidedMantlePiece(Collider2D collider2D)
    {
        var totalSegments = MantlePieces.Count;
        var collisionPosition = collider2D.bounds.ClosestPoint(transform.position);
        var centerPosition = transform.position;
        var rotationZ = Mantle.eulerAngles.z;

        // 회전을 적용한 각도를 계산합니다.
        var angle = GetAngle(collisionPosition, centerPosition, rotationZ);

        // 각도에 따른 세그먼트 인덱스를 계산합니다.
        var index = GetSegmentIndex(angle, totalSegments);
        if (index < 0) {
            index = 0;
        }

        //GameLogger.Log.Info("Planet", $"Collision angle: {angle}, Segment index: {index}");
        return MantlePieces[index].기본타일;
    }

    public int GetCollidedMantleIndex(Collider2D collider2D)
    {
        var totalSegments = MantlePieces.Count;
        var collisionPosition = collider2D.bounds.ClosestPoint(transform.position);
        var centerPosition = transform.position;
        var rotationZ = Mantle.eulerAngles.z;

        // 회전을 적용한 각도를 계산합니다.
        var angle = GetAngle(collisionPosition, centerPosition, rotationZ);

        // 각도에 따른 세그먼트 인덱스를 계산합니다.
        var index = GetSegmentIndex(angle, totalSegments);
        if (index < 0) {
            index = 0;
        }

        return index;
    }

    private float GetAngle(Vector3 collisionPosition, Vector3 centerPosition, float rotationZ)
    {
        var adjustedPosition = collisionPosition - centerPosition;
        // 회전된 상태에서의 각도를 구하기 위해 회전 값을 빼줍니다.
        var rotation = Quaternion.Euler(0, 0, -rotationZ);
        adjustedPosition = rotation * adjustedPosition;
        var angle = Mathf.Atan2(adjustedPosition.y, adjustedPosition.x) * Mathf.Rad2Deg;
        // 결과 각도가 0 이상 360 미만이 되도록 합니다.
        angle = (angle + 360) % 360;
        return angle;
    }

    private int GetSegmentIndex(float angle, int totalSegments)
    {
        var segmentSize = 360f / totalSegments;
        for (var i = 0; i < totalSegments; i++) {
            if (angle >= i * segmentSize && angle < (i + 1) * segmentSize) {
                return i;
            }
        }
        return -1;
    }

    public void ApplyShaderSettings()
    {
        if (PlanetSpriteRenderer == null) {
            PlanetSpriteRenderer = Mantle.GetComponentInChildren<SpriteRenderer>(true);
        }

        if (PlanetSpriteRenderer != null) {
            var mantlePiecesArray = MantlePieces.ConvertAll(x => (float)x.기본타일).ToArray();
            // 원본 머티리얼 복사
            var tempMaterial = new Material(PlanetSpriteRenderer.sharedMaterial);

            // 세그먼트 값 초기화
            tempMaterial.SetFloat("_Segments", mantlePiecesArray.Length);
            tempMaterial.SetFloatArray("_MantlePieces", mantlePiecesArray);
            tempMaterial.SetTexture("_ClipTex1", PlanetSprite.texture);

            // 수정된 머티리얼을 원본 머티리얼에 적용
            PlanetSpriteRenderer.sharedMaterial.CopyPropertiesFromMaterial(tempMaterial);

            // 임시 머티리얼 제거
            DestroyImmediate(tempMaterial);
        }
    }


    public void TriggerEvent(string eventId, int collidedMantlePiece)
    {
        StartCoroutine(StartEvent(eventId, collidedMantlePiece));
    }

    private IEnumerator StartEvent(string eventId, int collidedMantlePiece)
    {
        // 이벤트 ID에 따라 처리 수행
        switch (eventId) {
            case "Asteroid":
                var rand = RAND.RangeInt(0, 4) + 1;
                ResourceManager.LoadAssetAsync<Sprite>($"LavaPlanet{rand}", (sprite) => {
                    PlanetSprite = sprite;
                    for (var i = 0; i < MantlePieces.Count; i++) {
                        MantlePieces[i].기본타일 = 10;
                    }

                    var plants = Mantle.GetComponentsInChildren<Plant>();
                    for (var i = 0; i < plants.Length; i++) {
                        var p = plants[i];
                        p.ActionState(PlantState.Dead);
                    }

                    var planetObjects = Mantle.GetComponentsInChildren<PlanetObjects>();
                    for (var i = 0; i < plants.Length; i++) {
                        var p = planetObjects[i];
                        p.PlayAnim("Action_2");
                    }
                    ApplyShaderSettings();
                    CheckClear();
                });
                break;
            case "mountain":
                var planetObjects = Mantle.GetComponentsInChildren<PlanetObjects>();
                var plants = Mantle.GetComponentsInChildren<Plant>();
                var collidedMantle = MantlePieces[collidedMantlePiece];
                collidedMantle.기본타일 = collidedMantle.화산폭팔변경;
                for (var i = 0; i < plants.Length; i++) {
                    var plant = plants[i];
                    if (plant.MantlePiecesIndex == collidedMantlePiece) {
                        plant.CollidedTrigger(MantleCollider2D);
                    }
                }

                var leftindex = collidedMantlePiece - 1;
                var rightindex = collidedMantlePiece + 1;
                var leftlava = true;
                var rightlava = true;
                var s = new WaitForSeconds(1);
                ApplyShaderSettings();
                CheckCount();
                yield return s;

                var halfCount = (MantlePieces.Count / 2) - 1;
                var stepsTaken = 0;
                while (leftlava || rightlava) {
                    if (leftlava) {
                        if (leftindex < 0) {
                            leftindex = MantlePieces.Count - 1;
                        }

                        for (var i = 0; i < planetObjects.Length; i++) {
                            var planetObject = planetObjects[i];
                            if (planetObject.MantlePiecesIndex == leftindex) {
                                planetObject.TriggerEnter2D(MantleCollider2D, true);
                            }
                        }

                        for (var i = 0; i < plants.Length; i++) {
                            var plant = plants[i];
                            if (plant.MantlePiecesIndex == leftindex) {
                                plant.CollidedTrigger(MantleCollider2D);
                            }
                        }

                        var prevMantle = MantlePieces[leftindex];
                        prevMantle.기본타일 = prevMantle.화산폭팔변경;
                        leftindex--;

                        if (stepsTaken >= halfCount) {
                            leftlava = false;
                        }
                    }

                    if (rightlava) {
                        if (rightindex >= MantlePieces.Count) {
                            rightindex = 0;
                        }

                        var nextMantle = MantlePieces[rightindex];
                        nextMantle.기본타일 = nextMantle.화산폭팔변경;

                        for (var i = 0; i < planetObjects.Length; i++) {
                            var planetObject = planetObjects[i];
                            if (planetObject.MantlePiecesIndex == rightindex) {
                                planetObject.TriggerEnter2D(MantleCollider2D, true);
                            }
                        }

                        for (var i = 0; i < plants.Length; i++) {
                            var plant = plants[i];
                            if (plant.MantlePiecesIndex == rightindex) {
                                plant.CollidedTrigger(MantleCollider2D);
                            }
                        }

                        rightindex++;

                        if (stepsTaken >= halfCount) {
                            rightlava = false;
                        }
                    }

                    ApplyShaderSettings();
                    CheckCount();
                    yield return s;
                    stepsTaken++;
                }
                break;



            // 추가 이벤트를 처리하려면 여기에 case를 추가하세요.
            default:
                Debug.LogWarning("Unknown event ID: " + eventId);
                break;
        }

    }

}

[Serializable]
public class ClearConditions {
    public int ClearCount;
    public int Start3 = 5;
    public int Start2 = 7;
    public int Start1 = 10;

}

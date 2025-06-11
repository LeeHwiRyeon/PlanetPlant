using System.Collections.Generic;
using UnityEngine;
using Util;

public enum PlantState {
    Born, Idle, Dead,
}
public class Plant : MonoBehaviour {
    public bool IsStuck;
    private bool m_fire;
    public float speed;
    public Rigidbody2D Rigidbody2D;
    public Animator Animator;
    public Collider2D Collider2D;
    public SpriteRenderer Tree;
    public List<int> MantlePieces;
    public int MantlePiecesIndex;

    public PlantState PlantState { get; set; }
    private void Awake()
    {
        if (Rigidbody2D == null) {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        if (Collider2D == null) {
            Collider2D = GetComponent<Collider2D>();
        }
        Collider2D.enabled = false;

        if (Animator == null) {
            Animator = GetComponent<Animator>();
        }

        if (Tree == null) {
            Tree = GetComponentInChildren<SpriteRenderer>(true);
        }
    }

    public void Fire()
    {
        m_fire = true;
        Collider2D.enabled = true;
        Rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        if (m_fire == false) {
            return;
        }

        if (IsStuck == false) {
            var planet = PlanetManager.Inst.Planet;
            var pos = (Vector2)planet.transform.position + planet.BlackHole;
            var dir = (pos - Rigidbody2D.position).normalized;
            var limitedSpeed = Mathf.Min(speed * Time.fixedDeltaTime, (pos - Rigidbody2D.position).magnitude);
            Rigidbody2D.MovePosition(Rigidbody2D.position + (dir * limitedSpeed));
        }
    }

    public void ActionState(PlantState state)
    {
        PlantState = state;
        var animationName = string.Empty;
        switch (state) {
            case PlantState.Born:
                animationName = "born";
                break;
            case PlantState.Idle:
                animationName = "idle";
                break;
            case PlantState.Dead:
                animationName = "dead";
                break;
        }

        Animator.Play(animationName, 0, 0);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_fire == false) {
            return;
        }

        IsStuck = true;
        CollidedTrigger(collision);
    }

    public void CollidedTrigger(Collider2D collision)
    {
        var planet = PlanetManager.Inst.Planet;
        if (collision.CompareTag("Planet") && PlantState != PlantState.Dead) {
            transform.SetParent(planet.Mantle.transform);
            MantlePiecesIndex = planet.GetCollidedMantleIndex(Collider2D);
            var collidedMantlePiece = planet.GetCollidedMantlePiece(Collider2D);
            var isDead = true;
            if (PlantState != PlantState.Dead) {
                for (var i = 0; i < MantlePieces.Count; i++) {
                    var mantlePiece = MantlePieces[i];
                    if (mantlePiece == collidedMantlePiece) {
                        isDead = false;
                        break;
                    }
                }
            }

            ActionState(isDead ? PlantState.Dead : PlantState.Idle);
            PlayHitEffect(collidedMantlePiece);
            ApplySettings(planet);
        } else if (collision.CompareTag("Plant")) {
            ActionState(PlantState.Dead);
            MantlePiecesIndex = planet.GetCollidedMantleIndex(Collider2D);
            var collidedMantlePiece = planet.GetCollidedMantlePiece(Collider2D);
            PlayHitEffect(collidedMantlePiece);
            ApplySettings(planet);
        } else if (collision.CompareTag("Satellite")) {
            transform.SetParent(collision.transform);
            Collider2D.enabled = false;
            ActionState(PlantState.Dead);
            PlayHitEffect(0);
        } else {
            ActionState(PlantState.Dead);
            transform.SetParent(planet.Mantle.transform);
            MantlePiecesIndex = planet.GetCollidedMantleIndex(Collider2D);
            var collidedMantlePiece = planet.GetCollidedMantlePiece(Collider2D);
            PlayHitEffect(collidedMantlePiece);
            ApplySettings(planet);
        }

        planet.CheckCount();
    }


    private void PlayHitEffect(int collidedMantlePiece)
    {

        switch (collidedMantlePiece) {
            default: {
                    var index = RAND.RangeInt(0, 2);
                    SoundManager.Instance.PlayOneShot(index == 0 ? FixedResourceNames.stuckground_1 : FixedResourceNames.stuckground_2);
                    EffectSystem.Instance.Create(FixedResourceNames.Hit_Ground, Vector3.zero, new Vector3(0, 0, 180), Vector3.one, true, transform, gameObject.layer);
                }
                break;

            case 1: //물소리
            case 17: {
                    SoundManager.Instance.PlayOneShot(FixedResourceNames.watersplash_1);
                    EffectSystem.Instance.Create(FixedResourceNames.Hit_Water, Vector3.zero, new Vector3(0, 0, 180), Vector3.one, true, transform, gameObject.layer);
                }
                break;
            case 10: {
                    var index = RAND.RangeInt(0, 2);
                    SoundManager.Instance.PlayOneShot(index == 0 ? FixedResourceNames.stuckground_1 : FixedResourceNames.stuckground_2);
                    EffectSystem.Instance.Create(FixedResourceNames.Hit_Lava, Vector3.zero, new Vector3(0, 0, 180), Vector3.one, true, transform, gameObject.layer);
                }
                break;
        }
    }

    public void RandomDeadTree()
    {
        var index = RAND.RangeInt(0, 2);
        ResourceManager.Load<Sprite>(index == 0 ? FixedResourceNames.Plant_DeadTree1 : FixedResourceNames.Plant_DeadTree2, (sprite) => {
            Tree.sprite = sprite;
            Tree.transform.localScale = new Vector3(10, 10, 10);
        });
    }


    // 원 위의 좌표를 찾는 함수
    public Vector3 GetPointOnCircle(Vector3 center, float radius, float angleInDegrees)
    {
        var angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        var x = center.x + (radius * Mathf.Cos(angleInRadians));
        var y = center.y + (radius * Mathf.Sin(angleInRadians));
        return new Vector3(x, y, center.z);
    }

    // 원의 중심, 반지름, 각도를 사용하여 오브젝트를 원에 배치하는 함수
    public void PlaceObjectOnCircle(GameObject yourObject, Vector3 center, float radius, float angleInDegrees)
    {
        var pointOnCircle = GetPointOnCircle(center, radius, angleInDegrees);
        yourObject.transform.position = pointOnCircle;
    }

    public void ApplySettings(Planet target)
    {
        var go = target.Mantle.GetChild(0);
        // 1. 원점 (target.transform.position)을 기준으로 원 위에 있는 위치로 오브젝트를 이동시키십시오.
        Vector2 direction = (transform.position - target.transform.position).normalized;
        var newPosition = (Vector2)target.transform.position + (direction * (go.transform.localScale.x + 0.2f));
        transform.position = newPosition;

        // 2. 현재 오브젝트와 타겟 오브젝트 사이의 각도를 계산합니다.
        var angle = CalculateAngle(target.transform.position, transform.position);

        // 3. 계산된 각도를 사용하여 오브젝트를 Z축을 기준으로 회전시킵니다.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    public static float CalculateAngle(Vector2 pointA, Vector2 pointB)
    {
        var delta = pointB - pointA;
        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        return angle;
    }
}
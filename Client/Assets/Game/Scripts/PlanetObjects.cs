using UnityEngine;

public class PlanetObjects : MonoBehaviour {
    public string TypeName;
    public Collider2D Collider2D;
    public Animator Animator;
    public bool NotAutoSort;

    private float ElapsedTime;
    public float ComingTime;
    public Vector2 MeteorPolling;
    public Vector2 MeteorScaling;
    public int MantlePiecesIndex;

    private void Awake()
    {
        if (Collider2D == null) {
            Collider2D = GetComponent<Collider2D>();
        }

        if (Animator == null) {
            Animator = GetComponent<Animator>();
        }
    }

    public void Start()
    {
        MantlePiecesIndex = PlanetManager.Inst.Planet.GetCollidedMantleIndex(Collider2D);

    }

    public void ApplySettings(Planet target)
    {
        if (target == null || target.Mantle == null || NotAutoSort) {
            return;
        }

        var go = target.Mantle.GetChild(0);
        // 1. 원점 (target.transform.position)을 기준으로 원 위에 있는 위치로 오브젝트를 이동시키십시오.
        Vector2 direction = (transform.position - target.transform.position).normalized;
        var newPosition = (Vector2)target.transform.position + (direction * (go.transform.localScale.x + 0.2f));
        transform.position = newPosition;

        // 2. 현재 오브젝트와 타겟 오브젝트 사이의 각도를 계산합니다.
        var angle = CalculateAngle(target.transform.position, transform.position);

        // 3. 계산된 각도를 사용하여 오브젝트를 Z축을 기준으로 회전시킵니다.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    public static float CalculateAngle(Vector2 pointA, Vector2 pointB)
    {
        var delta = pointB - pointA;
        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnter2D(collision);
    }

    public void PlayAnim(string name)
    {
        Animator.Play(name, 0, 0);
    }

    public void TriggerEnter2D(Collider2D collision, bool forcedActive = false)
    {
        if (collision.CompareTag("Plant") || forcedActive) {
            var planet = PlanetManager.Inst.Planet;
            switch (TypeName) {
                case "mountain":
                    gameObject.PlayHitEffect(FixedResourceNames.Hit_Lava, FixedResourceNames.stuckground_1, transform);
                    Animator.Play("Action_2");
                    if (forcedActive == false) {
                        var collidedMantleIndex = planet.GetCollidedMantleIndex(Collider2D);
                        planet.TriggerEvent("mountain", collidedMantleIndex);
                    }
                    break;

                case "rock":
                    break;

                case "tornado":
                    break;

                case "meteor":
                    break;

            }
        }
    }
}



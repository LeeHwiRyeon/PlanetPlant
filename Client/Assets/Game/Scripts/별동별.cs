using UnityEngine;
using Util;

public class 별동별 : MonoBehaviour {
    public Collider2D Collider2D;
    public Animator Animator;
    public Rigidbody2D Rigidbody2D;
    public SpriteRenderer Sprite;
    private Vector2 StartPos;
    public string HitSound;

    public bool Asteroid;
    public float 대기시간 = 5f;
    public Vector2 Speed;
    public Vector2 MeteorScaling;
    public float speed;
    public float m_meteorScaling;
    public int percentage = 20;

    private void Awake()
    {
        if (Collider2D == null) {
            Collider2D = GetComponent<Collider2D>();
        }

        if (Animator == null) {
            Animator = GetComponent<Animator>();
        }

        StartPos = transform.position;
    }

    private void Start()
    {
        Init();
    }

    public float UpdateInterval = 1f;
    public float elapsedTime = 0f;
    public int 퍼센트;
    public bool Miss;
    public Vector2 dir;
    private bool 초기화 = false;
    private int rot;
    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;
        if (초기화 && 대기시간 < elapsedTime) {
            Init();
            초기화 = false;
            elapsedTime = 0f;
        } else if (퍼센트 < 100) {
            if (UpdateInterval > elapsedTime) {
                return;
            }

            elapsedTime = 0;
            퍼센트 += RAND.RangeInt(0, percentage);
            if (퍼센트 >= 100) {
                Animator.Play("Meteor2", 0, 0);
            }

            var x = m_meteorScaling * 퍼센트 * 0.01f;
            transform.localScale = new Vector3(x, x, x);
        } else if (초기화 == false && elapsedTime > 0.5f) {
            var planet = PlanetManager.Inst.Planet;
            var pos = dir * speed * Time.fixedDeltaTime;
            Rigidbody2D.MovePosition(Rigidbody2D.position + pos);
            rot += 1;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));

            if (Miss) {
                var dist = Vector3.Distance(transform.position, planet.transform.position);
                if (dist > 20) {
                    초기화 = true;
                }
                speed += Time.fixedDeltaTime;
            }
        }
    }


    public void ApplySettings(Planet target)
    {
        var go = target.Mantle.GetChild(0);
        // 1. 원점 (target.transform.position)을 기준으로 원 위에 있는 위치로 오브젝트를 이동시키십시오.
        Vector2 direction = (transform.position - target.transform.position).normalized;
        var newPosition = (Vector2)target.transform.position + (direction * (go.transform.localScale.x + 0.2f));
        transform.position = newPosition;

        var angle = CalculateAngle(target.transform.position, transform.position);
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
        if (collision.CompareTag("Planet")) {
            EffectSystem.Instance.Create(FixedResourceNames.Hit_Meteor, Vector3.zero, Vector3.zero, Vector3.one, true, transform, gameObject.layer);
            EffectSystem.Instance.Create(FixedResourceNames.Hit_Lava, Vector3.zero, Vector3.zero, Vector3.one, true, transform, gameObject.layer);
            Sprite.gameObject.SetActive(false);
            Collider2D.enabled = false;
            초기화 = true;
            SoundManager.Instance.PlayOneShot(HitSound);

            if (Asteroid) {
                enabled = false;
                var planet = PlanetManager.Inst.Planet;
                planet.TriggerEvent("Asteroid", 0);
            }
        }
    }

    private void Init()
    {
        퍼센트 = 0;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        elapsedTime = 0;
        transform.position = StartPos;
        Sprite.gameObject.SetActive(true);
        speed = Random.Range(Speed.x, Speed.y);
        m_meteorScaling = Random.Range(MeteorScaling.x, MeteorScaling.y);
        dir = (PlanetManager.Inst.Planet.transform.position - transform.position).normalized;
        Miss = Random.Range(0, 100) > 50;
        if (Miss) {
            Collider2D.enabled = false;
        } else {
            Collider2D.enabled = true;
        }
    }
}

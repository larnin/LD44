using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using DG.Tweening;
using NRand;

public class DollarControler : SerializedMonoBehaviour
{
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_minMoveTime = 1;
    [SerializeField] float m_maxMoveTime = 3;
    [SerializeField] float m_minFireTime = 1;
    [SerializeField] float m_maxFireTime = 3;
    [SerializeField] WeaponBase m_weapon = null;

    Animator m_animator;
    Rigidbody2D m_rigidbody;

    bool m_stopped = true;
    float m_timer = 0;

    Vector2 m_velocity = new Vector2(0, 0);
    Vector2 m_viewDirection = new Vector2();
    int m_tryCount = 0;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_timer = new UniformFloatDistribution(m_minFireTime, m_maxFireTime).Next(new StaticRandomGenerator<MT19937>());

        m_weapon.SetOwner(gameObject);
        m_weapon.SetPlayerWeapon(false);
        m_weapon.OnEquip();
    }

    void Update()
    {
        if (PlayerControler.Instance() == null)
            return;

        m_timer -= Time.deltaTime;

        if (m_stopped)
        {
            Vector2 target = PlayerControler.Instance().transform.position;
            Vector2 pos = transform.position;

            m_viewDirection = target - pos;
        }
        else m_viewDirection = m_velocity;

        if(m_timer <= 0)
        {
            if (m_stopped)
            {
                m_weapon.EndFire();
                m_timer = new UniformFloatDistribution(m_minMoveTime, m_maxMoveTime).Next(new StaticRandomGenerator<MT19937>());

                var dir = new UniformVector2CircleSurfaceDistribution(m_speed).Next(new StaticRandomGenerator<MT19937>());

                if (Physics2D.CircleCast(transform.position, 0.9f, dir, dir.magnitude, LayerMask.GetMask("Default")).collider == null || m_tryCount >= 10)
                {
                    m_velocity = dir;

                    m_animator.SetBool("Move", true);

                    m_stopped = false;
                    m_tryCount = 0;
                }
                else
                {
                    m_timer = 0;
                    m_tryCount++;
                }
            }
            else
            {
                Vector2 target = PlayerControler.Instance().transform.position;
                Vector2 pos = transform.position;

                m_viewDirection = target - pos;

                m_weapon.StartFire(m_viewDirection);

                m_velocity = new Vector2(0, 0);

                m_timer = new UniformFloatDistribution(m_minFireTime, m_maxFireTime).Next(new StaticRandomGenerator<MT19937>());

                m_animator.SetBool("Move", false);

                m_stopped = true;
            }
        }

        m_weapon.Process(m_viewDirection);

        m_rigidbody.velocity = m_velocity;

        float angle = Mathf.Atan2(m_viewDirection.y, m_viewDirection.x);
        m_animator.SetBool("Left", angle < -Mathf.PI / 2 || angle > Mathf.PI / 2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!m_stopped)
            m_timer = 0;
    }
}

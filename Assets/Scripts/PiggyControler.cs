using UnityEngine;
using System.Collections;
using DG.Tweening;
using NRand;

public class PiggyControler : MonoBehaviour
{
    [SerializeField] float m_minJumpDistance = 2;
    [SerializeField] float m_maxJumpDistance = 5;
    [SerializeField] float m_moveSpeed = 10;
    [SerializeField] float m_minIdleTime = 1;
    [SerializeField] float m_maxIdleTime = 5;
    [SerializeField] float m_startJumpDelay = 0.2f;

    Animator m_animator = null;
    Rigidbody2D m_rigidbody = null;

    float m_jumpTimer = 0;
    Vector2 m_velocity;
    bool m_jumping = false;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_jumpTimer = new UniformFloatDistribution(m_minIdleTime, m_maxIdleTime).Next(new StaticRandomGenerator<MT19937>());
    }

    void Update()
    {
        m_jumpTimer -= Time.deltaTime;

        if(m_jumpTimer < 0 && PlayerControler.Instance() != null && !m_jumping)
        {
            m_animator.SetTrigger("StartJump");

            m_jumping = true;

            Vector2 target = PlayerControler.Instance().transform.position;
            Vector2 pos = transform.position;

            var dist = new UniformFloatDistribution(m_minJumpDistance, m_maxJumpDistance).Next(new StaticRandomGenerator<MT19937>());

            var dir = (target - pos).normalized * dist;

            float jumpTime = dist / m_moveSpeed;

            DOVirtual.DelayedCall(m_startJumpDelay, () =>
            {
                m_velocity = dir / jumpTime;

                DOVirtual.DelayedCall(jumpTime, () =>
                {
                    m_velocity = new Vector2(0, 0);
                    m_jumpTimer = new UniformFloatDistribution(m_minIdleTime, m_maxIdleTime).Next(new StaticRandomGenerator<MT19937>());
                    m_jumping = false;

                    m_animator.SetTrigger("EndJump");
                });
            });
        }

        m_rigidbody.velocity = m_velocity;

        {
            Vector2 target = PlayerControler.Instance().transform.position;
            Vector2 pos = transform.position;

            float angle = Mathf.Atan2(target.y - pos.y, target.x - pos.x);

            m_animator.SetBool("Left", angle < -Mathf.PI / 2 || angle > Mathf.PI / 2);
        }
    }
}

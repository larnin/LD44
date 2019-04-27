using UnityEngine;
using System.Collections;

public class SimpleProjectile : BaseProjectile
{
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_maxLife = 1;

    Vector2 m_dir = new Vector2(0, 0);
    Rigidbody2D m_rigidbody = null;
    float m_life = 0;

    public override void SetDirection(Vector2 dir)
    {
        m_dir = dir.normalized;
    }

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        m_life += Time.deltaTime;

        if(m_life > m_maxLife)
        {
            //todo kill
            Destroy(gameObject);
        }

        m_rigidbody.velocity = m_dir * m_speed;
    }
}

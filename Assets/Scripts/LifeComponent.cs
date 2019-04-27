using UnityEngine;
using System.Collections;

public class LifeComponent : MonoBehaviour
{
    [SerializeField] float m_maxLife = 1;
    [SerializeField] float m_contactDamage = 1;
    float m_life = 0;

    private void Awake()
    {
        m_life = m_maxLife;
    }

    private void Start()
    {
        Event<EnemySpawnEvent>.Broadcast(new EnemySpawnEvent(gameObject));
    }

    public void Damage(float power)
    {
        m_life -= power;

        if (m_life <= 0)
        {
            OnKill();
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerControler>();
        if (player != null)
            player.Damage(m_contactDamage);
    }

    void OnKill()
    {
        Event<EnemyKillEvent>.Broadcast(new EnemyKillEvent(gameObject));
    }
}

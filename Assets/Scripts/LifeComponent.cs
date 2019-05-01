using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NRand;

public class LifeComponent : MonoBehaviour
{
    [Serializable]
    class LootInfo
    {
        public GameObject dropPrefab = null;
        public int countMin = 1;
        public int countMax = 1;
        public float weight = 1;
    }

    [SerializeField] List<LootInfo> m_loots = new List<LootInfo>();
    [SerializeField] int m_lootCount = 1;
    [SerializeField] float m_maxLife = 1;
    [SerializeField] float m_contactDamage = 1;
    [SerializeField] AudioClip m_deathSound = null;
    [SerializeField] bool m_isBoss = false;

    float m_life = 0;

    private void Awake()
    {
        m_life = m_maxLife;
    }

    private void Start()
    {
        Event<EnemySpawnEvent>.Broadcast(new EnemySpawnEvent(gameObject));

        if (m_isBoss)
            Event<BossLifeChangeEvent>.Broadcast(new BossLifeChangeEvent(m_life, m_maxLife));
    }

    public void Damage(float power)
    {
        m_life -= power;

        if(m_isBoss)
            Event<BossLifeChangeEvent>.Broadcast(new BossLifeChangeEvent(Mathf.Max(m_life, 0), m_maxLife));

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
        CreateLoot();
        SoundSystem.Instance().play(m_deathSound, 0.5f, true);
    }

    void CreateLoot()
    {
        if (m_loots.Count == 0 || m_lootCount <= 0)
            return;

        List<float> weights = new List<float>();
        for (int i = 0; i < m_loots.Count; i++)
            weights.Add(m_loots[i].weight);

        for (int i = 0; i < m_lootCount; i++)
        {
            int index = new DiscreteDistribution(weights).Next(new StaticRandomGenerator<MT19937>());
            int nb = new UniformIntDistribution(m_loots[index].countMin, m_loots[index].countMax + 1).Next(new StaticRandomGenerator<MT19937>());

            for (int j = 0; j < nb; j++)
            {
                var obj = Instantiate(m_loots[index].dropPrefab);
                obj.transform.position = transform.position + new Vector3(0, 0, 0.1f);
            }
        }
    }
}

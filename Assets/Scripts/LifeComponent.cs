﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NRand;
using DG.Tweening;

public class LifeComponent : MonoBehaviour
{
    const float fadeTime = 0.25f;

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
    [SerializeField] protected float m_maxLife = 1;
    [SerializeField] float m_contactDamage = 1;
    [SerializeField] AudioClip m_deathSound = null;
    [SerializeField] AudioClip m_hitSound = null;
    [SerializeField] protected GameObject m_deathObject = null;

    GameObject m_visual;

    Tweener m_currentTween = null;

    protected float m_life = 0;

    private void Awake()
    {
        m_life = m_maxLife;
        m_visual = transform.Find("Visual").gameObject;
    }

    protected virtual void Start()
    {
        Event<EnemySpawnEvent>.Broadcast(new EnemySpawnEvent(gameObject));
    }

    public virtual void Damage(float power)
    {
        m_life -= power;

        if (m_life <= 0)
        {
            OnKill();
            Destroy(gameObject);
        }
        else
        {
            SoundSystem.Instance().play(m_hitSound, 0.5f, true);
            var renderers = m_visual.GetComponentsInChildren<SpriteRenderer>();
            for(int i = 0; i < renderers.Length; i++)
            {
                if (m_currentTween != null && !m_currentTween.IsComplete())
                    m_currentTween.Complete(false);

                renderers[i].material.SetColor("_AdditiveColor", Color.white);
                m_currentTween = renderers[i].material.DOColor(Color.black, "_AdditiveColor", fadeTime);
            }
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

        CreateKillAnimation();
    }

    protected virtual void CreateKillAnimation()
    {
        if (m_deathObject != null)
        {
            var obj = Instantiate(m_deathObject);
            obj.transform.position = transform.position;
            Destroy(obj, 2);
        }
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

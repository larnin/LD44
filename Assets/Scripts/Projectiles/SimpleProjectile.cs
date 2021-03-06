﻿using UnityEngine;
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

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        m_rigidbody.rotation = angle;
    }

    private void Awake()
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

        float speedMultiplier = 1;
        if (isPlayerProjectile)
            speedMultiplier = PlayerStats.Instance().GetStatValue("ProjectileSpeedMultiplier");
        m_rigidbody.velocity = m_dir * m_speed * speedMultiplier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isPlayerProjectile)
        {
            float multiplier = PlayerStats.Instance().GetStatValue("DamageMultiplier");

            var life = collision.gameObject.GetComponent<LifeComponent>();
            if (life != null)
                life.Damage(m_baseDamage * multiplier);
        }
        else
        {
            var player = collision.gameObject.GetComponent<PlayerControler>();
            if (player != null)
                player.Damage(m_baseDamage);
        }

        Destroy(gameObject);
    }
}

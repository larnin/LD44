using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ContinuousWeapon : SimpleWeapon
{
    [SerializeField] float m_deltaTime = 0.2f;

    bool m_onFire = false;
    float m_fireTime = 0;

    public override void StartFire(Vector2 direction)
    {
        m_onFire = true;
    }

    public override void EndFire()
    {
        m_onFire = false;
    }

    public override void Process(Vector2 direction)
    {
        base.Process(direction);

        if(m_onFire && m_fireTime <= 0)
        {
            if (m_isPlayerWeapon)
                m_fireTime = m_deltaTime / PlayerStats.Instance().GetStatValue("FireRateMultiplier");
            else m_fireTime = m_deltaTime;

            Fire(direction);
        }

        m_fireTime -= Time.deltaTime;
    }
}

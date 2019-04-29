using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using NRand;

public class SimpleWeapon : WeaponBase
{
    [SerializeField] GameObject m_gunPrefab = null;
    [SerializeField] GameObject m_projectilePrefab = null;
    [SerializeField] Vector2 m_ballsStatOffset = new Vector2(0, 0);
    [SerializeField] List<AudioClip> m_bulletSounds = new List<AudioClip>();

    GameObject m_gun;
    GameObject m_gunFire;

    public override void OnEquip()
    {
        base.OnEquip();
        if (m_gunPrefab != null)
        {
            m_gun = GameObject.Instantiate(m_gunPrefab, m_owner.transform);
            m_gunFire = m_gun.transform.Find("Fire").gameObject;
            if(m_gunFire != null)
                m_gunFire.SetActive(false);
        }
    }

    public override void OnDesequip()
    {
        base.OnDesequip();
        if(m_gun != null)
            GameObject.Destroy(m_gun);
        m_gunFire = null;
    }

    public override void StartFire(Vector2 direction)
    {
        Fire(direction);
    }

    public override void EndFire()
    {

    }

    public override void Process(Vector2 direction)
    {
        if (m_gun == null)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float scale = 1;

        if (angle > 0 && angle < 180)
            m_gun.transform.localPosition = new Vector3(0, 0, 0.1f);
        else  m_gun.transform.localPosition = new Vector3(0, 0, -0.1f);

        if(angle < -90 || angle > 90)
        {
            angle += 180;
            scale = -1;
        }

        m_gun.transform.rotation = Quaternion.Euler(0, 0, angle);
        m_gun.transform.localScale = new Vector3(scale, 1, 1);
    }

    protected void Fire(Vector2 direction)
    {
        if (m_projectilePrefab == null)
            return;

        var projectileObj = GameObject.Instantiate(m_projectilePrefab);
        var projectile = projectileObj.GetComponent<BaseProjectile>();
        if (projectile != null)
        {
            projectile.isPlayerProjectile = m_isPlayerWeapon;
            projectile.SetDirection(direction);

            float angle = Mathf.Atan2(direction.y, direction.x);
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            Vector2 pos = m_owner.transform.position;
            var offset = m_ballsStatOffset;
            if (angle < -Mathf.PI / 2 || angle > Mathf.PI / 2)
                offset.y *= -1;
            pos += new Vector2(offset.x * cos - offset.y * sin, offset.x * sin + offset.y * cos);

            projectile.transform.position = pos;
        }

        if (m_isPlayerWeapon)
            PlayerStats.Instance().gold--;

        if (m_gunFire != null)
        {
            m_gunFire.SetActive(true);
            DOVirtual.DelayedCall(0.1f, () => 
            {
                if (this == null)
                    return;
                if(m_gunFire != null)
                    m_gunFire.SetActive(false);
            });
        }

        var index = new UniformIntDistribution(0, m_bulletSounds.Count).Next(new StaticRandomGenerator<MT19937>());
        SoundSystem.Instance().play(m_bulletSounds[index], 0.5f, true);
    }
}

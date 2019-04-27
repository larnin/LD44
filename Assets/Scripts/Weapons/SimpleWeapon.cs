using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleWeapon : WeaponBase
{
    [SerializeField] GameObject m_gunPrefab = null;
    [SerializeField] GameObject m_projectilePrefab = null;
    [SerializeField] Vector2 m_ballsStatOffset = new Vector2(0, 0);

    GameObject m_gun;

    public override void OnEquip()
    {
        base.OnEquip();
        if(m_gunPrefab != null)
            m_gun = GameObject.Instantiate(m_gunPrefab, m_owner.transform);
    }

    public override void OnDesequip()
    {
        base.OnDesequip();
        if(m_gun != null)
            GameObject.Destroy(m_gun);
    }

    public override void StartFire(Vector2 direction)
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
            pos += new Vector2(m_ballsStatOffset.x * cos - m_ballsStatOffset.y * sin, m_ballsStatOffset.x * sin + m_ballsStatOffset.y * cos);

            projectile.transform.position = pos;
        }

        if (m_isPlayerWeapon)
            PlayerStats.Instance().gold--;
    }

    public override void EndFire()
    {

    }

    public override void Process(Vector2 direction)
    {
        if (m_gun == null)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        m_gun.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (angle > 0 && angle < 180)
            m_gun.transform.localPosition = new Vector3(0, 0, 0.1f);
        else  m_gun.transform.localPosition = new Vector3(0, 0, -0.1f);

    }

}

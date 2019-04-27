using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class WeaponBase
{
    protected GameObject m_owner = null;
    protected bool m_isPlayerWeapon = true;
    [SerializeField] Dictionary<string, StatModifier> m_modifiers = new Dictionary<string, StatModifier>();

    public void SetOwner(GameObject owner)
    {
        m_owner = owner;
    }

    public void SetPlayerWeapon(bool isPlayerWeapon)
    {
        m_isPlayerWeapon = isPlayerWeapon;
    }

    public virtual void OnEquip()
    {
        if (m_isPlayerWeapon)
            foreach (var m in m_modifiers)
                PlayerStats.Instance().AddStatModifier(m.Key, m.Value);
    }

    public virtual void OnDesequip()
    {
        if (m_isPlayerWeapon)
            foreach (var m in m_modifiers)
                PlayerStats.Instance().RemoveStatModifier(m.Key, m.Value);
    }

    public abstract void StartFire(Vector2 direction);

    public abstract void EndFire();

    public abstract void Process(Vector2 direction);
}

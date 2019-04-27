using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class WeaponBase
{
    GameObject m_owner = null;

    public void SetOwner(GameObject owner)
    {
        m_owner = owner;
    }

    public abstract void OnEquip();

    public abstract void StartFire(Vector2 direction);

    public abstract void EndFire();

    public abstract void Process(Vector2 direction);

    public abstract void OnDesequip();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponPickedEvent : EventArgs
{
    public WeaponPickedEvent(WeaponBase _weapon, GameObject _drop)
    {
        weapon = _weapon;
        drop = _drop;
    }

    public WeaponBase weapon;
    public GameObject drop;
}

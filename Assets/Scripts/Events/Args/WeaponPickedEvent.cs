using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponPickedEvent : EventArgs
{
    public WeaponPickedEvent(WeaponBase _weapon, int _dropIndex)
    {
        weapon = _weapon;
        dropIndex = _dropIndex;
    }

    public WeaponBase weapon;
    public int dropIndex;
}

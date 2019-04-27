﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponTargetChangeEvent : EventArgs
{
    public WeaponTargetChangeEvent(Vector2 _target)
    {
        target = _target;
    }

    public Vector2 target;
}

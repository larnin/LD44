using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCameraEvent : EventArgs
{
    public MoveCameraEvent(Vector2 _target, float _duration)
    {
        target = _target;
        duration = _duration;
    }

    public Vector2 target;
    public float duration;
}
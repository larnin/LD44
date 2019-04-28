using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DoorUsedEvent : EventArgs
{
    public DoorUsedEvent(Vector2Int _direction)
    {
        direction = _direction;
    }
    public Vector2Int direction;
}
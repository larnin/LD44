using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UpdateMinimapEvent : EventArgs
{
    public UpdateMinimapEvent(List<MapSystem.RoomInstance> _rooms, Vector2Int _currentRoom)
    {
        rooms = _rooms;
        currentRoom = _currentRoom;
    }

    public List<MapSystem.RoomInstance> rooms;
    public Vector2Int currentRoom;
}

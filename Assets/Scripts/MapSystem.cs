using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using NRand;
using DG.Tweening;

public class MapSystem : MonoBehaviour
{
    public enum RoomType
    {
        DefaultRoom,
        StartRoom,
        ShopRoom,
        BossRoom
    }

    [Serializable]
    class RoomInfo
    {
        public GameObject roomPrefab = null;
        public RoomType type = RoomType.DefaultRoom;
    }

    public class RoomInstance
    {
        public GameObject room = null;
        public bool discovered = false;
        public int x = 0;
        public int y = 0;
        public RoomType type = RoomType.DefaultRoom;
        public bool upDoor = false;
        public bool downDoor = false;
        public bool leftDoor = false;
        public bool rightDoor = false;
    }

    class RoomGeneration
    {
        public bool upDoor = false;
        public bool downDoor = false;
        public bool leftDoor = false;
        public bool rightDoor = false;
        public RoomType type = RoomType.DefaultRoom;
        public int x = 0;
        public int y = 0;
    }
    
    [SerializeField] int m_roomCount = 10;
    [SerializeField] int m_shopRoomCount = 2;
    [SerializeField] Vector2 m_roomSize = new Vector2(20, 15);
    [SerializeField] List<RoomInfo> m_roomPrefabs = new List<RoomInfo>();
    [SerializeField] GameObject m_topDoor = null;
    [SerializeField] GameObject m_downDoor = null;
    [SerializeField] GameObject m_leftDoor = null;
    [SerializeField] GameObject m_rightDoor = null;
    [SerializeField] float m_cameraMoveSpeed = 1;
    [SerializeField] float m_playerDoorDistance = 1.0f;
    
    static MapSystem m_instance = null;

    List<RoomInstance> m_rooms = new List<RoomInstance>();

    Vector2Int m_currentRoom = new Vector2Int(0, 0);

    SubscriberList m_subscriberList = new SubscriberList();

    public static MapSystem GetInstance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;

        m_subscriberList.Add(new Event<DoorUsedEvent>.Subscriber(OnDoorUse));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Start()
    {
        List<RoomGeneration> rooms = new List<RoomGeneration>();
        List<Vector2Int> queueRooms = new List<Vector2Int>();

        RoomGeneration baseRoom = new RoomGeneration();
        baseRoom.type = RoomType.StartRoom;
        rooms.Add(baseRoom);
        queueRooms.Add(new Vector2Int(0, 0));

        while(rooms.Count < m_roomCount)
        {
            List<float> weights = new List<float>();
            for(int i = 0; i < queueRooms.Count; i++)
            {
                float weight = 0;
                RoomGeneration r = rooms.Find(x => { return x.x == queueRooms[i].x && x.y == queueRooms[i].y; });
                if (CanGo(queueRooms[i] + new Vector2Int(0, -1), rooms)) weight++;
                if (CanGo(queueRooms[i] + new Vector2Int(0, 1), rooms)) weight++;
                if (CanGo(queueRooms[i] + new Vector2Int(-1, 0), rooms)) weight++;
                if (CanGo(queueRooms[i] + new Vector2Int(1, 0), rooms)) weight++;

                weights.Add(weight);
            }

            {
                var index = new DiscreteDistribution(weights).Next(new StaticRandomGenerator<MT19937>());

                RoomGeneration r = rooms.Find(x => { return x.x == queueRooms[index].x && x.y == queueRooms[index].y; });
                int nbDoors = 0;
                if (CanGo(queueRooms[index] + new Vector2Int(0, -1), rooms)) nbDoors++;
                if (CanGo(queueRooms[index] + new Vector2Int(0, 1), rooms)) nbDoors++;
                if (CanGo(queueRooms[index] + new Vector2Int(-1, 0), rooms)) nbDoors++;
                if (CanGo(queueRooms[index] + new Vector2Int(1, 0), rooms)) nbDoors++;

                RoomGeneration nextRoom = new RoomGeneration();

                nbDoors = new UniformIntDistribution(0, nbDoors).Next(new StaticRandomGenerator<MT19937>()) + 1;
                Vector2Int pos = queueRooms[index];
                if (CanGo(queueRooms[index] + new Vector2Int(0, 1), rooms)) nbDoors--;
                if (nbDoors == 0)
                {
                    r.upDoor = true;
                    nextRoom.downDoor = true;
                    pos.y++;
                }
                else
                {
                    if (CanGo(queueRooms[index] + new Vector2Int(0, -1), rooms)) nbDoors--;
                    if (nbDoors == 0)
                    {
                        r.downDoor = true;
                        nextRoom.upDoor = true;
                        pos.y--;
                    }
                    else
                    {
                        if (CanGo(queueRooms[index] + new Vector2Int(-1, 0), rooms)) nbDoors--;
                        if (nbDoors == 0)
                        {
                            r.leftDoor = true;
                            nextRoom.rightDoor = true;
                            pos.x--;
                        }
                        else
                        {
                            if (CanGo(queueRooms[index] + new Vector2Int(1, 0), rooms)) nbDoors--;
                            if (nbDoors == 0)
                            {
                                r.rightDoor = true;
                                nextRoom.leftDoor = true;
                                pos.x++;
                            }
                        }
                    }
                }

                nextRoom.x = pos.x;
                nextRoom.y = pos.y;
                rooms.Add(nextRoom);
                queueRooms.Add(pos);
                if (r.upDoor && r.downDoor && r.leftDoor && r.rightDoor)
                    queueRooms.Remove(new Vector2Int(r.x, r.y));
            }
        }

        rooms[rooms.Count - 1].type = RoomType.BossRoom;

        for(int i = 0; i < m_shopRoomCount; i++)
        {
            int validRoomCount = m_roomCount - 2 - i;
            int index = new UniformIntDistribution(0, validRoomCount).Next(new StaticRandomGenerator<MT19937>()) + 1;

            for (int j = 0; j < rooms.Count; j++)
            {
                if (rooms[j].type == RoomType.DefaultRoom)
                    index--;
                if (index == 0)
                {
                    rooms[j].type = RoomType.ShopRoom;
                    break;
                }
            }
        }

        for (int i = 0; i < rooms.Count; i++)
            InstantiateRoom(rooms[i]);

        m_rooms[0].discovered = true;

        Event<UpdateMinimapEvent>.Broadcast(new UpdateMinimapEvent(m_rooms, m_currentRoom));
    }

    bool CanGo(Vector2Int to, List<RoomGeneration> rooms)
    {
        if (rooms.Find(x => { return x.x == to.x && x.y == to.y; }) != null)
            return false;

        return true;
    }

    void InstantiateRoom(RoomGeneration r)
    {
        int nbRooms = 0;
        for (int i = 0; i < m_roomPrefabs.Count; i++)
            if (m_roomPrefabs[i].type == r.type)
                nbRooms++;
        int index = new UniformIntDistribution(0, nbRooms).Next(new StaticRandomGenerator<MT19937>()) + 1;
        for (int i = 0; i < m_roomPrefabs.Count; i++)
            if (m_roomPrefabs[i].type == r.type)
            {
                index--;
                if(index == 0)
                {
                    InstantiateRoom(r, m_roomPrefabs[i]);
                    break;
                }
            }
    }

    void InstantiateRoom(RoomGeneration r, RoomInfo rInfo)
    {
        var room = Instantiate(rInfo.roomPrefab);
        room.transform.position = new Vector3(r.x * m_roomSize.x, r.y * m_roomSize.y, 1);
        RoomInstance instance = new RoomInstance();
        instance.room = room;
        instance.type = r.type;
        instance.x = r.x;
        instance.y = r.y;
        instance.upDoor = r.upDoor;
        instance.downDoor = r.downDoor;
        instance.leftDoor = r.leftDoor;
        instance.rightDoor = r.rightDoor;

        if(instance.upDoor)
        {
            var door = Instantiate(m_topDoor, room.transform);
            door.transform.localPosition = new Vector3(0, m_roomSize.y / 2, -1);
        }
        if (instance.downDoor)
        {
            var door = Instantiate(m_downDoor, room.transform);
            door.transform.localPosition = new Vector3(0, -m_roomSize.y / 2, -1);
        }
        if (instance.rightDoor)
        {
            var door = Instantiate(m_rightDoor, room.transform);
            door.transform.localPosition = new Vector3(m_roomSize.x / 2, 0, -1);
        }
        if (instance.leftDoor)
        {
            var door = Instantiate(m_leftDoor, room.transform);
            door.transform.localPosition = new Vector3(-m_roomSize.x / 2, 0, -1);
        }

        m_rooms.Add(instance);
    }

    void OnDoorUse(DoorUsedEvent e)
    {
        m_currentRoom += e.direction;

        var room = m_rooms.Find(x => { return x.x == m_currentRoom.x && x.y == m_currentRoom.y; });
        if(room != null)
        {
            if (!room.discovered)
            {
                var spawns = room.room.GetComponentsInChildren<SpawnPoint>();
                for (int i = 0; i < spawns.Length; i++)
                    spawns[i].Spawn();
            }

            room.discovered = true;

            Event<MoveCameraEvent>.Broadcast(new MoveCameraEvent(new Vector2(m_currentRoom.x * m_roomSize.x, m_currentRoom.y * m_roomSize.y), m_cameraMoveSpeed));

            Vector2 targetPos = m_currentRoom * m_roomSize - e.direction * new Vector2(m_roomSize.x / 2 - m_playerDoorDistance, m_roomSize.y / 2 - m_playerDoorDistance);
            if (e.direction.y < 0)
                targetPos.y -= 2;

            Event<TeleportPlayerEvent>.Broadcast(new TeleportPlayerEvent(targetPos));

            Event<UpdateMinimapEvent>.Broadcast(new UpdateMinimapEvent(m_rooms, m_currentRoom));
        }
    }
}

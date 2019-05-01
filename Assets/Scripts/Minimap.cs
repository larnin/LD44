using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class Minimap : SerializedMonoBehaviour
{
    [Serializable]
    class SpriteInfos
    {
        public Sprite fullRoom = null;
        public Sprite doorUp = null;
        public Sprite doorDown = null;
        public Sprite doorLeft = null;
        public Sprite doorRight = null;
        public Sprite icon = null;
    }

    [SerializeField] Dictionary<MapSystem.RoomType, SpriteInfos> m_sprites = new Dictionary<MapSystem.RoomType, SpriteInfos>();
    [SerializeField] Sprite m_selectorSprite = null;
    [SerializeField] GameObject m_spritePrefab = null;
    [SerializeField] Vector2 m_roomSize = new Vector2(1, 1);
    [SerializeField] Color m_hiddenColor = Color.white;

    SubscriberList m_subscriberList = new SubscriberList();
    List<GameObject> m_images = new List<GameObject>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<UpdateMinimapEvent>.Subscriber(OnMapUpdate));
        m_subscriberList.Add(new Event<HideMapEvent>.Subscriber(OnHideMap));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnMapUpdate(UpdateMinimapEvent e)
    {
        for (int i = 0; i < m_images.Count; i++)
            Destroy(m_images[i]);
        m_images.Clear();

        var bounds = GetBounds(e.rooms);

        for(int i = 0; i < e.rooms.Count; i++)
        {
            var r = e.rooms[i];

            bool discovered = r.discovered;
            bool neighborDiscovered = HaveNeighborDiscovered(r, e.rooms);

            if (!discovered && !neighborDiscovered)
                continue;

            DrawMapItem(r, bounds);
        }

        DrawSelectedRoom(e.currentRoom, bounds);
    }

    RectInt GetBounds(List<MapSystem.RoomInstance> rooms)
    {
        Vector2Int min = new Vector2Int(0, 0);
        Vector2Int max = new Vector2Int(0, 0);

        for (int i = 0; i < rooms.Count; i++)
        {
            min.x = Math.Min(min.x, rooms[i].x);
            min.y = Math.Min(min.y, rooms[i].y);
            max.x = Math.Max(max.x, rooms[i].x);
            max.y = Math.Max(max.y, rooms[i].y);
        }

        return new RectInt(min, max - min + new Vector2Int(1, 1));
    }

    bool HaveNeighborDiscovered(MapSystem.RoomInstance room, List<MapSystem.RoomInstance> rooms)
    {
        if (room.upDoor && IsDiscovered(new Vector2Int(room.x, room.y + 1), rooms))
            return true;
        if (room.downDoor && IsDiscovered(new Vector2Int(room.x, room.y - 1), rooms))
            return true;
        if (room.leftDoor && IsDiscovered(new Vector2Int(room.x - 1, room.y), rooms))
            return true;
        if (room.rightDoor && IsDiscovered(new Vector2Int(room.x + 1, room.y), rooms))
            return true;
        return false;
    }

    bool IsDiscovered(Vector2Int pos, List<MapSystem.RoomInstance> rooms)
    {
        var r = rooms.Find(x => { return x.x == pos.x && x.y == pos.y; });
        return r != null && r.discovered;
    }

    void DrawMapItem(MapSystem.RoomInstance room, RectInt bounds)
    {
        var spriteSet = m_sprites[room.type];

        Color undiscoveredColor = new Color(0.5f, 0.5f, 0.5f);

        Vector2 pos = new Vector2(0, 0);
        pos.x = -bounds.width + room.x - bounds.x;
        pos.y = room.y - bounds.y + 1;
        pos *= m_roomSize;

        var roomRect = spriteSet.fullRoom.textureRect;

        var imageObj = Instantiate(m_spritePrefab, transform);
        imageObj.transform.localPosition = pos;

        var image = imageObj.GetComponent<Image>();
        image.sprite = spriteSet.fullRoom;
        if (!room.discovered) image.color = undiscoveredColor;
        image.SetNativeSize();

        m_images.Add(imageObj);

        if(room.upDoor)
        {
            var doorObj = Instantiate(m_spritePrefab, imageObj.transform);

            Vector2 doorPos = new Vector2(0, (1 - spriteSet.doorUp.textureRect.height / roomRect.height) * m_roomSize.y / 2);
            doorObj.transform.localPosition = doorPos;
         
            var doorImage = doorObj.GetComponent<Image>();
            doorImage.sprite = spriteSet.doorUp;
            if (!room.discovered) doorImage.color = undiscoveredColor;
            doorImage.SetNativeSize();

            m_images.Add(doorObj);
        }

        if (room.downDoor)
        {
            var doorObj = Instantiate(m_spritePrefab, imageObj.transform);

            Vector2 doorPos = new Vector2(0, -(1 - spriteSet.doorDown.textureRect.height / roomRect.height) * m_roomSize.y / 2);
            doorObj.transform.localPosition = doorPos;

            var doorImage = doorObj.GetComponent<Image>();
            doorImage.sprite = spriteSet.doorDown;
            if (!room.discovered) doorImage.color = undiscoveredColor;
            doorImage.SetNativeSize();

            m_images.Add(doorObj);
        }

        if (room.leftDoor)
        {
            var doorObj = Instantiate(m_spritePrefab, imageObj.transform);

            Vector2 doorPos = new Vector2(-(1 - spriteSet.doorLeft.textureRect.width / roomRect.width) * m_roomSize.x / 2, 0);
            doorObj.transform.localPosition = doorPos;

            var doorImage = doorObj.GetComponent<Image>();
            doorImage.sprite = spriteSet.doorLeft;
            if (!room.discovered) doorImage.color = undiscoveredColor;
            doorImage.SetNativeSize();

            m_images.Add(doorObj);
        }

        if (room.rightDoor)
        {
            var doorObj = Instantiate(m_spritePrefab, imageObj.transform);

            Vector2 doorPos = new Vector2((1 - spriteSet.doorRight.textureRect.width / roomRect.width) * m_roomSize.x / 2, 0);
            doorObj.transform.localPosition = doorPos;

            var doorImage = doorObj.GetComponent<Image>();
            doorImage.sprite = spriteSet.doorLeft;
            if (!room.discovered) doorImage.color = undiscoveredColor;
            doorImage.SetNativeSize();

            m_images.Add(doorObj);
        }

        if(spriteSet.icon != null)
        {
            var icon = Instantiate(m_spritePrefab, imageObj.transform);
            
            icon.transform.localPosition = new Vector2(0, 0);

            var iconImage = icon.GetComponent<Image>();
            iconImage.sprite = spriteSet.icon;
            iconImage.SetNativeSize();

            m_images.Add(icon);
        }
    }

    void DrawSelectedRoom(Vector2 current, RectInt bounds)
    {
        Vector2 pos = new Vector2(0, 0);
        pos.x = -bounds.width + current.x - bounds.x;
        pos.y = current.y - bounds.y + 1;
        pos *= m_roomSize;

        var imageObj = Instantiate(m_spritePrefab, transform);
        imageObj.transform.localPosition = pos;

        var image = imageObj.GetComponent<Image>();
        image.sprite = m_selectorSprite;
        image.SetNativeSize();

        m_images.Add(imageObj);
    }

    void OnHideMap(HideMapEvent e)
    {
        Color c = e.hide ? m_hiddenColor : Color.white;

        for(int i = 0; i < m_images.Count; i++)
        {
            var img = m_images[i].GetComponent<Image>();
            if (img != null)
                img.color = c;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ObjectsList : MonoBehaviour
{
    [SerializeField] GameObject m_itemShowPrefab = null;
    [SerializeField] float m_itemtWidth = 64;
    [SerializeField] float m_itemHeight = 64;
    [SerializeField] float m_itemScale = 0.5f;
    [SerializeField] int m_itemNbWidth = 10;
    [SerializeField] Color m_imageColor = Color.white;

    SubscriberList m_subscriberList = new SubscriberList();

    static ObjectsList m_instance = null;

    int m_itemCount = 0;

    public static int GetItemCount()
    {
        if (m_instance == null)
            return 0;
        return m_instance.m_itemCount;
    }

    private void Awake()
    {
        m_subscriberList.Add(new Event<ObjectPickedEvent>.Subscriber(OnObjectPicked));
        m_subscriberList.Subscribe();

        m_instance = this;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnObjectPicked(ObjectPickedEvent e)
    {
        if (e.sprite != null)
        {
            var obj = Instantiate(m_itemShowPrefab, transform);
            Vector2Int index = new Vector2Int(m_itemCount % m_itemNbWidth, m_itemCount / m_itemNbWidth);
            obj.transform.localPosition = new Vector2(index.x * m_itemtWidth, index.y * m_itemHeight);
            var renderer = obj.GetComponentInChildren<Image>();
            if (renderer != null)
            {
                renderer.sprite = e.sprite;
                renderer.SetNativeSize();
                renderer.color = m_imageColor;
            }
            obj.transform.localScale = new Vector3(m_itemScale, m_itemScale, m_itemScale);

            m_itemCount++;
        }
        
        if(e.modifier != null)
            PlayerStats.Instance().AddStatModifier(e.modifierName, e.modifier);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerGoldUI : MonoBehaviour
{
    Text[] m_texts;
    SubscriberList m_subscriberList = new SubscriberList();

    void Awake()
    {
        m_subscriberList.Add(new Event<GoldChangedEvent>.Subscriber(OnGoldChange));
        m_subscriberList.Subscribe();

        m_texts = GetComponentsInChildren<Text>();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnGoldChange(GoldChangedEvent e)
    {
        for (int i = 0; i < m_texts.Length; i++)
            m_texts[i].text = e.value.ToString();
    }
}



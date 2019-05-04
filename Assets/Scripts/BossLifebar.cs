using UnityEngine;
using System.Collections;

public class BossLifebar : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    GameObject m_barObj = null;
    SpriteRenderer m_barSprite = null;
    float m_initialWidth = 1;
    Vector3 m_initialPos = new Vector3(0, 0, 0);

    private void Awake()
    {
        m_subscriberList.Add(new Event<BossLifeChangeEvent>.Subscriber(OnLifeChange));
        m_subscriberList.Subscribe();

        m_barObj = transform.Find("Bar").gameObject;
        m_barSprite = m_barObj.GetComponent<SpriteRenderer>();

        m_initialWidth = m_barSprite.size.x;
        m_initialPos = m_barObj.transform.localPosition;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnLifeChange(BossLifeChangeEvent e)
    {
        if(e.life <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        m_barSprite.size = new Vector2(m_initialWidth * e.life / e.maxLife, m_barSprite.size.y);
        m_barSprite.transform.localPosition = m_initialPos + new Vector3((-m_initialWidth + m_barSprite.size.x) / 2.0f, 0, 0);
    }
}

using UnityEngine;
using System.Collections;

public class CursorLogic : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<WeaponTargetChangeEvent>.Subscriber(OnCursorMove));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnCursorMove(WeaponTargetChangeEvent e)
    {
        transform.position = e.target;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeLogic : MonoBehaviour
{
    [SerializeField] float m_fadeTime = 1.0f;

    Image m_renderer;
    SubscriberList m_subscriberList = new SubscriberList();

    static FadeLogic m_instance = null;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(transform.parent.gameObject);

        m_subscriberList.Add(new Event<ShowLoadingScreenEvent>.Subscriber(onFade));
        m_subscriberList.Subscribe();
        m_renderer = GetComponent<Image>();

        m_renderer.color = new Color(0, 0, 0, 0);
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void onFade(ShowLoadingScreenEvent e)
    {
        if (e.start)
            m_renderer.DOColor(Color.black, m_fadeTime);
        else m_renderer.DOColor(new Color(0, 0, 0, 0), m_fadeTime);

    }
}
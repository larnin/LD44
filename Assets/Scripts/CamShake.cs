using UnityEngine;
using System.Collections;
using NRand;

public class CamShake : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    float m_intensity = 0;
    float m_time = 0;

    Transform m_camTransform;

    private void Awake()
    {
        m_subscriberList.Add(new Event<CamShakeEvent>.Subscriber(OnCamShake));
        m_subscriberList.Subscribe();

        m_camTransform = transform.Find("Cam");
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnCamShake(CamShakeEvent e)
    {
        m_intensity = e.intensity;
        m_time = e.duration;
    }

    private void Update()
    {
        if (m_time > 0)
        {
            var dir = new UniformVector2CircleDistribution(m_intensity).Next(new StaticRandomGenerator<MT19937>());
            m_camTransform.localPosition = dir;
        }
        else m_camTransform.localPosition = new Vector3(0, 0, 0);

        m_time -= Time.deltaTime;
    }
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraLogic : MonoBehaviour
{
    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<MoveCameraEvent>.Subscriber(OnCameraMove));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnCameraMove(MoveCameraEvent e)
    {
        transform.DOMove(new Vector3(e.target.x, e.target.y, transform.position.z), e.duration).SetEase(Ease.InOutSine);
    }
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DoorBoss : MonoBehaviour
{
    [SerializeField] AudioClip m_openSound = null;
    [SerializeField] AudioClip m_closeSound = null;

    SubscriberList m_subscriberList = new SubscriberList();

    Animator m_animator = null;
    GameObject m_light = null;
    bool m_doorState = true;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_light = transform.Find("Light").gameObject;

        m_subscriberList.Add(new Event<AllEnemyKilledEvent>.Subscriber(OnAllEnemyKill));
        m_subscriberList.Add(new Event<EnemySpawnEvent>.Subscriber(OnEnemySpawn));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Start()
    {
        m_doorState = true;
    }

    void OnAllEnemyKill(AllEnemyKilledEvent e)
    {
        if (m_doorState)
            return;

        m_doorState = true;

        m_animator.SetBool("Closed", false);
        DOVirtual.DelayedCall(0.3f, () =>
        {
            if (this != null)
                m_light.SetActive(true);
        });

        SoundSystem.Instance().play(m_openSound);
    }

    void OnEnemySpawn(EnemySpawnEvent e)
    {
        if (!m_doorState)
            return;

        m_doorState = false;

        m_animator.SetBool("Closed", true);
        m_light.SetActive(false);

        SoundSystem.Instance().play(m_closeSound);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_doorState)
            return;

        if (collision.GetComponent<PlayerControler>() == null)
            return;

        Event<VictoryEvent>.Broadcast(new VictoryEvent());
    }
}

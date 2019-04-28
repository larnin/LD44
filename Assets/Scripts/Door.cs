using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject m_closedDoor = null;
    [SerializeField] GameObject m_openDoor = null;
    [SerializeField] Vector2Int m_direction = new Vector2Int(0, 0);

    bool m_doorState = false; //closed but open on start

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
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
        OnAllEnemyKill(new AllEnemyKilledEvent());
    }

    void OnAllEnemyKill(AllEnemyKilledEvent e)
    {
        if (m_doorState)
            return;

        m_doorState = true;

        m_closedDoor.SetActive(false);
        m_openDoor.SetActive(true);
    }

    void OnEnemySpawn(EnemySpawnEvent e)
    {
        if (!m_doorState)
            return;

        m_doorState = false;

        m_closedDoor.SetActive(true);
        m_openDoor.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_doorState)
            return;

        Event<DoorUsedEvent>.Broadcast(new DoorUsedEvent(m_direction));
    }
}

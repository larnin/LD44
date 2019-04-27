using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AliveEnemyCounter : MonoBehaviour
{
    List<GameObject> m_livingEnemy = new List<GameObject>();

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<EnemySpawnEvent>.Subscriber(OnEnemySpawn));
        m_subscriberList.Add(new Event<EnemyKillEvent>.Subscriber(OnEnemyKill));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnEnemySpawn(EnemySpawnEvent e)
    {
        m_livingEnemy.Add(e.enemy);
    }

    void OnEnemyKill(EnemyKillEvent e)
    {
        m_livingEnemy.Remove(e.enemy);

        if (m_livingEnemy.Count == 0)
            Event<AllEnemyKilledEvent>.Broadcast(new AllEnemyKilledEvent());
    }
}

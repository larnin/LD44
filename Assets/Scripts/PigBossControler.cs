using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class PigBossControler : SerializedMonoBehaviour
{
    enum State
    {
        Awake,
        Fire,
        WaitBeforeSpawn,
        Spawning
    }

    [SerializeField] float m_awakeTime = 1.0f;
    [SerializeField] float m_speed = 1;
    [SerializeField] WeaponBase m_weapon = null;
    [SerializeField] float m_minMoveTime = 3.0f;
    [SerializeField] float m_maxMoveTime = 5.0f;
    [SerializeField] float m_spawningTime = 2.0f;
    [SerializeField] float m_minSPawnDelay = 0.4f;
    [SerializeField] float m_maxSPawnDelay = 0.5f;
    [SerializeField] float m_minSpawnDistance = 2.0f;
    [SerializeField] float m_maxSpawnDistance = 3.0f;
    [SerializeField] List<GameObject> m_entities = new List<GameObject>();

    Rigidbody2D m_rigodbody = null;
    Animator m_animator = null;

    float m_time = 0.0f;
    State m_state = State.Awake;
    Vector2 m_velocity = new Vector2(0, 0);

    void Start()
    {
        m_rigodbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_time = m_awakeTime;
    }
    
    void Update()
    {
        switch(m_state)
        {
            case State.Awake:
                if (m_time <= 0)
                    StartFire();
                break;
            case State.Fire:
                if (m_time <= 0)
                    StartSpawn();

                m_weapon.Process(m_velocity);


                break;
            case State.WaitBeforeSpawn:
                if (m_time <= 0)
                    RealyStartSpawning();
                break;
            case State.Spawning:

                if(m_time <= 0)
                    StartFire();
                break;
        }

        m_time -= Time.deltaTime;
        m_rigodbody.velocity = m_velocity;
    }

    void StartFire()
    {

    }

    void StartSpawn()
    {

    }

    void RealyStartSpawning()
    {

    }
}

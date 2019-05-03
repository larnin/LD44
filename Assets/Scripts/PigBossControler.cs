using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using NRand;
using DG.Tweening;

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
    [SerializeField] float m_spawnStartDelay = 1.0f;
    [SerializeField] float m_spawningTime = 2.0f;
    [SerializeField] float m_minSpawnDelay = 0.4f;
    [SerializeField] float m_maxSpawnDelay = 0.5f;
    [SerializeField] float m_minSpawnDistance = 2.0f;
    [SerializeField] float m_maxSpawnDistance = 3.0f;
    [SerializeField] int m_minSpawnNb = 1;
    [SerializeField] int m_maxSpawnNb = 2;
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

        m_weapon.SetOwner(gameObject);
        m_weapon.SetPlayerWeapon(false);
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

                UpdateVelocity();
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
        UpdateVelocity();

        m_weapon.OnEquip();
        m_weapon.StartFire(m_velocity);

        m_time = new UniformFloatDistribution(m_minMoveTime, m_maxMoveTime).Next(new StaticRandomGenerator<MT19937>());

        m_state = State.Fire;
    }

    void StartSpawn()
    {
        m_time = m_spawnStartDelay;
        m_velocity = new Vector2(0, 0);

        m_state = State.WaitBeforeSpawn;

        m_weapon.EndFire();
        m_weapon.OnDesequip();
    }

    void RealyStartSpawning()
    {
        m_time = m_spawningTime;

        int spawnNb = new UniformIntDistribution(m_minSpawnNb, m_maxSpawnNb + 1).Next(new StaticRandomGenerator<MT19937>());
        var pos = transform.position;

        for(int i = 0; i < spawnNb; i++)
        {

        }

        m_animator.SetTrigger("Spawn");

        m_state = State.Spawning;
    }

    void UpdateVelocity()
    {
        if(PlayerControler.Instance() == null)
        {
            m_velocity = new Vector2(0, 0);
            return;
        }

        var pos = transform.position;
        var targetPos = PlayerControler.Instance().transform.position;

        m_animator.SetBool("Left", targetPos.x < pos.x);

        m_velocity = (targetPos - pos).normalized * m_speed;
    }
}

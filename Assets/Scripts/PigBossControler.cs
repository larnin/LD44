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
    [SerializeField] float m_poweredSpeed = 1.5f;
    [SerializeField] WeaponBase m_weapon = null;
    [SerializeField] WeaponBase m_poweredWeapon = null;
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
    [SerializeField] float m_poweredSpawnMultiplier = 1;
    [SerializeField] List<GameObject> m_entities = new List<GameObject>();
    [SerializeField] GameObject m_spawnFx = null;
    [SerializeField] AudioClip m_spawnSound = null;

    Rigidbody2D m_rigodbody = null;
    Animator m_animator = null;

    float m_time = 0.0f;
    State m_state = State.Awake;
    Vector2 m_velocity = new Vector2(0, 0);

    WeaponBase m_currentWeapon = null;
    bool m_isPowered = false;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<BossLifeChangeEvent>.Subscriber(OnLifeUpdate));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Start()
    {
        m_rigodbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_time = m_awakeTime;

        m_weapon.SetOwner(gameObject);
        m_weapon.SetPlayerWeapon(false);

        m_poweredWeapon.SetOwner(gameObject);
        m_poweredWeapon.SetPlayerWeapon(false);

        m_currentWeapon = m_weapon;
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
                UpdateVelocity();
                m_currentWeapon.Process(m_velocity);

                if (m_time <= 0)
                    StartSpawn();
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

        m_currentWeapon = m_isPowered ? m_poweredWeapon : m_weapon;

        m_currentWeapon.OnEquip();
        m_currentWeapon.StartFire(m_velocity);

        m_time = new UniformFloatDistribution(m_minMoveTime, m_maxMoveTime).Next(new StaticRandomGenerator<MT19937>());

        m_state = State.Fire;
    }

    void StartSpawn()
    {
        m_time = m_spawnStartDelay;
        m_velocity = new Vector2(0, 0);

        m_state = State.WaitBeforeSpawn;

        m_currentWeapon.EndFire();
        m_currentWeapon.OnDesequip();
    }

    void RealyStartSpawning()
    {
        m_time = m_spawningTime;
        m_velocity = new Vector2(0, 0);

        var rand = new StaticRandomGenerator<MT19937>();
        float spawnMultiplier = m_isPowered ? m_poweredSpawnMultiplier : 1;
        int spawnNb = Mathf.RoundToInt(new UniformIntDistribution(m_minSpawnNb, m_maxSpawnNb + 1).Next(rand) * spawnMultiplier);
        if (spawnNb > 0)
        {
            var pos = transform.position;

            float angle = new UniformFloatDistribution(0, Mathf.PI * 2).Next(rand);
            float minAngle = Mathf.PI / spawnNb / 2;
            float maxAngle = minAngle * 4;

            int tryCount = 0;
            for (int i = 0; i < spawnNb; i++)
            {
                var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * new UniformFloatDistribution(m_minSpawnDistance, m_maxSpawnDistance).Next(rand);

                var ray = Physics2D.Raycast(transform.position, dir, dir.magnitude, LayerMask.GetMask("Default"));
                if(ray.collider != null)
                {
                    tryCount++;
                    if (tryCount < 10)
                        i--;
                    else tryCount = 0;
                }
                else
                {
                    tryCount = 0;
                    
                    DOVirtual.DelayedCall(new UniformFloatDistribution(m_minSpawnDelay, m_maxSpawnDelay).Next(rand), () =>
                    {
                        if (this == null)
                            return;

                        DOVirtual.DelayedCall(0.3f, () =>
                        {
                            if (this == null)
                                return;
                            var index = new UniformIntDistribution(0, m_entities.Count).Next(rand);
                            var obj = Instantiate(m_entities[index]);
                            obj.transform.position = pos + new Vector3(dir.x, dir.y);
                        });

                        if(m_spawnFx != null)
                        {
                            var fx = Instantiate(m_spawnFx);
                            fx.transform.position = pos + new Vector3(dir.x, dir.y, -0.1f);
                            Destroy(fx, 2);
                        }
                        SoundSystem.Instance().play(m_spawnSound, 0.5f, true);
                    });
                }

                angle += new UniformFloatDistribution(minAngle, maxAngle).Next(rand);
            }
        }

        m_animator.SetTrigger("Invoke");

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

        m_velocity = (targetPos - pos).normalized * (m_isPowered ? m_poweredSpeed : m_speed);
    }

    void OnLifeUpdate(BossLifeChangeEvent e)
    {
        m_isPowered = (e.life / e.maxLife) < 0.5f;
    }
}

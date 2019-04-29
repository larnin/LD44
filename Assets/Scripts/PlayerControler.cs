using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class PlayerControler : MonoBehaviour
{
    const string horizontalInputName = "Horizontal";
    const string verticalInputName = "Vertical";

    [SerializeField] float m_maxSpeed = 1.0f;
    [SerializeField] float m_acceleration = 1.0f;
    [SerializeField] float m_threshold = 0.1f;
    [SerializeField] float m_blinkTime = 10.0f;
    [SerializeField] Material m_mat = null;
    [SerializeField] GameObject m_goldGainPrefab = null;
    [SerializeField] AudioClip m_deathSound = null;
    [SerializeField] List<AudioClip> m_footSteps = new List<AudioClip>();
    [SerializeField] float m_stepDistance = 0.5f;

    Vector2 m_speed = new Vector2(0, 0);
    Vector2 m_input = new Vector2(0, 0);

    Vector2 m_oldSpeed = new Vector2(1, 0);

    Rigidbody2D m_rigidbody;
    Animator m_animator;

    SubscriberList m_subscriberList = new SubscriberList();

    float m_invincibleTime = 0;

    float m_totalDistance = 0;
    Vector2 m_oldPosition = new Vector2();

    static PlayerControler m_instance;

    public static PlayerControler Instance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_subscriberList.Add(new Event<TeleportPlayerEvent>.Subscriber(OnTeleport));
        m_subscriberList.Add(new Event<GoldChangedEvent>.Subscriber(OnGoldGain));
        m_subscriberList.Subscribe();

        m_instance = this;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_oldPosition = transform.position;
    }

    void Update()
    {
        m_input.x = Input.GetAxis(horizontalInputName);
        m_input.y = Input.GetAxis(verticalInputName);

        var magnitude = m_input.magnitude;
        if (magnitude > 1.0f)
            m_input /= magnitude;

        m_invincibleTime -= Time.deltaTime;
        if(m_invincibleTime > 0)
        {
            float colorValue = Mathf.Sin(m_invincibleTime * m_blinkTime) * 0.5f + 0.5f;
            var color = new Color(colorValue, colorValue, colorValue);
                m_mat.SetColor("_AdditiveColor", color);
        }
        else
        {
            m_mat.SetColor("_AdditiveColor", Color.black);
        }

        Vector2 pos = transform.position;
        m_totalDistance += (pos - m_oldPosition).magnitude;
        if(m_totalDistance > m_stepDistance)
        {
            m_totalDistance -= m_stepDistance;
            int index = new UniformIntDistribution(0, m_footSteps.Count).Next(new StaticRandomGenerator<MT19937>());
            SoundSystem.Instance().play(m_footSteps[index], 0.05f, true);
        }

        m_oldPosition = pos;
    }

    void FixedUpdate()
    {
        float speedMultiplier = PlayerStats.Instance().GetStatValue("SpeedMultiplier");

        m_speed = m_rigidbody.velocity;

        var targetSpeed = m_input * m_maxSpeed * speedMultiplier;
        if (targetSpeed.magnitude < m_threshold * m_maxSpeed * speedMultiplier)
            targetSpeed = new Vector2(0, 0);

        float acceleration = Time.deltaTime * m_acceleration * speedMultiplier;

        m_speed.x = CalculateSpeed(m_speed.x, targetSpeed.x, speedMultiplier);
        m_speed.y = CalculateSpeed(m_speed.y, targetSpeed.y, speedMultiplier);

        float speedMagnitude = m_speed.magnitude;
        if (speedMagnitude > m_maxSpeed * speedMultiplier)
            m_speed = m_speed / speedMagnitude * m_maxSpeed * speedMultiplier;

        m_rigidbody.velocity = m_speed;

        if (m_speed.sqrMagnitude > 0.01f)
            m_oldSpeed = m_speed;

        m_animator.SetBool("Move", m_speed.sqrMagnitude > 0.01f);
    }

    float CalculateSpeed(float speed, float targetSpeed, float multiplier)
    {
        float acceleration = Time.deltaTime * m_acceleration * multiplier;

        if (speed < targetSpeed)
            speed = Mathf.Min(speed + acceleration, targetSpeed);
        else speed = Mathf.Max(speed - acceleration, targetSpeed);

        return speed;
    }

    public Vector2 GetMoveDirection()
    {
        return m_oldSpeed / m_maxSpeed;
    }

    public void Damage(float power)
    {
        if (m_invincibleTime > 0)
            return;

        m_invincibleTime = PlayerStats.Instance().GetStatValue("RecoverTime");

        float multiplier = PlayerStats.Instance().GetStatValue("DamageTakenMultiplier");

        int value = Mathf.CeilToInt(power * multiplier);

        PlayerStats.Instance().gold -= value;
    }

    void OnTeleport(TeleportPlayerEvent e)
    {
        transform.position = new Vector3(e.target.x, e.target.y, transform.position.z);
        m_oldPosition = transform.position;
    }

    void OnGoldGain(GoldChangedEvent e)
    {
        if (m_goldGainPrefab != null)
        {
            var obj = Instantiate(m_goldGainPrefab);
            obj.transform.position = transform.position;

            var gain = obj.GetComponentInChildren<GoldGain>();
            if (gain != null)
                gain.SetValue(e.offset);
        }

        if (e.value <= 0)
            OnDeath();
    }

    void OnDeath()
    {

        SoundSystem.Instance().play(m_deathSound, 0.5f, true);
        Destroy(gameObject);
    }
}

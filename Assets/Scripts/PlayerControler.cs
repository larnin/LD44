using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    const string horizontalInputName = "Horizontal";
    const string verticalInputName = "Vertical";

    [SerializeField] float m_maxSpeed = 1.0f;
    [SerializeField] float m_acceleration = 1.0f;
    [SerializeField] float m_threshold = 0.1f;

    Vector2 m_speed = new Vector2(0, 0);
    Vector2 m_input = new Vector2(0, 0);

    Rigidbody2D m_rigidbody;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        m_input.x = Input.GetAxis(horizontalInputName);
        m_input.y = Input.GetAxis(verticalInputName);

        var magnitude = m_input.magnitude;
        if (magnitude > 1.0f)
            m_input /= magnitude;
    }

    void FixedUpdate()
    {
        m_speed = m_rigidbody.velocity;

        var targetSpeed = m_input * m_maxSpeed;
        if (targetSpeed.magnitude < m_threshold * m_maxSpeed)
            targetSpeed = new Vector2(0, 0);

        float acceleration = Time.deltaTime * m_acceleration;

        m_speed.x = CalculateSpeed(m_speed.x, targetSpeed.x);
        m_speed.y = CalculateSpeed(m_speed.y, targetSpeed.y);

        float speedMagnitude = m_speed.magnitude;
        if (speedMagnitude > m_maxSpeed)
            m_speed = m_speed / speedMagnitude * m_maxSpeed;

        m_rigidbody.velocity = m_speed;
    }

    float CalculateSpeed(float speed, float targetSpeed)
    {
        float acceleration = Time.deltaTime * m_acceleration;

        if (speed < targetSpeed)
            speed = Mathf.Min(speed + acceleration, targetSpeed);
        else speed = Mathf.Max(speed - acceleration, targetSpeed);

        return speed;
    }
}

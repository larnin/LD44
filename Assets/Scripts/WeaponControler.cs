using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class WeaponControler : SerializedMonoBehaviour
{
    const string joysticFireXAxis = "JoyFireX";
    const string joysticFireYAxis = "JoyFireY";
    const string mouseXAxis = "MouseX";
    const string mouseYAxis = "MouseY";
    const string fireButton = "Fire";
    
    [SerializeField] float m_cursorSpeed = 1;
    [SerializeField] float m_cursorMaxDistance = 1;
    [SerializeField] float m_threshold = 0.1f;
    [SerializeField] WeaponBase m_weapon = null;
    [SerializeField] GameObject m_weaponPrefab = null;

    Vector2 m_cursorPosition = new Vector2(0, 0);
    bool m_controlerWasCentredLastFrame = false;

    SubscriberList m_subscriberList = new SubscriberList();

    PlayerControler m_playerControler = null;
    Animator m_animator = null;

    private void Awake()
    {
        m_playerControler = GetComponent<PlayerControler>();
        m_animator = GetComponent<Animator>();

        m_subscriberList.Add(new Event<WeaponPickedEvent>.Subscriber(OnWeaponPickup));
        m_subscriberList.Subscribe();

        if (m_weapon != null)
        {
            m_weapon.SetOwner(gameObject);
            m_weapon.OnEquip();
            m_weapon.SetPlayerWeapon(true);
        }
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        UpdateMouseCursor();
        UpdateControlerCursor();

        Vector2 pos = transform.position;
        Event<WeaponTargetChangeEvent>.Broadcast(new WeaponTargetChangeEvent(m_cursorPosition + pos, m_cursorPosition.magnitude));

        var dir = m_cursorPosition;
        if (dir.sqrMagnitude < 0.01f && m_playerControler != null)
            dir = m_playerControler.GetMoveDirection();

        if (m_weapon != null)
        {
            

            if (Input.GetButtonDown(fireButton))
                m_weapon.StartFire(dir);
            m_weapon.Process(dir);
            if (Input.GetButtonUp(fireButton))
                m_weapon.EndFire();
        }

        float angle = Mathf.Atan2(dir.y, dir.x);
        m_animator.SetBool("Left", angle < -Mathf.PI / 2 || angle > Mathf.PI / 2);
    }

    void UpdateMouseCursor()
    {
        Vector2 offset = new Vector2(Input.GetAxisRaw(mouseXAxis), Input.GetAxisRaw(mouseYAxis));

        if (Mathf.Abs(offset.x) <= 0 || Mathf.Abs(offset.y) <= 0)
            return;

        offset *= m_cursorSpeed;

        m_cursorPosition += offset;

        float magnitude = m_cursorPosition.magnitude;
        if (magnitude > m_cursorMaxDistance)
            m_cursorPosition *= m_cursorMaxDistance / magnitude;
    }

    void UpdateControlerCursor()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw(joysticFireXAxis), -Input.GetAxisRaw(joysticFireYAxis));

        if (dir.sqrMagnitude < m_threshold * m_threshold)
        {
            if (m_controlerWasCentredLastFrame)
                return;
            m_controlerWasCentredLastFrame = true;
            dir = new Vector2(0, 0);
        }
        else m_controlerWasCentredLastFrame = false;

        m_cursorPosition = dir * m_cursorMaxDistance;
    }

    void OnWeaponPickup(WeaponPickedEvent e)
    {
        if(m_weapon != null)
        {
            m_weapon.OnDesequip();
            m_weapon.SetOwner(null);

            if(m_weaponPrefab != null)
            {
                var obj = Instantiate(m_weaponPrefab);
                obj.transform.position = transform.position;
            }
        }

        m_weapon = e.weapon;
        if(m_weapon != null)
        {
            m_weapon.SetOwner(gameObject);
            m_weapon.OnEquip();
            m_weapon.SetPlayerWeapon(true);
        }
    }
}

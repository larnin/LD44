using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    const string pauseButton = "Pause";

    [SerializeField] AudioClip m_cancelSound = null;
    [SerializeField] AudioClip m_openSound = null;

    SubscriberList m_subscriberList = new SubscriberList();

    GameObject m_menu = null;

    bool m_open = false;

    static PauseMenu m_instance = null;

    public static bool IsPaused()
    {
        if (m_instance == null)
            return false;
        return m_instance.m_open;
    }

    private void Awake()
    {
        m_subscriberList.Add(new Event<ButtonPressEvent>.Subscriber(OnMenuSelect));
        m_subscriberList.Subscribe();

        m_instance = this;
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Start()
    {
        m_menu = transform.Find("Pause").gameObject;
        m_menu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (m_open)
                SoundSystem.Instance().play(m_cancelSound, 0.5f, true);
            else SoundSystem.Instance().play(m_openSound, 0.5f, true);
            SwitchPauseState();
        }
    }

    void OnMenuSelect(ButtonPressEvent e)
    {
        if (e.index == 0)
            SwitchPauseState();
        else SceneSystem.changeScene("MainMenu");

        Time.timeScale = 1;
    }

    void SwitchPauseState()
    {
        if(!m_open)
        {
            Time.timeScale = 0;
            m_menu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            m_menu.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (m_open)
            DOVirtual.DelayedCall(0.01f, () =>
            {
                if (this != null)
                    m_open = false;
            });
        else m_open = true;
    }
}

using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    const string pauseButton = "Pause";

    [SerializeField] AudioClip m_cancelSound = null;
    [SerializeField] AudioClip m_openSound = null;

    SubscriberList m_subscriberList = new SubscriberList();

    GameObject m_menu = null;

    bool m_open = false;

    private void Awake()
    {
        m_subscriberList.Add(new Event<ButtonPressEvent>.Subscriber(OnMenuSelect));
        m_subscriberList.Subscribe();
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

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        m_open = !m_open;
    }
}

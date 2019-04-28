using UnityEngine;
using System.Collections;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] string m_sceneName = "";
    [SerializeField] string m_ludumURL = "";

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<ButtonPressEvent>.Subscriber(OnMenuSelect));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnMenuSelect(ButtonPressEvent e)
    {
        switch(e.index)
        {
            case 0: //play
                PlayerStats.Instance().Reset();
                SceneSystem.changeScene(m_sceneName);
                break;
            case 1: //ld page
                Application.OpenURL(m_ludumURL);
                break;
            case 2: //quit
                Application.Quit();
                break;
        }
    }
}

using UnityEngine;
using System.Collections;

public class DefeatMenu : MonoBehaviour
{
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
        if (e.index == 0)
            SceneSystem.changeScene("MainMenu");
        else Application.Quit();
    }
}

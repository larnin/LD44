using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DefeatAndVictory : MonoBehaviour
{
    [SerializeField] float m_stateDelay = 2;
    [SerializeField] AudioClip m_defeatSong = null;
    [SerializeField] AudioClip m_victorySong = null;

    SubscriberList m_subscriberList = new SubscriberList();

    bool m_started = false;

    private void Awake()
    {
        m_subscriberList.Add(new Event<GoldChangedEvent>.Subscriber(OnGoldChange));
        m_subscriberList.Add(new Event<VictoryEvent>.Subscriber(OnVictory));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnGoldChange(GoldChangedEvent e)
    {
        if (m_started)
            return;

        if(e.value <= 0)
        {
            m_started = true;
            SoundSystem.Instance().play(m_defeatSong);
            DOVirtual.DelayedCall(m_stateDelay, () => { SceneSystem.changeScene("DefeatMenu"); });
        }
    }

    void OnVictory(VictoryEvent e)
    {
        if (m_started)
            return;

        m_started = true;
        SoundSystem.Instance().play(m_victorySong);
        DOVirtual.DelayedCall(m_stateDelay, () => { SceneSystem.changeScene("WinMenu"); });
    }
}

using UnityEngine;
using System.Collections;

public class BossMusicSwitcher : MonoBehaviour
{
    [SerializeField] AudioClip m_enableMusic = null;
    [SerializeField] AudioClip m_disableMusic = null;

    bool m_started = false;

    private void Awake()
    {
        m_started = true;
    }

    private void OnEnable()
    {
        if(m_started)
            SoundSystem.Instance().PlayMusic(m_enableMusic);
    }

    private void OnDisable()
    {
        if(m_started)
            SoundSystem.Instance().PlayMusic(m_disableMusic);
    }
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SoundSystem : MonoBehaviour
{
    static SoundSystem m_instance;
    public static SoundSystem Instance()
    {
        return m_instance;
    }

    AudioSource[] m_sources;
    AudioSource m_musicSource1;
    AudioSource m_musicSource2;
    bool m_currentSource1 = true;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_sources = transform.Find("Sounds").GetComponentsInChildren<AudioSource>();
        m_musicSource1 = transform.Find("Music1").GetComponent<AudioSource>();
        m_musicSource2 = transform.Find("Music2").GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, float volume = 0.5f, float transitionTime = 1)
    {
        if(m_currentSource1)
        {
            m_musicSource1.DOFade(0, transitionTime);
            m_musicSource2.clip = clip;
            m_musicSource2.volume = 0;
            m_musicSource2.Play();
            m_musicSource2.DOFade(volume, transitionTime);
        }
    }

    public void play(AudioClip clip, float volume = 0.5f, bool force = false)
    {
        bool canBeAdded = true;
        if(!force)
        {
            foreach (var s in m_sources)
            {
                if (s.isPlaying && clip == s.clip)
                {
                    canBeAdded = false;
                    break;
                }
            }
        }

        if (!canBeAdded)
            return;

        foreach (var s in m_sources)
        {
            if (!s.isPlaying)
            {
                s.clip = clip;
                s.volume = volume;
                s.Play();
                return;
            }
        }
    }
}
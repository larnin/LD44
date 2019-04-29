using UnityEngine;
using System.Collections;

public class MusicPlayerOnEnable : MonoBehaviour
{
    [SerializeField] AudioClip m_clip = null;
    
    void Start()
    {
        if (m_clip != null)
            SoundSystem.Instance().PlayMusic(m_clip);
    }
}

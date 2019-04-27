using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class DefaultStats : SerializedMonoBehaviour
{
    [SerializeField] Dictionary<string, StatModifier> m_stats = new Dictionary<string, StatModifier>();
    [SerializeField] bool m_debugStats;
    
    private void Awake()
    {
        foreach(var s in m_stats)
        {
            PlayerStats.Instance().AddStatModifier(s.Key, s.Value);
        }
    }

    private void OnDestroy()
    {
        foreach (var s in m_stats)
        {
            PlayerStats.Instance().RemoveStatModifier(s.Key, s.Value);
        }
    }

    private void OnGUI()
    {
        if (!m_debugStats)
            return;

        GUI.Label(new Rect(10, 10, 200, 30), "Gold " + PlayerStats.Instance().gold.ToString());

        GUI.Label(new Rect(10, 40, 200, 30), "Stats");

        var keys = PlayerStats.Instance().GetStatNames();
        for(int i = 0; i < keys.Length; i++)
        {
            GUI.Label(new Rect(25, 60 + i * 20, 200, 20), keys[i] + " " + PlayerStats.Instance().GetStatValue(keys[i]));
        }
    }
}

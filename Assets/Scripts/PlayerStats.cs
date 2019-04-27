using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStats
{
    static PlayerStats m_instance;

    int m_gold;

    Dictionary<string, StatValue> m_stats = new Dictionary<string, StatValue>();

    public static PlayerStats Instance()
    {
        if (m_instance == null)
            m_instance = new PlayerStats();
        return m_instance;
    }

    public int gold
    {
        get { return m_gold; }
        set
        {
            if (m_gold == value)
                return;
            m_gold = value;

            Event<GoldChangedEvent>.Broadcast(new GoldChangedEvent(m_gold));
        }
    }

    public float GetStatValue(string name, float defaultValue = 0)
    {
        if (!m_stats.ContainsKey(name))
            return defaultValue;
        return m_stats[name].GetValue();
    }

    public void AddStatModifier(string name, StatModifier modifier)
    {
        if (!m_stats.ContainsKey(name))
            m_stats.Add(name, new StatValue());
        m_stats[name].AddModifier(modifier);
    }

    public void RemoveStatModifier(string name, StatModifier modifier)
    {
        if (!m_stats.ContainsKey(name))
            return;
        m_stats[name].RemoveModifier(modifier);
    }

    public void ResetStat(string name)
    {
        if (!m_stats.ContainsKey(name))
            return;
        m_stats[name].RemoveAllModifiers();
    }

    public string[] GetStatNames()
    {
        return m_stats.Keys.ToArray();
    }

    public void Reset()
    {
        m_gold = 0;
        m_stats.Clear();
    }
}
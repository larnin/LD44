using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StatValue
{
    List<StatModifier> m_modifiers = new List<StatModifier>(); 

    public void AddModifier(StatModifier modifier)
    {
        if (m_modifiers.IndexOf(modifier) < 0)
            m_modifiers.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        m_modifiers.Remove(modifier);
    }

    public void RemoveAllModifiers()
    {
        m_modifiers.Clear();
    }

    public float GetValue()
    {
        float value = 0;
        for (int i = 0; i < m_modifiers.Count(); i++)
            value += m_modifiers[i].baseValue;
   
        for (int i = 0; i < m_modifiers.Count(); i++)
            value *= m_modifiers[i].multiplier;

        return value;
    }
}

[Serializable]
public class StatModifier
{
    public float baseValue = 0;
    public float multiplier = 1;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStats
{
    static PlayerStats m_instance;

    int m_gold;

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

    public void Reset()
    {
        m_gold = 0;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GoldChangedEvent : EventArgs
{
    public GoldChangedEvent(int _value, int _offset)
    {
        value = _value;
        offset = _offset;
    }

    public int value;
    public int offset;
}

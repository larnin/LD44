using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HideMapEvent : EventArgs
{
    public HideMapEvent(bool _hide)
    {
        hide = _hide;
    }

    public bool hide;
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WeaponPickedEvent : EventArgs
{
    public WeaponPickedEvent(WeaponBase _weapon)
    {
        weapon = _weapon;
    }

    public WeaponBase weapon;
}

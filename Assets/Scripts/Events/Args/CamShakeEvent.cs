using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CamShakeEvent : EventArgs
{
    public CamShakeEvent(float _intensity, float _duration)
    {
        duration = _duration;
        intensity = _intensity;
    }

    public float intensity;
    public float duration;
}

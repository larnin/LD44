using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BossLifeChangeEvent : EventArgs
{
    public BossLifeChangeEvent(float _life, float _maxLife)
    {
        life = _life;
        maxLife = _maxLife;
    }
    public float life;
    public float maxLife;
}

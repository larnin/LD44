using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawnEvent : EventArgs
{
    public EnemySpawnEvent(GameObject _enemy)
    {
        enemy = _enemy;
    }

    public GameObject enemy;
}

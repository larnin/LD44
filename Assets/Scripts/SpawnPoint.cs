using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] List<GameObject> m_prefabs = new List<GameObject>();

    public void Spawn()
    {
        int index = new UniformIntDistribution(0, m_prefabs.Count).Next(new StaticRandomGenerator<MT19937>());

        var obj = Instantiate(m_prefabs[index]);
        obj.transform.position = transform.position;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NRand;

public class ShopItemGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> m_items = new List<GameObject>();
    
    void Start()
    {
        if (m_items.Count == 0)
            return;

        var index = new UniformIntDistribution(0, m_items.Count).Next(new StaticRandomGenerator<MT19937>());
        var obj = Instantiate(m_items[index]);
        obj.transform.position = transform.position;
    }
}

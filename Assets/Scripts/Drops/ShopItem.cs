using UnityEngine;
using System.Collections;

public class ShopItem : DropItem
{
    [SerializeField] GameObject m_dropItem = null;
    [SerializeField] int m_price = 1;
    
    public override void ApplyLoot(GameObject player)
    {
        var obj = Instantiate(m_dropItem);
        obj.transform.position = transform.position;

        PlayerStats.Instance().gold -= m_price;
    }
}

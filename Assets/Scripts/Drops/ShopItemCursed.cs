using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShopItemCursed : DropItem
{
    [SerializeField] int m_itemIndex = 0;
    [SerializeField] int m_price = 1;

    protected override void Start()
    {
        base.Start();

        var price = m_tooltip.transform.Find("popup sprite_01/Price");
        var priceBack = m_tooltip.transform.Find("popup sprite_01/PriceBack");

        price.GetComponent<TextMesh>().text = m_price.ToString();
        priceBack.GetComponent<TextMesh>().text = m_price.ToString();
    }

    public override void ApplyLoot(GameObject player)
    {
        base.ApplyLoot(player);

        DropBank.Instance().Spawn(m_itemIndex, transform.position);

        PlayerStats.Instance().gold -= m_price;
    }
}

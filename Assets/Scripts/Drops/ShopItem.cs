using UnityEngine;
using System.Collections;

public class ShopItem : DropItem
{
    [SerializeField] GameObject m_dropItem = null;
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

        var obj = Instantiate(m_dropItem);
        obj.transform.position = transform.position;

        PlayerStats.Instance().gold -= m_price;
    }
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShopItem : DropItem
{
    [SerializeField] GameObject m_dropItem = null;
    [SerializeField] int m_price = 1;
    [SerializeField] float m_priceObjectMultiplier = 1.0f;

    int m_currentPrice = 1;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<ObjectPickedEvent>.Subscriber(OnObjectPicked));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    protected override void Start()
    {
        base.Start();

        UpdatePrice();
    }

    public override void ApplyLoot(GameObject player)
    {
        base.ApplyLoot(player);

        var obj = Instantiate(m_dropItem);
        obj.transform.position = transform.position;

        PlayerStats.Instance().gold -= m_price;
    }

    void UpdatePrice()
    {
        m_currentPrice = Mathf.RoundToInt(m_price * (1 + ObjectsList.GetItemCount() * (m_priceObjectMultiplier - 1)));

        var price = m_tooltip.transform.Find("popup sprite_01/Price");
        var priceBack = m_tooltip.transform.Find("popup sprite_01/PriceBack");

        price.GetComponent<TextMesh>().text = m_currentPrice.ToString();
        priceBack.GetComponent<TextMesh>().text = m_currentPrice.ToString();
    }

    void OnObjectPicked(ObjectPickedEvent e)
    {
        DOVirtual.DelayedCall(0.01f, () =>
        {
            if (this == null)
                return;
            UpdatePrice();
        });
    }
}

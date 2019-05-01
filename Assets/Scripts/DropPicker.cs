using UnityEngine;
using System.Collections;

public class DropPicker : MonoBehaviour
{
    const string pickupButton = "Pickup";

    [SerializeField] float m_lootDistance = 1;
    [SerializeField] float m_autoLootDistance = 1;

    DropItem m_currentLoot = null;
    
    void Update()
    {
        UpdateAutoLoot();
        UpdateLoot();
    }

    void UpdateAutoLoot()
    {
        Vector2 pos = transform.position;
        var colliders = Physics2D.OverlapCircleAll(pos, m_autoLootDistance * PlayerStats.Instance().GetStatValue("LootDistanceMultiplier"));

        for (int i = 0; i < colliders.Length; i++)
        {
            var item = colliders[i].GetComponent<DropItem>();
            if(item != null && item.autoloot)
                PickubLoot(item);
        }
    }

    void UpdateLoot()
    {
        Vector2 pos = transform.position;
        var colliders = Physics2D.OverlapCircleAll(pos, m_lootDistance * PlayerStats.Instance().GetStatValue("LootDistanceMultiplier"));

        DropItem bestItem = null;
        float bestDistance = float.MaxValue;
        for (int i = 0; i < colliders.Length; i++)
        {
            var item = colliders[i].GetComponent<DropItem>();
            if (item == null || item.autoloot)
                continue;

            Vector2 itemPos = item.transform.position;
            float distance = (itemPos - pos).sqrMagnitude;
            if(distance < bestDistance)
            {
                bestDistance = distance;
                bestItem = item;
            }
        }

        float currentItemDist = float.MaxValue;
        if (m_currentLoot != null)
        {
            Vector2 currentItemPos = m_currentLoot.transform.position;
            currentItemDist = (currentItemPos - pos).sqrMagnitude;
            if(currentItemDist > m_lootDistance)
            {
                m_currentLoot.HideTooltip();
                m_currentLoot = null;
                currentItemDist = float.MaxValue;
            }
        }

        if(bestDistance < currentItemDist)
        {
            if(m_currentLoot != null)
            {
                m_currentLoot.HideTooltip();
                m_currentLoot = null;
            }

            m_currentLoot = bestItem;
            m_currentLoot.ShowTooltip();
        }

        if(Input.GetButtonDown(pickupButton) && m_currentLoot != null)
        {
            m_currentLoot.HideTooltip();
            PickubLoot(m_currentLoot);
            m_currentLoot = null;
        }
    }

    void PickubLoot(DropItem item)
    {
        if (item == null)
            return;

        item.ApplyLoot(gameObject);

        Destroy(item.gameObject);
    }
}

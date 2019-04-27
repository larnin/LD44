using UnityEngine;
using System.Collections;

public class DropItemGold : DropItem
{
    [SerializeField] int value = 0;
    [SerializeField] GameObject m_feedbackPrefab = null;

    public override void ApplyLoot(GameObject player)
    {
        PlayerStats.Instance().gold += value;

        if(m_feedbackPrefab)
        {
            Instantiate(m_feedbackPrefab, player.transform);
        }
    }
}

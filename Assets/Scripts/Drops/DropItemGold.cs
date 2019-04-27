using UnityEngine;
using System.Collections;
using NRand;

public class DropItemGold : DropItem
{
    [SerializeField] int value = 0;
    [SerializeField] GameObject m_feedbackPrefab = null;

    public override void ApplyLoot(GameObject player)
    {
        float goldStat = PlayerStats.Instance().GetStatValue("GoldMultiplier");
        float finalGold = value * goldStat;
        float fPart = finalGold - Mathf.Floor(finalGold);
        if (new UniformFloatDistribution(0, 1).Next(new StaticRandomGenerator<MT19937>()) < fPart)
            finalGold++;

        PlayerStats.Instance().gold += Mathf.FloorToInt(finalGold);

        if(m_feedbackPrefab)
        {
            Instantiate(m_feedbackPrefab, player.transform);
        }
    }
}

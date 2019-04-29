using UnityEngine;
using System.Collections;

public class DropItemWeapon : DropItem
{
    [SerializeField] WeaponBase m_weapon = null;
    [SerializeField] int m_dropIndex = 0;

    public override void ApplyLoot(GameObject player)
    {
        base.ApplyLoot(player);

        Event<WeaponPickedEvent>.Broadcast(new WeaponPickedEvent(m_weapon, m_dropIndex));
    }
}

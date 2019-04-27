using UnityEngine;
using System.Collections;

public class DropItemWeapon : DropItem
{
    [SerializeField] WeaponBase m_weapon;

    public override void ApplyLoot(GameObject player)
    {
        Event<WeaponPickedEvent>.Broadcast(new WeaponPickedEvent(m_weapon));
    }
}

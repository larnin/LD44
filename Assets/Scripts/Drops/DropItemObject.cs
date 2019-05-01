using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DropItemObject : DropItem
{
    [SerializeField] Dictionary<string, StatModifier> m_modifiers = new Dictionary<string, StatModifier>();
    [SerializeField] Sprite m_sprite = null;

    public override void ApplyLoot(GameObject player)
    {
        base.ApplyLoot(player);

        Event<ObjectPickedEvent>.Broadcast(new ObjectPickedEvent("", null, m_sprite));

        foreach(var m in m_modifiers)
            Event<ObjectPickedEvent>.Broadcast(new ObjectPickedEvent(m.Key, m.Value, null));
    }
}

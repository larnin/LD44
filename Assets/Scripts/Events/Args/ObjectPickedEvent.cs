using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPickedEvent : EventArgs
{
    public ObjectPickedEvent(string _modifierName, StatModifier _modifier, Sprite _sprite)
    {
        modifierName = _modifierName;
        modifier = _modifier;
        sprite = _sprite;
    }

    public string modifierName;
    public StatModifier modifier;
    public Sprite sprite;
}

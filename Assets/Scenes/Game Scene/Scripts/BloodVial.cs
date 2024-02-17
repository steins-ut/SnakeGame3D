using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BloodVialItem))]
public class BloodVial : InteractableComponent
{
    public override InteractableType Type => InteractableType.ITEM;

    public override bool AcceptItem(ItemComponent item)
    {
        if(item == GetCachedComponent())
        {
            return true;
        }
        return false;
    }

    public override void ApplyEffect(EffectType effect, int level)
    {
    }

    public override void RemoveEffect(EffectType effect)
    {   
    }
}

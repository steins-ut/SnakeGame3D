using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BandageItem))]
public class Bandage : InteractableComponent
{
    public override InteractableType Type => InteractableType.ITEM;

    public override bool AcceptItem(ItemComponent item)
    {
        return false;
    }

    public override void ApplyEffect(Effect effect)
    {

    }

    public override void RemoveEffect(EffectType type)
    {

    }
}

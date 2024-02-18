using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FryingPanItem))]
public class FryingPan : InteractableComponent
{
    public override InteractableType Type => InteractableType.ITEM;

    public override void ApplyEffect(Effect effect)
    {
        
    }

    public override void Interact(InteractableComponent sender)
    {
        
    }

    public override void RemoveEffect(EffectType type)
    {
        
    }
}

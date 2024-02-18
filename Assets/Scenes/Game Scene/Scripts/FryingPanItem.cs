using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPanItem : ItemComponent
{
    public override ItemType Type => ItemType.FRYING_PAN;

    public override bool IsValidTarget(InteractableComponent target)
    {
        return target.GetType() == typeof(Egg);
    }

    public override string GetUseAnimationTrigger()
    {
        return "pan";
    }

    public override bool Use(InteractableComponent target)
    {
        return false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandageItem : ItemComponent
{
    public override ItemType Type => ItemType.BANDAGE;

    public override bool ForceOnSelf()
    {
        return true;
    }

    public override bool IsValidTarget(InteractableComponent target)
    {
        return target.GetType() == typeof(PlayerController)
            && ((PlayerController)target).IsBleeding();
    }

    public override string GetUseAnimationTrigger()
    {
        return "bandage";
    }

    public override bool Use(InteractableComponent target)
    {
        target.RemoveEffect(EffectType.BLEED);
        return true;
    }
}

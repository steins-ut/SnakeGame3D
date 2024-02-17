using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeItem : ItemComponent
{
    public override ItemType Type => ItemType.KNIFE;

    private const int k_BleedRate = 8;
    private const float k_BleedDelay = 2f;

    public override bool ForceOnSelf()
    {
        return true;
    }

    public override bool IsValidTarget(InteractableComponent target)
    {
        return target.GetType() == typeof(PlayerController) 
            && !((PlayerController)target).IsBleeding();
    }

    public override bool Use(InteractableComponent target)
    {
        target.ApplyEffect(new Effect(EffectType.BLEED,
                    k_BleedRate,
                    k_BleedDelay,
                    -1));

        return true;
    }
}

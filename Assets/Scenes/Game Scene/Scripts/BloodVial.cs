using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BloodVialItem))]
public class BloodVial : InteractableComponent
{
    public override InteractableType Type => InteractableType.ITEM;

    private BloodVialItem vial = null;

    private Coroutine m_bloodRoutine = null;

    private void Start()
    {
        vial = (BloodVialItem)GetCachedComponent();
    }
    public override void Interact(InteractableComponent sender)
    {

    }

    public override bool AcceptItem(ItemComponent item)
    {
        return false;
    }

    public override void ApplyEffect(Effect effect)
    {
        if (effect.type == EffectType.BLOOD_FED && m_bloodRoutine == null)
        {
            m_bloodRoutine = StartCoroutine(HandleBlood(effect));
        }
    }

    private IEnumerator HandleBlood(Effect effect)
    {
        if (vial.AtMax() || effect.repeat == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(effect.delay);
        vial.AddBlood(effect.magnitude);

        effect.repeat--;
        yield return HandleBlood(effect);
    }

    public override void RemoveEffect(EffectType effect)
    {
        if (effect == EffectType.BLOOD_FED && m_bloodRoutine != null)
        {
            StopCoroutine(m_bloodRoutine);
        }
    }
}
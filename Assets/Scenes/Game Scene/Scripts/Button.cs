using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : InteractableComponent
{
    public override InteractableType Type => InteractableType.ENTITY;

    private Heater m_heater;

    private void Awake()
    {
        m_heater = GetComponentInParent<Heater>();
    }

    public override void Interact(InteractableComponent sender)
    {
        if (sender.GetType() == typeof(PlayerController))
        {
            m_heater.ToggleHeat();
        }
    }

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

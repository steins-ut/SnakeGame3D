using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableComponent : MonoBehaviour
{
    private Component m_cachedComponent = null;

    public abstract void Interact(InteractableComponent sender);

    public virtual string GetInteractionAnimationTrigger()
    { return null; }

    public virtual bool WillAcceptItem(ItemComponent item)
    { return false; }

    public virtual bool AcceptItem(ItemComponent item)
    { return false; }

    public abstract void ApplyEffect(Effect effect);

    public abstract void RemoveEffect(EffectType type);

    public abstract InteractableType Type { get; }

    public Component GetCachedComponent()
    { return m_cachedComponent; }

    private void Awake()
    {
        InteractableType type = Type;
        if (type == InteractableType.ITEM)
        {
            m_cachedComponent = GetComponent<ItemComponent>();
        }
    }
}
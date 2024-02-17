using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableComponent : MonoBehaviour
{
    private Component m_cachedComponent = null;
    public abstract bool AcceptItem(ItemComponent item);
    public abstract void ApplyEffect(Effect effect);
    public abstract void RemoveEffect(EffectType type);
    public abstract InteractableType Type { get; }
    public Component GetCachedComponent() { return m_cachedComponent; }

    private void Awake()
    {
        InteractableType type = Type;
        if(type == InteractableType.ITEM) {
            m_cachedComponent = GetComponent<ItemComponent>();
        }
    }
}

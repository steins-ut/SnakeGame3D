using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemComponent : MonoBehaviour
{
    public abstract ItemType Type { get; }

    public abstract bool IsValidTarget(InteractableComponent target);

    public abstract bool ForceOnSelf(InteractableComponent target);

    public abstract bool OnUse(InteractableComponent target);
}
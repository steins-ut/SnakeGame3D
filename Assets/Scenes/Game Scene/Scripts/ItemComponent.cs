using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemComponent : MonoBehaviour
{
    public abstract ItemType Type { get; }

    public abstract bool IsValidTarget(GameObject target);

    public abstract bool ForceOnSelf();

    public abstract bool OnUse(GameObject target);
}
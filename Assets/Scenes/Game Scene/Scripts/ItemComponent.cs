using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemComponent : MonoBehaviour
{
    [SerializeField]
    private Quaternion m_HoldRotation;

    [SerializeField]
    private Vector3 m_HoldPosition;

    public abstract ItemType Type { get; }

    public abstract bool IsValidTarget(InteractableComponent target);

    public abstract bool ForceOnSelf();

    public abstract bool Use(InteractableComponent target);

    public Quaternion GetHoldRotation() { return m_HoldRotation; }

    public Vector3 GetHoldPosition() { return m_HoldPosition; }
}
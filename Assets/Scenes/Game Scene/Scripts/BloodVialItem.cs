using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodVialItem : ItemComponent
{
    private const int k_BloodLimit = 75;

    private int m_blood = 0;

    public override ItemType Type => ItemType.BLOOD_VIAL;

    public bool AtMax()
    {
        return m_blood == k_BloodLimit;
    }

    public void AddBlood(int blood) { m_blood = Math.Min(k_BloodLimit, m_blood + blood); }

    public override bool ForceOnSelf(InteractableComponent target)
    {
        return false;
    }

    public override bool IsValidTarget(InteractableComponent target)
    {
        return target.GetType() == typeof(Egg);
    }

    public override bool OnUse(InteractableComponent target)
    {
        Egg egg = (Egg)target;
        egg.AddBlood(RemoveBlood(egg.GetBloodHunger()));
        return true;
    }

    public int RemoveBlood(int number) { 
        if(number > m_blood)
        {
            int ret = m_blood;
            m_blood = 0;
            return ret;
        }
        else
        {
            m_blood -= number;
            return number;
        }
    }
}

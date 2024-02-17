using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : InteractableComponent
{
    private const int k_BloodLimit = 100;
    private const int k_BloodHungerRate = 5;
    private const int k_BloodHungerTime = 1;

    private const int k_MinTemperature = 33;
    private const int k_MaxTemperature = 40;

    private const float k_TemperatureChangeTime = 5f;
    private const int k_TemperataureChangeRandomness = 0;

    private int m_blood = k_BloodLimit;
    private int m_temperature = 35;

    public override InteractableType Type => InteractableType.ENTITY;

    private IEnumerator HandleTemperature()
    {
        yield return new WaitForSeconds(k_TemperatureChangeTime +
                                UnityEngine.Random.value * k_TemperataureChangeRandomness);

        m_temperature = m_temperature - 1;
        if (m_temperature < k_MinTemperature)
        {
            //die code
            Debug.Log("you dieded");
        }

        yield return HandleTemperature();
    }

    private IEnumerator HandleBlood()
    {
        yield return new WaitForSeconds(k_BloodHungerTime);

        m_blood -= k_BloodHungerRate;
        if (m_blood <= 0)
        {
            //die code
            Debug.Log("you dieded");
        }

        yield return HandleBlood();
    }

    public void AddHeat(int heat)
    {
        m_temperature += heat;
        if (m_temperature > k_MaxTemperature)
        {
            //die code
            Debug.Log("you dieded");
        }
    }

    public void AddBlood(int blood)
    {
        m_blood = Math.Min(k_BloodLimit, m_blood + blood);
    }

    public int GetBloodHunger()
    {
        return k_BloodLimit - m_blood;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void StartHatching()
    {
        StartCoroutine(HandleTemperature());
        StartCoroutine(HandleBlood());
    }

    public void StopHatching()
    {
        StopAllCoroutines();
    }

    public override bool AcceptItem(ItemComponent item)
    {
        if(item.Type == ItemType.BLOOD_VIAL) {
            BloodVialItem vial = (BloodVialItem)item;
            return true;
        }

        return false;
    }

    public override void ApplyEffect(Effect effect)
    {
        throw new NotImplementedException();
    }

    public override void RemoveEffect(EffectType effect)
    {
        throw new NotImplementedException();
    }
}
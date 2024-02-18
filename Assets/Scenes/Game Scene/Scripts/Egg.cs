using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : InteractableComponent
{
    [SerializeField]
    private TMPro.TextMeshPro m_temperatureText;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_bloodthirstText;

    private const int k_BloodLimit = 100;
    private const int k_BloodHungerRate = 5;
    private const int k_BloodHungerTime = 1;

    private const int k_MinTemperature = 33;
    private const int k_MaxTemperature = 40;

    private const float k_TemperatureChangeTime = 7f;
    private const float k_TemperataureChangeRandomness = 2f;

    private int m_blood = k_BloodLimit;
    private int m_temperature = 35;

    public override InteractableType Type => InteractableType.ENTITY;

    private IEnumerator HandleTemperature()
    {
        yield return new WaitForSeconds(k_TemperatureChangeTime +
                                UnityEngine.Random.Range(k_TemperatureChangeTime, 
                                                k_TemperatureChangeTime + k_TemperataureChangeRandomness));

        m_temperature = m_temperature - 1;
        if (m_temperature < k_MinTemperature)
        {
            GameManager.s_Instance.GoToGameOver(GameOverReason.HYPOTHERMIA);
        }

        yield return HandleTemperature();
    }

    private IEnumerator HandleBlood()
    {
        yield return new WaitForSeconds(k_BloodHungerTime);

        m_blood -= k_BloodHungerRate;
        if (m_blood <= 0)
        {
            GameManager.s_Instance.GoToGameOver(GameOverReason.BLOODTHIRST);
        }

        yield return HandleBlood();
    }

    public void AddHeat(int heat)
    {
        m_temperature += heat;
        if (m_temperature > k_MaxTemperature)
        {
            GameManager.s_Instance.GoToGameOver(GameOverReason.HYPERTHERMIA);
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
        m_temperatureText.text = m_temperature.ToString() + "°C";
        m_bloodthirstText.text = m_blood.ToString();
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
        if(WillAcceptItem(item)) {
            BloodVialItem vial = (BloodVialItem)item;
            int add = vial.RemoveBlood(GetBloodHunger());
            m_blood += add;
            Debug.Log(add);
            return true;
        }

        return false;
    }

    public override void ApplyEffect(Effect effect)
    {

    }

    public override void RemoveEffect(EffectType effect)
    {

    }

    public override void Interact(InteractableComponent sender)
    {

    }

    public override bool WillAcceptItem(ItemComponent item)
    {
        return item.Type == ItemType.BLOOD_VIAL;
    }
}
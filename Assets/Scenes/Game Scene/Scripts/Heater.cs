using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heater : MonoBehaviour
{
    [SerializeField]
    private Egg m_egg;

    private const float k_HeatDelay = 4;
    private const int k_HeatChange = 1;

    private Coroutine m_heaterRoutine = null;

    private IEnumerator HandleHeat()
    {
        yield return new WaitForSeconds(k_HeatDelay);

        m_egg.AddHeat(k_HeatChange);

        yield return HandleHeat();
    }

    public void ToggleHeat()
    {
        if (m_heaterRoutine != null)
        {
            SoundManager.s_Instance.StopMicrowaveHum();
            StopCoroutine(m_heaterRoutine);
            m_heaterRoutine = null;
        }
        else
        {
            SoundManager.s_Instance.PlayMicrowaveHum();
            m_heaterRoutine = StartCoroutine(HandleHeat());
        }
    }
}
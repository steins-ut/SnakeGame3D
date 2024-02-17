using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_timer;

    [SerializeField]
    private Egg m_egg;

    private Coroutine m_timerRoutine;

    private int m_remainingTime = 900;

    private void OnTick()
    {
    }

    private IEnumerator TickTime()
    {
        yield return new WaitForSeconds(1);
        OnTick();
        m_remainingTime--;
        m_timer.text = (m_remainingTime / 60) + ":" + (m_remainingTime % 60);
        if (m_remainingTime > 0)
        {
            yield return TickTime();
        }
        else
        {
            m_timerRoutine = null;
            yield break;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_timerRoutine = StartCoroutine(TickTime());
        m_egg.StartHatching();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
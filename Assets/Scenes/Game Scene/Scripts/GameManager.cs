using SnakeGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_timer;

    [SerializeField]
    private TemperatureHandler m_temperatureHandler;

    private Coroutine Timer;

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
            Debug.Log("You win lololol");
            Timer = null;
            yield break;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Timer = StartCoroutine(TickTime());
        m_temperatureHandler.StartHandling();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDestroy()
    {
        if (Timer != null) { StopCoroutine(Timer); }
    }
}
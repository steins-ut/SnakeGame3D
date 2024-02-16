using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class TemperatureHandler : MonoBehaviour
    {
        private bool m_isHeaterOn = false;
        private int m_currentTemperature = 35;

        private const int k_MinTemperature = 33;
        private const int k_MaxTemperature = 40;

        private const float k_TemperatureChangeTime = 5f;
        private const int k_TemperataureChangeRandomness = 0;

        private IEnumerator HandleTemperature()
        {
            Debug.Log(m_currentTemperature);
            Debug.Log(m_isHeaterOn);

            yield return new WaitForSeconds(k_TemperatureChangeTime +
                                    Random.value * k_TemperataureChangeRandomness);

            m_currentTemperature = m_isHeaterOn ? m_currentTemperature + 1 : m_currentTemperature - 1;
            if (m_currentTemperature < k_MinTemperature || m_currentTemperature > k_MaxTemperature)
            {
                Debug.Log("you dieded");
            }

            yield return HandleTemperature();
        }

        // Start is called before the first frame update
        private void Start()
        {
            m_currentTemperature = (k_MaxTemperature + k_MinTemperature) / 2;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H)) { m_isHeaterOn = !m_isHeaterOn; }
        }

        public void StartHandling()
        {
            StartCoroutine(HandleTemperature());
        }
    }
}
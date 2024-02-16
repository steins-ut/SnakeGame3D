using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const int m_blackoutMin = 2;

    private const int m_blackoutMax = 4;

    private int m_blood = 120;
    private int m_bloodRegenRate = 12;
    private int m_blackoutLimit = 12;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
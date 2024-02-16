using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodVial : MonoBehaviour
{
    private const int k_BloodLimit = 75;

    private int m_blood = 0;

    public void AddBlood(int blood) { m_blood = Math.Min(k_BloodLimit, m_blood + blood); }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

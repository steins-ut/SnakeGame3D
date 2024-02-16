using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GunHandler : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    private Camera m_mainCamera;
    private bool m_isGunInHand = false;
    private int m_gunAmmo = 1;

    private bool m_handling = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_handling && Input.GetMouseButtonDown(0))
        {
            if (!m_isGunInHand)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider = Physics2D.OverlapPoint(pos, layerMask);

                if (collider != null && collider.CompareTag("Gun"))
                {
                    m_isGunInHand = true;
                    Debug.Log("gun picked up");
                }
            }
            else
            {
                //shoomt
            }
        }
    }

    public void StartHandling()
    {
    }
}
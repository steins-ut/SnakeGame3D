using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public bool inSight;
    public KeyCode interactKey;
    public UnityEvent action;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inSight)
        {
            if (Input.GetKeyDown(interactKey))
            {
                action.Invoke();
            }
        }

    }
}

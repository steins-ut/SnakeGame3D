using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageData : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_line1;

    public TMPro.TextMeshProUGUI GetLine1()
    {
        return m_line1;
    }

    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        GetComponent<Animator>().SetTrigger("animate");
        source.loop = false;
        source.Play();
    }
}
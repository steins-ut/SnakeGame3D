using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_playerController;

    [SerializeField]
    private UnityEngine.UI.Image m_blackscreen;

    public PlayerController GetPlayerController()
    { return m_playerController; }

    public void Blackout()
    { m_blackscreen.enabled = true; }
}
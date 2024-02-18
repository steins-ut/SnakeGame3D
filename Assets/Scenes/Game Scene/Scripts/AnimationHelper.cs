using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_playerController;

    public PlayerController GetPlayerController() { return m_playerController; }
}

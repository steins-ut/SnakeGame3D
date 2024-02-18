using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_message;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_description;

    void Start()
    {
        m_message.text = Globals.s_GameOverMessage;
        m_description.text = Globals.s_GameOverDescription;
    }

    public void Replay()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu Scene");
    }
}

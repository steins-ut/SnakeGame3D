using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static void StartGame()
    {
        SceneManager.LoadScene("Transition Scene");
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        GetComponent<AudioSource>().Play();
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
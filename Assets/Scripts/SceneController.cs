using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void endGame()
    {
        SceneManager.LoadScene("endScreen");
    }

    public void Title()
    {
        SceneManager.LoadScene("title");
    }
}

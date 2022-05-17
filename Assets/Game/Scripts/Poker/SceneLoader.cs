using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadTownScene()
    {
        SceneManager.LoadScene("Muncie");
    }

    public void LoadQuickMatch()
    {
        SceneManager.LoadScene("PokerTable");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

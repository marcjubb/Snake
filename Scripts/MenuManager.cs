using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MenuManager : MonoBehaviour
{

 

    private void Start()
    {
        
    }
    public void Settings()
    {
        FindObjectOfType<SceneTransition>().FadeToLevel("settings");
    }

    public void PlayGame()
    {
        FindObjectOfType<SceneTransition>().FadeToLevel("game");
    }

    public void SelectProfile()
    {
        FindObjectOfType<SceneTransition>().FadeToLevel("profile");
    }
    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quit");
        UnityEngine.Application.Quit();
    }
}

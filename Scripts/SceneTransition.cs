using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnimator;

    public GameObject transitionPanel;
    private string sceneNameLoad;

    public void FadeToLevel(string sceneName)
    {
        sceneNameLoad = sceneName;
        transitionAnimator.SetTrigger("FadeOut");

    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneNameLoad);
    }



}
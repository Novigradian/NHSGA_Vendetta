using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject selectTutorialMenu;
    public GameObject creditsMenu;
    public Animator crossfadeAnim;
    public AudioManager audioManager;

    // Update is called once per frame
    void Update()
    {

    }

    #region Start Game
    public void StartGame()
    {
        selectTutorialMenu.SetActive(true);
        audioManager.Play("UI");
    }

    public void StartTutorial()
    {
        StartCoroutine(BeginLoadScene(1));
        audioManager.Play("UI");
    }

    public void SkipTutorial()
    {
        StartCoroutine(BeginLoadScene(6));
        audioManager.Play("UI");
    }

    private IEnumerator BeginLoadScene(int index)
    {
        crossfadeAnim.SetTrigger("StartFade");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(index);
    }

    #endregion

    #region Credits
    public void ShowCredits()
    {
        creditsMenu.SetActive(true);
        audioManager.Play("UI");
    }

    public void HideCredits()
    {
        creditsMenu.SetActive(false);
        audioManager.Play("UI");
    }

    #endregion
}

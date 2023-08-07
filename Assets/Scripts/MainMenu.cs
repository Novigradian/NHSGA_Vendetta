using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject selectTutorialMenu;
    public GameObject creditsMenu;
    public GameObject crossfade;
    private Animator crossfadeAnim;

    void Start()
    {
        crossfadeAnim = crossfade.GetComponent<Animator>();

        selectTutorialMenu.SetActive(false);
        creditsMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Start Game
    public void StartGame()
    {
        selectTutorialMenu.SetActive(true);
    }

    public void StartTutorial()
    {
        StartCoroutine(BeginLoadScene(1));
    }

    public void SkipTutorial()
    {
        StartCoroutine(BeginLoadScene(2));
    }

    private IEnumerator BeginLoadScene(int index)
    {
        //crossfade.SetActive(true);
        crossfadeAnim.SetTrigger("StartFade");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(index);
    }

    #endregion

    #region Credits
    public void ShowCredits()
    {
        creditsMenu.SetActive(true);
    }

    public void HideCredits()
    {
        creditsMenu.SetActive(false);
    }

    #endregion
}

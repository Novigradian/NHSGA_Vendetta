using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public MusicManager musicManager;
    public AudioManager audioManager;
    [HideInInspector] public bool isPaused;

    public Animator crossfadeAnim;
    private void Start()
    {
        
    }
    void Update()
    {
        //AudioSource[] audios = FindObjectsOfType<AudioSource>();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in audios)
        {
            a.Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("resume");
        audioManager.Play("UI");
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in audios)
        {
            a.UnPause();
        }
    }
    public void Exit()
    {
        Debug.Log("exit");
        audioManager.Play("UI");
        Time.timeScale = 1;
        isPaused = false;
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        crossfadeAnim.gameObject.SetActive(true);
        crossfadeAnim.SetTrigger("StartFade");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowControls()
    {
        audioManager.Play("UI");
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void HideControls()
    {
        audioManager.Play("UI");
        pauseMenu.SetActive(true);
        controlsMenu.SetActive(false);
    }
}

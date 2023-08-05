using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    [HideInInspector] public bool isPaused;

    private void Start()
    {
        
    }
    void Update()
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
                foreach (AudioSource a in audios)
                {
                    a.UnPause();
                }
            }
            else
            {
                Pause();
                foreach (AudioSource a in audios)
                {
                    a.Pause();
                }
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

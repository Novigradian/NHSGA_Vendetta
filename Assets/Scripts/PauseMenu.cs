using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public MusicManager musicManager;
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
                    if (a.clip.name != "TutorialMusic")
                    {
                        a.UnPause();
                    }
                    else
                    {
                        musicManager.EndMuffle();
                    }
                }
            }
            else
            {
                Pause();
                foreach (AudioSource a in audios)
                {
                    if (a.clip.name != "TutorialMusic")
                    {
                        a.Pause();
                    }
                    else
                    {
                        musicManager.StartMuffle();
                    }
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
        Debug.Log("resume");
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Exit()
    {
        Debug.Log("exit");
        SceneManager.LoadScene("MainMenu");
    }
}

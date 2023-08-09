using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject restartButton;
    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public IEnumerator enableButton()
    {
        yield return new WaitForSeconds(5);
        restartButton.SetActive(true);
    }

    private void Start()
    {
        restartButton.SetActive(false);
        StartCoroutine(enableButton());
    }
}

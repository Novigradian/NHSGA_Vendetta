using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Keyboard kb;

    void Start()
    {
        kb = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (kb.anyKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(1);
        }   
    }
}

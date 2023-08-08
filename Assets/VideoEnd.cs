using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoEnd : MonoBehaviour
{
    // Start is called before the first frame update
    private float timer;
    [SerializeField] private float videoTime;
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= videoTime)
        {
            SceneManager.LoadScene("Level1");
        }
    }
}

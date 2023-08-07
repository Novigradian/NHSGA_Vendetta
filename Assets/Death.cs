using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyController enemyController;
    public GameManager gameManager;
    public GameObject optionUI;
    void Start()
    {
        enemyController = FindObjectOfType<EnemyController>();
        optionUI.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyController.state == EnemyController.EnemyState.dead && gameManager.gameState == "PlayerWin")
        {
            optionUI.SetActive(true);
        }
    }
}

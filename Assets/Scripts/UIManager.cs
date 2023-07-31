using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject enemyUI;

    public GameManager gameManager;

    private bool isResetUIPos;

    void Start()
    {
        isResetUIPos = false;
        enemyUI.transform.position = new Vector3(enemyUI.transform.position.x, enemyUI.transform.position.y + 100f, enemyUI.transform.position.z);
        playerUI.transform.position = new Vector3(playerUI.transform.position.x, playerUI.transform.position.y - 100f, playerUI.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        string gameState = gameManager.gameState;
        if (gameState == "Fight" && !isResetUIPos)
        {
            isResetUIPos = true;
            enemyUI.transform.position = new Vector3(enemyUI.transform.position.x, enemyUI.transform.position.y - 100f, enemyUI.transform.position.z);
            playerUI.transform.position = new Vector3(playerUI.transform.position.x, playerUI.transform.position.y + 100f, playerUI.transform.position.z);
        }
    
    }
}

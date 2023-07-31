using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    #region Dialogue Manager Components and Variables
    Keyboard kb;

    public string[] preFightDialogueSpeakerList;
    public string[] preFightDialogueList;
    private int preFightDialogueIndex;

    public string[] playerWinDialogueList;
    private int playerWinDialogueIndex;

    public string[] enemyWinDialogueList;
    private int enemyWinDialogueIndex;

    public GameObject playerDialogue;
    public TMP_Text playerDialogueText;
    public GameObject enemyDialogue;
    public TMP_Text enemyDialogueText;

    public GameManager gameManager;
    #endregion

    void Start()
    {
        #region Initialize Variables
        kb = Keyboard.current;

        preFightDialogueIndex = 0;
        playerWinDialogueIndex = 0;
        enemyWinDialogueIndex = 0;
        #endregion

        ShowPreFightDialogue(preFightDialogueIndex);
    }

    // Update is called once per frame
    void Update()
    {
        string gameState = gameManager.gameState;
        if (gameState == "PreFight")
        {
            if (kb.anyKey.wasPressedThisFrame)
            {
                preFightDialogueIndex++;
                if (preFightDialogueIndex < preFightDialogueList.Length)
                {
                    ShowPreFightDialogue(preFightDialogueIndex);
                }
                else
                {
                    gameManager.gameState = "Fight";
                    playerDialogue.SetActive(false);
                    enemyDialogue.SetActive(false);
                }
            }
        }
        else if (gameState == "PlayerWinDialogue")
        {
            playerDialogueText.text = playerWinDialogueList[playerWinDialogueIndex];
            if (kb.anyKey.wasPressedThisFrame)
            {
                playerWinDialogueIndex++;
                if (playerWinDialogueIndex >= playerWinDialogueList.Length)
                {
                    gameManager.gameState = "PlayerWin";
                    playerDialogue.SetActive(false);
                }
            }
        }
        else if (gameState == "EnemyWinDialogue")
        {
            enemyDialogueText.text = enemyWinDialogueList[enemyWinDialogueIndex];
            if (kb.anyKey.wasPressedThisFrame)
            {
                enemyWinDialogueIndex++;
                if (enemyWinDialogueIndex >= enemyWinDialogueList.Length)
                {
                    gameManager.gameState = "EnemyWin";
                    enemyDialogue.SetActive(false);
                }
            }
        }
    }

    private void ShowPreFightDialogue(int index)
    {
        playerDialogue.SetActive(false);
        enemyDialogue.SetActive(false);

        string speaker = preFightDialogueSpeakerList[index];
        if (speaker == "Enemy")
        {
            enemyDialogue.SetActive(true);
            enemyDialogueText.text = preFightDialogueList[index];
        }
        else if (speaker == "Player")
        {
            playerDialogue.SetActive(true);
            playerDialogueText.text = preFightDialogueList[index];
        }
    }
}

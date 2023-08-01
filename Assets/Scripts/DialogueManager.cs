using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    #region Dialogue Manager Components and Variables
    Keyboard kb;
    private Animator anim;

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

    private Coroutine currentCoroutine;

    [SerializeField] private float displayDialogueInterval;

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
                    anim.SetTrigger("returnToIdle");
                    ShowPreFightDialogue(preFightDialogueIndex);
                }
                else
                {
                    currentCoroutine = null;
                    gameManager.gameState = "FightText";
                    gameManager.dialogueVolume.SetActive(false);
                    playerDialogue.SetActive(false);
                    enemyDialogue.SetActive(false);
                }
            }
        }
        else if (gameState == "PlayerWinDialogue")
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DisplayPlayerText(playerWinDialogueList[playerWinDialogueIndex]));
            }
            
            if (kb.anyKey.wasPressedThisFrame)
            {
                playerWinDialogueIndex++;
                if (playerWinDialogueIndex >= playerWinDialogueList.Length)
                {
                    gameManager.gameState = "PlayerWin";
                    playerDialogue.SetActive(false);
                }
                else
                {
                    StopCoroutine(currentCoroutine);
                    anim.SetTrigger("returnToIdle");
                    currentCoroutine =StartCoroutine(DisplayPlayerText(playerWinDialogueList[playerWinDialogueIndex]));
                }
            }
        }
        else if (gameState == "EnemyWinDialogue")
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DisplayEnemyText(enemyWinDialogueList[enemyWinDialogueIndex]));
            }
            if (kb.anyKey.wasPressedThisFrame)
            {
                enemyWinDialogueIndex++;
                if (enemyWinDialogueIndex >= enemyWinDialogueList.Length)
                {
                    gameManager.gameState = "EnemyWin";
                    enemyDialogue.SetActive(false);
                }
                else
                {
                    StopCoroutine(currentCoroutine);
                    anim.SetTrigger("returnToIdle");
                    currentCoroutine =StartCoroutine(DisplayEnemyText(enemyWinDialogueList[enemyWinDialogueIndex]));
                }
            }
        }
    }

    private void ShowPreFightDialogue(int index)
    {
        playerDialogue.SetActive(false);
        enemyDialogue.SetActive(false);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        string speaker = preFightDialogueSpeakerList[index];
        string text= preFightDialogueList[index];
        if (speaker == "Enemy")
        {
            enemyDialogue.SetActive(true);
            currentCoroutine=StartCoroutine(DisplayEnemyText(text));
        }
        else if (speaker == "Player")
        {
            playerDialogue.SetActive(true);
            currentCoroutine=StartCoroutine(DisplayPlayerText(text));
        }
    }

    private IEnumerator DisplayPlayerText(string text)
    {
        anim = playerDialogueText.GetComponent<Animator>();
        anim.SetTrigger("startFadeIn");
        
        playerDialogueText.text = "";
        foreach (char character in text)
        {
            playerDialogueText.text += character;
            yield return new WaitForSeconds(displayDialogueInterval);
        }
    }

    private IEnumerator DisplayEnemyText(string text)
    {
        anim = enemyDialogueText.GetComponent<Animator>();
        anim.SetTrigger("startFadeIn");

        enemyDialogueText.text = "";
        foreach (char character in text)
        {
            enemyDialogueText.text += character;
            yield return new WaitForSeconds(displayDialogueInterval);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{

    #region Dialogue Manager Components and Variables
    Keyboard kb;
    private Animator anim;

    public MusicManager musicManager;

    public string[] mysteriousVoiceDialogueList;
    private int mysteriousVoiceDialogueIndex;

    public string[] preFightDialogueSpeakerList;
    public string[] preFightDialogueList;
    private int preFightDialogueIndex;

    public string[] playerWinDialogueList;
    public string[] playerWinDialogueSpeakerList;
    private int playerWinDialogueIndex;

    public string[] enemyWinDialogueList;
    private int enemyWinDialogueIndex;

    public GameObject playerDialogue;
    public TMP_Text playerDialogueText;
    public GameObject enemyDialogue;
    public TMP_Text enemyDialogueText;
    public GameObject mysteroisVoiceDialogue;
    public TMP_Text mysteriousVoiceDialogueText;

    private Coroutine currentCoroutine;

    [SerializeField] private float displayDialogueInterval;

    public GameManager gameManager;

    private bool isSceneTwo;
    #endregion

    void Start()
    {
        #region Initialize Variables
        kb = Keyboard.current;

        preFightDialogueIndex = 0;
        playerWinDialogueIndex = 0;
        enemyWinDialogueIndex = 0;
        mysteriousVoiceDialogueIndex = 0;

        isSceneTwo = gameManager.isSceneTwo;
        #endregion
        
        if (gameManager.gameState == "PreFight")
        {
            ShowPreFightDialogue(preFightDialogueIndex);
            musicManager.StartMuffle();
        }
        else if (gameManager.gameState=="MysteriousVoice")
        {
            mysteroisVoiceDialogue.SetActive(true);
            ShowMysteriousVoiceDialogue(mysteriousVoiceDialogueIndex);
            musicManager.StartMuffle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        string gameState = gameManager.gameState;
        if (gameState=="MysteriousVoice")
        {
            if (kb.enterKey.wasPressedThisFrame)
            {
                mysteriousVoiceDialogueIndex++;
                if (mysteriousVoiceDialogueIndex < mysteriousVoiceDialogueList.Length)
                {
                    anim.SetTrigger("returnToIdle");
                    ShowMysteriousVoiceDialogue(mysteriousVoiceDialogueIndex);
                }
                else
                {
                    currentCoroutine = null;
                    gameManager.gameState = "PreFight";
                    gameManager.enemy.SetActive(true);
                    gameManager.player.SetActive(true);
                    mysteroisVoiceDialogue.SetActive(false);
                    ShowPreFightDialogue(preFightDialogueIndex);

                    gameManager.mysteriousVoiceVolume.SetActive(false);
                    gameManager.dialogueVolume.SetActive(true);
                }
            }
        }
        else if (gameState == "PreFight")
        {
            if (kb.enterKey.wasPressedThisFrame)
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
                    gameManager.combatVolume.SetActive(true);

                    musicManager.EndMuffle();
                    if (gameManager.isSceneTwo)
                    {
                        DataHolder.hasLevelTwoDialogueShown = true;
                    }
                    else
                    {
                        DataHolder.hasLevelOneDialogueShown = true;
                    }
                }
            }
        }
        else if (gameState == "PlayerWinDialogue")
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DisplayPlayerText(playerWinDialogueList[playerWinDialogueIndex]));
            }
            
            if (kb.enterKey.wasPressedThisFrame)
            {
                playerWinDialogueIndex++;
                if (playerWinDialogueIndex >= playerWinDialogueList.Length)
                {
                    currentCoroutine = null;
                    if (isSceneTwo)
                    {
                        gameManager.gameState = "KillChoice";
                    }
                    else
                    {
                        gameManager.gameState = "PlayerWin";
                    }
                    
                    playerDialogue.SetActive(false);
                    enemyDialogue.SetActive(false);

                    musicManager.EndMuffle();
                }
                else
                {
                    anim.SetTrigger("returnToIdle");
                    ShowPlayerWinDialogue(playerWinDialogueIndex);
                }
            }
        }
        else if (gameState == "EnemyWinDialogue")
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(DisplayEnemyText(enemyWinDialogueList[enemyWinDialogueIndex]));
            }
            if (kb.enterKey.wasPressedThisFrame)
            {
                enemyWinDialogueIndex++;
                if (enemyWinDialogueIndex >= enemyWinDialogueList.Length)
                {
                    gameManager.gameState = "EnemyWin";
                    enemyDialogue.SetActive(false);

                    musicManager.EndMuffle();
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

    private void ShowMysteriousVoiceDialogue(int index)
    {
        mysteroisVoiceDialogue.SetActive(false);
        
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        mysteroisVoiceDialogue.SetActive(true);
        currentCoroutine = StartCoroutine(DisplayMysteriousVoiceText(mysteriousVoiceDialogueList[index]));
    }

    private void ShowPreFightDialogue(int index)
    {
        playerDialogue.SetActive(false);
        enemyDialogue.SetActive(false);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        if (index == 3 && isSceneTwo)
        {
            enemyDialogue.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Daniel";
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
    public void ShowPlayerWinDialogue(int index)
    {
        playerDialogue.SetActive(false);
        enemyDialogue.SetActive(false);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        string speaker = playerWinDialogueSpeakerList[index];
        Debug.Log(speaker);
        string text = playerWinDialogueList[index];
        if (speaker == "Enemy")
        {
            enemyDialogue.SetActive(true);
            currentCoroutine = StartCoroutine(DisplayEnemyText(text));
        }
        else if (speaker == "Player")
        {
            playerDialogue.SetActive(true);
            currentCoroutine = StartCoroutine(DisplayPlayerText(text));
        }
    }

    private IEnumerator DisplayMysteriousVoiceText(string text)
    {
        anim = mysteriousVoiceDialogueText.GetComponent<Animator>();
        anim.SetTrigger("startFadeIn");

        mysteriousVoiceDialogueText.text = "";
        foreach (char character in text)
        {
            mysteriousVoiceDialogueText.text += character;
            yield return new WaitForSeconds(displayDialogueInterval);
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

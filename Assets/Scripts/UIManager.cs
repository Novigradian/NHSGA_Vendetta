using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject enemyUI;

    public GameObject playerWinUI;
    public GameObject enemyWinUI;

    public GameObject fightTextUI;
    [SerializeField] private float fightTextUIShowDuration;

    public GameManager gameManager;

    private bool isResetUIPos;
    private bool isShowWinText;
    private bool isShowFightText;

    void Start()
    {
        isResetUIPos = false;
        isShowWinText = false;
        isShowFightText = false;
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
        else if ((gameState == "FightText") && !isShowFightText)
        {
            StartCoroutine(BeginFight());
        }
        else if ((gameState=="PlayerWinDialogue" || gameState == "EnemyWinDialogue") && isResetUIPos)
        {
            isResetUIPos = false;
            enemyUI.transform.position = new Vector3(enemyUI.transform.position.x, enemyUI.transform.position.y + 100f, enemyUI.transform.position.z);
            playerUI.transform.position = new Vector3(playerUI.transform.position.x, playerUI.transform.position.y - 100f, playerUI.transform.position.z);
        }
        else if ((gameState == "PlayerWin") && !isShowWinText)
        {
            isShowWinText = true;
            playerWinUI.SetActive(true);
        }
        else if ((gameState=="EnemyWin")&& !isShowWinText)
        {
            isShowWinText = true;
            enemyWinUI.SetActive(true);
        }
    }

    private IEnumerator BeginFight()
    {
        isShowFightText = true;
        fightTextUI.SetActive(true);
        TMP_Text fightText = fightTextUI.GetComponent<TextMeshProUGUI>();
        fightText.text = "3";
        yield return new WaitForSeconds(fightTextUIShowDuration*0.25f);
        fightText.text = "2";
        yield return new WaitForSeconds(fightTextUIShowDuration * 0.25f);
        fightText.text = "1";
        yield return new WaitForSeconds(fightTextUIShowDuration * 0.25f);
        fightText.text = "En Garde!";
        
        gameManager.gameState = "Fight";
        gameManager.fightVolume.SetActive(true);

        yield return new WaitForSeconds(fightTextUIShowDuration * 0.25f);
        fightTextUI.SetActive(false);
    }
}

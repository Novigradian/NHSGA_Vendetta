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

    public GameObject parryTextUI;
    public GameObject riposteTextUI;
    public GameObject damageTextUI;
    public GameObject blockTextUI;
    private RectTransform parryTextRectTransform;
    private RectTransform riposteTextRectTransform;
    [SerializeField] private float showTextDuration;
    [SerializeField] private float showDamageTextDuration;

    public GameManager gameManager;
    public GameObject canvas;
    private RectTransform canvasRectTransform;

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

        parryTextRectTransform = parryTextUI.GetComponent<RectTransform>();
        riposteTextRectTransform = riposteTextUI.GetComponent<RectTransform>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
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

    public void ShowParryText(Vector3 playerWorldPos)
    {
        parryTextUI.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerWorldPos);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, Camera.main, out canvasPosition);
        riposteTextRectTransform.localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, 145f);
        StartCoroutine(HideParryText());
    }

    private IEnumerator HideParryText()
    {
        yield return new WaitForSeconds(showTextDuration);
        parryTextUI.SetActive(false);
    }

    public void ShowRiposteText(Vector3 playerWorldPos)
    {
        riposteTextUI.SetActive(true);
        riposteTextRectTransform.localPosition = WorldToCanvasPos(playerWorldPos)+new Vector2(0f,145f);
        StartCoroutine(HideRiposteText());
    }

    private IEnumerator HideRiposteText()
    {
        yield return new WaitForSeconds(showTextDuration);
        riposteTextUI.SetActive(false);
    }

    public void ShowDamageText(Vector3 playerWorldPos, float damage)
    {
        GameObject damageTextUIInstance = Instantiate(damageTextUI,canvas.transform);
        damageTextUIInstance.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(damage).ToString();

        damageTextUIInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, 145f);
        StartCoroutine(HideDamageText(damageTextUIInstance));
    }

    private IEnumerator HideDamageText(GameObject damageTextUIInstance)
    {
        yield return new WaitForSeconds(showDamageTextDuration);
        Destroy(damageTextUIInstance);
    }

    public void ShowBlockText(Vector3 playerWorldPos)
    {
        GameObject blockTextUIInstance = Instantiate(blockTextUI, canvas.transform);

        blockTextUIInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, 187.5f);
        StartCoroutine(HideDamageText(blockTextUIInstance));
    }

    private Vector2 WorldToCanvasPos(Vector3 playerWorldPos)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerWorldPos);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, Camera.main, out canvasPosition);
        return canvasPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    #region Tutorial Manager Variables and Components
    Keyboard kb;

    public string gameState;

    public GameObject canvas;
    private RectTransform canvasRectTransform;

    public MusicManager musicManager;

    public TutorialPlayerController tutorialPlayerController;

    [Header ("GamePlay")]
    public float minimumPlayerDummyDistance;

    [Header("Tutorial Instructions")]
    public GameObject tutorialInstructionUI;
    public TMP_Text tutorialInstructionText;
    public string[] tutorialInstructionList;
    public int tutorialInstructionIndex;

    public int[] instructionTasksCompletedList;
    private int instructionTasksCompleted;
    private bool isNextInstruction;
    //[SerializeField] private float showNextInstructionWaitDuration;

    [Header("Text")]
    public GameObject fientTextUI;
    public GameObject damageTextUI;
    public GameObject blockTextUI;
    public GameObject outOfStaminaTextUI;
    public GameObject riposteTextUI;
    public GameObject critTextUI;
    [SerializeField] private float showDamageTextDuration;
    [SerializeField] private float showTextDuration;
    private RectTransform riposteTextRectTransform;

    [Header("Particle Effects")]
    public GameObject leftBloodParticlePrefab;
    public GameObject rightBloodParticlePrefab;

    [Header("Postprocessing")]
    public GameObject dialogueVolume;
    public GameObject getHitVolume;
    [SerializeField] private float getHitVolumeShowDuration;

    [Header("Dialogue")]
    [SerializeField] private float displayDialogueInterval;
    public GameObject playerDialogue;
    public TMP_Text playerDialogueText;
    public string[] preTutorialDialogueList;
    public int preTutorialDialogueIndex;
    private Animator anim;

    private Coroutine currentCoroutine;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        #region Initialize Variables
        kb = Keyboard.current;

        gameState = "PreTutorialDialogue";

        tutorialInstructionIndex = -1;
        isNextInstruction = true;
        instructionTasksCompleted = 0;

        playerDialogue.SetActive(false);
        preTutorialDialogueIndex = 0;

        canvasRectTransform = canvas.GetComponent<RectTransform>();
        riposteTextRectTransform = riposteTextUI.GetComponent<RectTransform>();

        dialogueVolume.SetActive(true);
        getHitVolume.SetActive(false);
        #endregion

        
        ShowPreTutorialDialogue(0);
        musicManager.StartMuffle();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState== "PreTutorialDialogue")
        {
            if (kb.anyKey.wasPressedThisFrame)
            {
                preTutorialDialogueIndex++;
                if (preTutorialDialogueIndex < preTutorialDialogueList.Length-1)
                {
                    anim.SetTrigger("returnToIdle");
                    ShowPreTutorialDialogue(preTutorialDialogueIndex);
                }
                else
                {
                    currentCoroutine = null;
                    gameState = "Fight";
                    dialogueVolume.SetActive(false);
                    playerDialogue.SetActive(false);
                    tutorialInstructionUI.SetActive(true);

                    musicManager.EndMuffle();
                }
            }
        }
        else if (gameState == "Fight" && isNextInstruction)
        {
            tutorialInstructionIndex++;
            isNextInstruction = false;
            //Debug.Log(tutorialInstructionIndex);
            if (tutorialInstructionIndex < tutorialInstructionList.Length - 4)
            {
                ShowInstruction(tutorialInstructionIndex);
                
            }
            else
            {
                gameState = "SpecialTutorialInstructions";
                ShowSpecialInstruction(tutorialInstructionIndex);
            }

        }
        else if (gameState == "SpecialTutorialInstructions")
        {
            if (kb.escapeKey.wasPressedThisFrame)
            {
                tutorialInstructionIndex++;
                if (tutorialInstructionIndex < tutorialInstructionList.Length - 1)
                {
                    ShowSpecialInstruction(tutorialInstructionIndex);
                }
                else
                {
                    if (!tutorialPlayerController.isJumping)
                    {
                        gameState = "PostTutorialDialogue";
                        tutorialInstructionUI.SetActive(false);
                        dialogueVolume.SetActive(true);
                        ShowPreTutorialDialogue(2);

                        musicManager.StartMuffle();
                    }
                }
            }
        }
        else if (gameState == "PostTutorialDialogue")
        {
            if (kb.anyKey.wasPressedThisFrame)
            {
                currentCoroutine = null;
                gameState = "Practice";
                tutorialInstructionUI.SetActive(true);
                dialogueVolume.SetActive(false);
                playerDialogue.SetActive(false);

                musicManager.EndMuffle();
            }
        }
        else if (gameState == "Practice")
        {
            tutorialInstructionText.text = tutorialInstructionList[tutorialInstructionList.Length - 1];
            if (kb.escapeKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(2);
            }
        }
    }

    #region Tutorial Instruction Functions
    private void ShowInstruction(int index)
    {
        tutorialInstructionText.text = tutorialInstructionList[index]+"\n"+instructionTasksCompleted.ToString()+"/"+instructionTasksCompletedList[tutorialInstructionIndex];
    }

    private void ShowSpecialInstruction(int index)
    {
        tutorialInstructionText.text = tutorialInstructionList[index];
    }

    public void UpdateInstructionsCompleted()
    {
        instructionTasksCompleted++;
        ShowInstruction(tutorialInstructionIndex);

        if (instructionTasksCompleted >= instructionTasksCompletedList[tutorialInstructionIndex])
        {
            instructionTasksCompleted = 0;
            isNextInstruction = true;
        }
    }
    #endregion

    #region Dialogue
    private void ShowPreTutorialDialogue(int index)
    {
        playerDialogue.SetActive(false);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        string text = preTutorialDialogueList[index];
        playerDialogue.SetActive(true);
        currentCoroutine = StartCoroutine(DisplayPlayerText(text));
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
    #endregion

    #region Text Functions
    public void ShowFientText(Vector3 playerWorldPos)
    {
        GameObject fientTextUIInstance = Instantiate(fientTextUI, canvas.transform);

        fientTextUIInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, 145f);
        StartCoroutine(HideDamageText(fientTextUIInstance));
    }

    public void ShowBlockText(Vector3 playerWorldPos)
    {
        GameObject blockTextUIInstance = Instantiate(blockTextUI, canvas.transform);

        blockTextUIInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, 187.5f);
        StartCoroutine(HideDamageText(blockTextUIInstance));
    }

    public void ShowDamageText(Vector3 playerWorldPos, float damage)
    {
        GameObject damageTextUIInstance = Instantiate(damageTextUI, canvas.transform);
        damageTextUIInstance.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(damage).ToString();

        damageTextUIInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f,145f);
        StartCoroutine(HideDamageText(damageTextUIInstance));
    }

    private IEnumerator HideDamageText(GameObject damageTextUIInstance)
    {
        yield return new WaitForSeconds(showDamageTextDuration);
        Destroy(damageTextUIInstance);
    }

    public void ShowRiposteText(Vector3 playerWorldPos)
    {
        StopCoroutine(HideRiposteText());
        riposteTextUI.SetActive(true);
        riposteTextRectTransform.localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f,145f);
        StartCoroutine(HideRiposteText());
    }

    private IEnumerator HideRiposteText()
    {
        yield return new WaitForSeconds(showTextDuration);
        riposteTextUI.SetActive(false);
    }

    public void ShowCritText(Vector3 playerWorldPos)
    {
        GameObject critTextUIInstance = Instantiate(critTextUI, canvas.transform);

        critTextUIInstance.GetComponent<RectTransform>().localPosition = WorldToCanvasPos(playerWorldPos) + new Vector2(0f, 187.5f);
        StartCoroutine(HideDamageText(critTextUIInstance));
    }
    #endregion

    #region Particle Functions
    public void SpawnLeftBloodParticle(Vector3 WorldPos)
    {
        GameObject bloodParticle = Instantiate(leftBloodParticlePrefab, WorldPos, Quaternion.identity);
        StartCoroutine(DestroyBloodParticle(bloodParticle));
    }

    public void SpawnRightBloodParticle(Vector3 WorldPos)
    {
        GameObject bloodParticle = Instantiate(leftBloodParticlePrefab, WorldPos, Quaternion.identity);
        StartCoroutine(DestroyBloodParticle(bloodParticle));
    }

    private IEnumerator DestroyBloodParticle(GameObject particle)
    {
        yield return new WaitForSeconds(15f);
        Destroy(particle);
    }
    #endregion

    #region Postprocessing Functions
    public void ResetGetHitUI()
    {
        StartCoroutine(ResetGetHitUICoroutine());
    }

    private IEnumerator ResetGetHitUICoroutine()
    {
        yield return new WaitForSeconds(getHitVolumeShowDuration);
        getHitVolume.SetActive(false);
    }
    #endregion

    private Vector2 WorldToCanvasPos(Vector3 playerWorldPos)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerWorldPos);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, Camera.main, out canvasPosition);
        return canvasPosition;
    }
}

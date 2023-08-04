using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    #region Tutorial Manager Variables and Components
    public float minimumPlayerDummyDistance;

    public string gameState;

    public GameObject fientTextUI;
    public GameObject damageTextUI;
    public GameObject blockTextUI;
    public GameObject outOfStaminaTextUI;
    public GameObject riposteTextUI;
    public GameObject critTextUI;
    [SerializeField] private float showDamageTextDuration;
    [SerializeField] private float showTextDuration;
    private RectTransform riposteTextRectTransform;

    public GameObject canvas;
    private RectTransform canvasRectTransform;

    public GameObject leftBloodParticlePrefab;
    public GameObject rightBloodParticlePrefab;

    public GameObject getHitVolume;
    [SerializeField] private float getHitVolumeShowDuration;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        #region Initialize Variables
        gameState = "Fight";

        canvasRectTransform = canvas.GetComponent<RectTransform>();
        riposteTextRectTransform = riposteTextUI.GetComponent<RectTransform>();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

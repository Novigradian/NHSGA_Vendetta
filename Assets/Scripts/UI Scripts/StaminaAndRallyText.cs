using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaAndRallyText : MonoBehaviour
{
    private bool isShowingStaminaText;
    private bool isShowingRallyText;

    public Vector3 topTextPos;
    public Vector3 bottomTextPos;

    public GameObject staminaText;
    public GameObject rallyText;
    private RectTransform staminaTextRectTransform;
    private RectTransform rallyTextRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        isShowingRallyText = false;
        isShowingStaminaText = false;

        staminaTextRectTransform = staminaText.GetComponent<RectTransform>();
        rallyTextRectTransform = rallyText.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStaminaText()
    {
        if (!isShowingStaminaText)
        {
            staminaText.SetActive(true);
            isShowingStaminaText = true;
        }
        if (!isShowingRallyText)
        {
            staminaTextRectTransform.anchoredPosition = topTextPos;
        }
        else
        {
            staminaTextRectTransform.anchoredPosition = bottomTextPos;
        }
    }

    public void HideStaminaText()
    {
        if (isShowingStaminaText)
        {
            staminaText.SetActive(false);
            isShowingStaminaText = false;
        }
        if (isShowingRallyText && rallyText.transform.position==bottomTextPos)
        {
            rallyTextRectTransform.anchoredPosition = topTextPos;
        }
    }

    public void ShowRallyText()
    {
        if (!isShowingRallyText)
        {
            rallyText.SetActive(true);
            isShowingRallyText = true;
        }
        if (!isShowingStaminaText)
        {
            rallyTextRectTransform.anchoredPosition = topTextPos;
        }
        else
        {
            rallyTextRectTransform.anchoredPosition = bottomTextPos;
        }
    }

    public void HideRallyText()
    {
        if (isShowingRallyText)
        {
            rallyText.SetActive(false);
            isShowingRallyText = false;
        }
        if (isShowingStaminaText && staminaText.transform.position == bottomTextPos)
        {
            staminaTextRectTransform.anchoredPosition = topTextPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChangeButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text buttonText;

    private Color originalColor;

    public Color hoverColor;


    // Start is called before the first frame update
    void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        //Debug.Log("enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = originalColor;
        //Debug.Log("exit");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeavyLungeChargeBar : MonoBehaviour
{
    public Slider slider;

    public Color originalColor;
    public Color readyColor;
    void Start()
    {
        slider.value = 0f;
    }

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void ChangeColor()
    {
        transform.GetChild(0).gameObject.GetComponent<Image>().color = readyColor;
    }

    public void ResetColor()
    {
        transform.GetChild(0).gameObject.GetComponent<Image>().color = originalColor;
    }
}

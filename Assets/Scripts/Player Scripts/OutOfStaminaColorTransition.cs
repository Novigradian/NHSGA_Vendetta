using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfStaminaColorTransition : MonoBehaviour
{
    public Color normalColor;
    public Color outOfStaminaColor;
    public float frequency;
    public float amplitude;

    private SpriteRenderer spriteRenderer;

    public bool isOutOfStamina;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOutOfStamina)
        {
            float time = Time.time * frequency;
            float t = (Mathf.Sin(time) * amplitude) + amplitude;

            spriteRenderer.color = Color.Lerp(normalColor, outOfStaminaColor, t);
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }
}

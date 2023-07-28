using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashDistance;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float thrustSpeed;
    public GameObject sword;
    private Vector2 initPosition;
    bool isDashing = false;
    bool canDash = true;
    bool canSwing = true;
    bool isSwinging = false;
    private float swordDistance;
    [HideInInspector] public const int moveState = 1;
    [HideInInspector] public const int attackState = 2;
    [HideInInspector] public const int dashState = 3;
    private int direction = 1;
    [HideInInspector] public bool isFacingLeft = false;
    void Start()
    {
        initPosition = sword.transform.position;
        swordDistance = Vector3.Distance(transform.position, initPosition);
    }

    // Update is called once per frame
    void Update()
    {
        //checkState();
        checkMovement();
    }
    void flip()
    {
        isFacingLeft = !isFacingLeft;
        transform.Rotate(0f, 180f, 0f);
    }
    public void checkMovement()
    {
        Keyboard kb = Keyboard.current;
        if (kb.aKey.isPressed)
        {
            rb.position += Vector2.left * Time.deltaTime * playerSpeed;
            direction = -1;
            if (!isFacingLeft)
            {
                flip();
            }
        }
        if (kb.dKey.isPressed)
        {
            rb.position += Vector2.right * Time.deltaTime * playerSpeed;
            direction = 1;
            if (isFacingLeft)
            {
                flip();
            }
        }
        if (kb.shiftKey.wasPressedThisFrame && !isDashing && canDash)
        {
            StartCoroutine(Dash());
        }
        if (kb.oKey.wasPressedThisFrame && canSwing && !isSwinging)
        {
            StartCoroutine(Swing());
        }
        if (!isSwinging && !kb.oKey.wasPressedThisFrame)
        {
            sword.GetComponent<Rigidbody2D>().position = new Vector2(gameObject.transform.position.x + swordDistance * direction, gameObject.transform.position.y + 1);
        }
        if (kb.spaceKey.wasPressedThisFrame)
        {

        }
    }
    public IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        if (direction == 1)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashDistance, 0);
        }
        if (direction == -1)
        {
            rb.velocity = new Vector2(transform.localScale.x * -dashDistance, 0);
        }
        yield return new WaitForSeconds(0.5f);
        isDashing = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }
    public IEnumerator Swing()
    {
        sword.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * thrustSpeed, ForceMode2D.Impulse);
        isSwinging = true;
        canSwing = false;
        yield return new WaitForSeconds(0.1f);
        sword.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        sword.GetComponent<Rigidbody2D>().position = new Vector2(gameObject.transform.position.x + swordDistance * direction, gameObject.transform.position.y + 1);
        isSwinging = false;
        yield return new WaitForSeconds(0.3f);
        canSwing = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashDistance;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float lungeSpeed;
    [SerializeField] private float rallyTimer;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int enemyAttackValue;

    [HideInInspector] public bool rallyOn = false;

    [HideInInspector] public int currentStamina;
    [HideInInspector] public int staminaCost;

    public float rallyPercentage;
    public int maxHealth;
    public int maxStamina;
    public int recoverySpeed;
    public PlayerHealthBar healthBar;
    public PlayerStaminaBar staminaBar;
    public GameObject sword;
    private Vector2 initPosition;
    private float timer;
    private Coroutine recoverStamina;
    public enemy enemy;
    //bool isDashing = false;
    //bool canDash = true;
    bool canInput = true;
    bool canSwing = true;
    bool isStunned = false;
    bool isSwinging = false;
    bool canJump = true;
    bool isJumping = false;
    bool canMove = true;
    //bool canBlock = false;
    private float swordDistance;
    private int direction = 1;
    //[HideInInspector] public bool isFacingEnemy = true;
    void Start()
    {
        initPosition = sword.transform.position;
        swordDistance = Vector3.Distance(transform.position, initPosition);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        enemyAttackValue = 10;
        timer = rallyTimer;
    }


    void Update()
    {
        //checkState();
        CheckStun();
        if(canInput) CheckMovement();
        CheckHealth();
        //checkDirection();
        CheckRally();
    }
    //private void FixedUpdate()
    //{
    //    checkDirection();
    //}
    //void flip()
    //{
    //    isFacingEnemy = !isFacingEnemy;
    //    transform.Rotate(0f, 180f, 0f);
    //}
    //public void checkDirection()
    //{
    //    RaycastHit2D hitEnemy = Physics2D.Raycast(transform.position, Vector2.left * direction);
    //    if (hitEnemy && hitEnemy.transform.tag == "enemy")
    //    {
    //        isFacingEnemy = false;
    //    }
    //    Debug.DrawRay(transform.position, Vector2.right * direction * 100, color: Color.red);
    //    Debug.Log(hitEnemy);
    //    if(transform.position.x - enemy.transform.position.x < 0)
    //    {
    //        direction = 1;
    //    }
    //    else
    //    {
    //        direction = -1;
    //    }
    //}
    public void CheckMovement()
    {
        //canBlock = false;
        Keyboard kb = Keyboard.current;
        if (kb.aKey.isPressed && canMove)
        {
            rb.position += Vector2.left * Time.deltaTime * playerSpeed;
            //canBlock = true;
        }
        //if (!isFacingEnemy)
        //{
        //    flip();
        //}
        if (kb.dKey.isPressed && canMove)
        {
            rb.position += Vector2.right * Time.deltaTime * playerSpeed;
            
        }
        //if (kb.shiftKey.wasPressedThisFrame && !isDashing && canDash)
        //{
        //    StartCoroutine(Dash());
        //}
        if (kb.oKey.wasPressedThisFrame && canSwing && !isSwinging && currentStamina >= 5)
        {
            if (isJumping)
            {
                enemy.playerAttackValue = 15;
            }
            else
            {
                enemy.playerAttackValue = 10;
            }
            UseStamina(5);
            StartCoroutine(Swing());
        }
        if (kb.pKey.wasPressedThisFrame && canSwing && !isSwinging && canJump && !isJumping && currentStamina >= 10)
        {
            enemy.playerAttackValue = 20;
            UseStamina(10);
            StartCoroutine(HeavySwing());
            
        }
        if (!isSwinging && !kb.oKey.wasPressedThisFrame)
        {
            sword.GetComponent<Rigidbody2D>().position = new Vector2(gameObject.transform.position.x + swordDistance * direction, gameObject.transform.position.y + 1);
        }
        if (kb.spaceKey.wasPressedThisFrame && !isJumping && canJump && currentStamina >= 5)
        {
            UseStamina(5);
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            //Debug.Log(rb.velocity);
            canJump = false;
            isJumping = true;
            
        }
    }

    public void UseStamina(int cost)
    {
        currentStamina -= cost;
        staminaBar.SetStamina(currentStamina);

        if(recoverStamina != null)
        {
            StopCoroutine(recoverStamina);
        }
        
        recoverStamina = StartCoroutine(RecoverStamina());
    }

    public void CheckStun()
    {
        if (isStunned)
        {
            StartCoroutine(Stunned());
        }
    }

    public IEnumerator Stunned()
    {
        canInput = false;
        yield return new WaitForSeconds(0.5f);
        canInput = true;
        isStunned = false;
    }
    //public IEnumerator Dash()
    //{
    //    isDashing = true;
    //    canDash = false;
    //    if (direction == 1)
    //    {
    //        rb.velocity = new Vector2(transform.localScale.x * dashDistance, 0);
    //    }
    //    if (direction == -1)
    //    {
    //        rb.velocity = new Vector2(transform.localScale.x * -dashDistance, 0);
    //    }
    //    yield return new WaitForSeconds(0.5f);
    //    isDashing = false;
    //    yield return new WaitForSeconds(0.5f);
    //    canDash = true;
    //}
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
    public IEnumerator HeavySwing()
    {
        rb.velocity = new Vector2(0, 0);
        canMove = false;
        canJump = false;
        //canDash = false;
        rb.velocity = new Vector2(-3 * direction, 0);
        yield return new WaitForSeconds(0.5f);
        sword.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * lungeSpeed * .75f, ForceMode2D.Impulse);
        rb.AddForce(gameObject.transform.right * lungeSpeed, ForceMode2D.Impulse);
        isSwinging = true;
        canSwing = false;
        yield return new WaitForSeconds(.48f);
        sword.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(.5f);
        sword.GetComponent<Rigidbody2D>().position = new Vector2(gameObject.transform.position.x + swordDistance * direction, gameObject.transform.position.y + 1);
        isSwinging = false;
        canSwing = true;
        canMove = true;
        canJump = true;
        //canDash = true;
    }

    public IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(1f);
        while (currentStamina < maxStamina)
        {
            currentStamina += recoverySpeed;
            staminaBar.SetStamina(currentStamina);
            yield return new WaitForSeconds(0.1f);
        }
        recoverStamina = null;
    }
    
    public void CheckRally()
    {
        if (rallyOn)
        {
            timer -= Time.deltaTime;
            //Debug.Log(timer);
        }
        if (timer <= 0)
        {
            rallyOn = false;
            timer = rallyTimer;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground"))
        {
            canJump = true;
            isJumping = false;
        }
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemy sword"))
        {
            currentHealth = currentHealth - enemyAttackValue;
            healthBar.SetHealth(currentHealth);
            //Debug.Log(currentHealth);
            //Debug.Log(enemyAttackValue);
            rallyOn = true;
            isStunned = true;
        }
    }
    public void CheckHealth()
    {
        if(currentHealth == 0)
        {
            Time.timeScale = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Player States
    public enum PlayerState
    {
        idle,
        stepLeft, stepRight, shuffleLeft, shuffleRight,
        jump, jumpAttack,
        lightAttackWindup, lightAttack,
        heavyLungeWindup, heavyLunge, heavyLungeStun,
        parry,
        getHit,
    }

    [SerializeField] PlayerState state;
    #endregion

    #region Player Variables and Components
    Keyboard kb;

    [HideInInspector] public bool isFacingLeft;

    private Rigidbody2D rb;
    public GameObject sword;
    private Rigidbody2D swordRb;
    private Transform swordPivot;

    [Header("Movement")]
    [SerializeField] private float stepSpeed;
    [SerializeField] private float stepDuration;
    [SerializeField] private float stepCooldown;
    [SerializeField] private float shuffleSpeed;
    [SerializeField] private bool canShift;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHorizontalSpeed;

    [Header("Light Attack")]
    
    [SerializeField] private float lightAttackWindupDuration;
    [SerializeField] private float lightAttackWindupSpeed;
    [SerializeField] private float lightAttackDuration;
    [SerializeField] private float lightAttackThrustSpeed;

    [Header("Heavy Attack")]
    [SerializeField] private float heavyLungeMinimumWindupTime;
    [SerializeField] private float heavyLungeMaximumWindupTime;
    [SerializeField] private float heavyLungeWindupSpeed;
    [SerializeField] private float heavyLungeWindupThrustScale;
    private float heavyLungeWindupTime;
    private float heavyLungeThrustTime;
    [SerializeField] private float heavyLungeLowerSwordScale;
    [SerializeField] private float heavyLungeThrustSpeed;
    [SerializeField] private float heavyLungeStunDuration;

    [Header("Jump Attack")]
    [SerializeField] private float jumpAttackDuration;
    [SerializeField] private float jumpAttackSwingSpeed;
    [SerializeField] private Vector2 jumpAttackThrustSpeed;

    private float direction;
    #region Timers
    private float stepLeftTimer;
    private float stepRightTimer;
    #endregion
    #endregion

    // Start is called before the first frame update
    [Header("Old stuff")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float jumpSpeed;
    private Vector2 initPosition;
    bool isDashing = false;
    bool isJumping = false;
    bool canDash = true;
    bool canSwing = true;
    bool isSwinging = false;
    bool canJump = true;
    private float swordDistance;
    [HideInInspector] public const int moveState = 1;
    [HideInInspector] public const int attackState = 2;
    [HideInInspector] public const int dashState = 3;

    [SerializeField] private float lungeSpeed;
    bool canMove = true;
    void Start()
    {
        //initPosition = sword.transform.position;
        //swordDistance = Vector3.Distance(transform.position, initPosition);

        #region Initialize Variables
        kb = Keyboard.current;
        rb = gameObject.GetComponent<Rigidbody2D>();
        swordRb = sword.GetComponent<Rigidbody2D>();
        swordPivot = sword.transform.parent;

        isFacingLeft = false;

        canShift = true;

        direction = 1f;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        //checkState();
        //checkMovement();

        //Debug.Log(state);

        switch (state)
        {
            #region Idle Actions and Transitions
            case PlayerState.idle:
                IdleActions();
                IdleTransitions();
                break;
            #endregion

            #region Movement Actions and Transitions
            case PlayerState.stepLeft:
                StepLeftActions();
                StepLeftTransitions();
                break;
            case PlayerState.stepRight:
                StepRightActions();
                StepRightTransitions();
                break;
            case PlayerState.shuffleLeft:
                ShuffleLeftActions();
                ShuffleLeftTransitions();
                break;
            case PlayerState.shuffleRight:
                ShuffleRightActions();
                ShuffleRightTransitions();
                break;
            #endregion

            #region Jump Actions and Transitions
            case PlayerState.jump:
                JumpActions();
                JumpTransitions();
                break;
            case PlayerState.jumpAttack:
                JumpAttackActions();
                JumpAttackTransitions();
                break;
            #endregion

            #region Light Attack Actions and Transitions
            case PlayerState.lightAttackWindup:
                LightAttackWindupActions();
                LightAtackWindupTransitions();
                break;
            case PlayerState.lightAttack:
                LightAttackActions();
                LightAttackTransitions();
                break;
            #endregion

            #region Heavy Attack Actions and Transitions
            case PlayerState.heavyLungeWindup:
                HeavyLungeWindupActions();
                HeavyLungeWindupTransitions();
                break;
            case PlayerState.heavyLunge:
                HeavyLungeActions();
                HeavyLungeTransitions();
                break;
            case PlayerState.heavyLungeStun:
                HeavyLungeStunActions();
                HeavyLungeStunTransitions();
                break;
            #endregion

            #region Parry Actions and Transitions
            case PlayerState.parry:
                ParryActions();
                ParryTransitions();
                break;
            #endregion

            #region Get Hit Actions and Transitions
            case PlayerState.getHit:
                GetHitActions();
                GetHitTransitions();
                break;
                #endregion
        }
    }

    #region Idle Functions
    private void IdleActions()
    {

    }

    private void IdleTransitions()
    {
        if (kb.aKey.isPressed) //Go Left
        {
            if (kb.sKey.isPressed && canShift)
            {
                state = PlayerState.stepLeft;
                //rb.AddForce(Vector2.left * stepDistance);

                stepLeftTimer = 0f;
            }
            else
            {
                state = PlayerState.shuffleLeft;
            }

            /*
            if (!isFacingLeft)
            {
                Flip();
            }
            */
        }
        else if (kb.dKey.isPressed) //Go Right
        {
            if (kb.sKey.isPressed && canShift)
            {
                state = PlayerState.stepRight;
                //rb.AddForce(Vector2.right * stepDistance);

                stepRightTimer = 0f;
            }
            else
            {
                state = PlayerState.shuffleRight;
            }
            
            /*
            if (isFacingLeft)
            {
                Flip();
            }
            */
        }
        else if (kb.spaceKey.isPressed)
        {
            state = PlayerState.jump;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (kb.oKey.wasPressedThisFrame)
        {
            state = PlayerState.lightAttackWindup;
        }
        else if (kb.pKey.wasPressedThisFrame)
        {
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
        }
    }
    #endregion

    #region Movement Functions
    private void StepLeftActions()
    {
        rb.position += Vector2.left * Time.deltaTime * stepSpeed;
    }

    private void StepLeftTransitions()
    {
        /*
        if (kb.dKey.wasPressedThisFrame) //Step Right
        {
            state = PlayerState.stepRight;
            //rb.AddForce(Vector2.right * stepDistance);

            stepRightTimer = 0f;

            if (isFacingLeft)
            {
                Flip();
            }
        }

        if (kb.spaceKey.isPressed)
        {
            state = PlayerState.jump;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        */

        stepLeftTimer += Time.deltaTime;
        //Debug.Log(stepLeftTimer);
        if (stepLeftTimer >= stepDuration)
        {
            state = PlayerState.idle;
            canShift = false;
            StartCoroutine(ResetCanShift());
        }
    }

    private void StepRightActions()
    {
        rb.position += Vector2.right * Time.deltaTime * stepSpeed;
    }

    private void StepRightTransitions()
    {
        /*
        if (kb.aKey.wasPressedThisFrame) //Step Left
        {
            state = PlayerState.stepLeft;
            //rb.AddForce(Vector2.left * stepDistance);

            stepLeftTimer = 0f;

            if (!isFacingLeft)
            {
                Flip();
            }
            //rb.position += Vector2.left * Time.deltaTime * playerSpeed;
            //direction = -1;
        }
        if (kb.spaceKey.isPressed)
        {
            state = PlayerState.jump;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        */

        stepRightTimer += Time.deltaTime;
        //Debug.Log(stepRightTimer);
        if (stepRightTimer >= stepDuration)
        {
            state = PlayerState.idle;
            canShift = false;
            StartCoroutine(ResetCanShift());
        }
    }
    
    private void ShuffleLeftActions()
    {
        rb.position += Vector2.left * Time.deltaTime * shuffleSpeed;
    }

    private void ShuffleLeftTransitions()
    {
        if (kb.oKey.wasPressedThisFrame)
        {
            state = PlayerState.lightAttackWindup;
        }
        else if (kb.pKey.wasPressedThisFrame)
        {
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
        }
        else if (kb.spaceKey.isPressed)
        {
            state = PlayerState.jump;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (kb.aKey.wasReleasedThisFrame)
        {
            state = PlayerState.idle;
        }
        else if (kb.dKey.isPressed)
        {
            if (kb.sKey.isPressed && canShift)
            {
                state = PlayerState.stepRight;

                stepRightTimer = 0f;
            }
            else
            {
                state = PlayerState.shuffleRight;
            }

            /*
            if (isFacingLeft)
            {
                Flip();
            }
            */
        }
        else if (kb.sKey.isPressed && canShift)
        {
            state = PlayerState.stepLeft;

            stepLeftTimer = 0f;
        }
    }

    private void ShuffleRightActions()
    {
        rb.position += Vector2.right * Time.deltaTime * shuffleSpeed;
    }

    private void ShuffleRightTransitions()
    {
        if (kb.oKey.wasPressedThisFrame)
        {
            state = PlayerState.lightAttackWindup;
        }
        else if (kb.pKey.wasPressedThisFrame)
        {
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
        }
        else if (kb.spaceKey.isPressed)
        {
            state = PlayerState.jump;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (kb.dKey.wasReleasedThisFrame)
        {
            state = PlayerState.idle;
        }
        else if (kb.aKey.isPressed)
        {
            if (kb.sKey.isPressed && canShift)
            {
                state = PlayerState.stepLeft;

                stepLeftTimer = 0f;
            }
            else
            {
                state = PlayerState.shuffleLeft;
            }

            /*
            if (!isFacingLeft)
            {
                Flip();
            }
            */
        }
        else if (kb.sKey.isPressed && canShift)
        {
            state = PlayerState.stepRight;

            stepRightTimer = 0f;
        }
    }

    private IEnumerator ResetCanShift()
    {
        yield return new WaitForSeconds(stepCooldown);
        canShift = true;
    }
    #endregion

    #region Jump Functions
    private void JumpActions()
    {
        if (kb.dKey.isPressed)
        {
            rb.position += Vector2.right * Time.deltaTime * jumpHorizontalSpeed;
        }
        else if (kb.aKey.isPressed)
        {
            rb.position += Vector2.left * Time.deltaTime * jumpHorizontalSpeed;
        }
    }

    private void JumpTransitions()
    {
        if (kb.oKey.wasPressedThisFrame)
        {
            state = PlayerState.jumpAttack;
            swordRb.isKinematic = false;
        }
    }

    private void JumpAttackActions()
    {
        swordPivot.localEulerAngles -= new Vector3(0f, 0f, jumpAttackSwingSpeed);
        swordRb.position += jumpAttackThrustSpeed * direction * Time.deltaTime;
    }

    private void JumpAttackTransitions()
    {
        StartCoroutine(JumpAttack());
    }

    private IEnumerator JumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackDuration);
        if (state != PlayerState.idle)
        {
            ResetSwordPosition();
            swordRb.isKinematic = true;
            state = PlayerState.jump;
        }
        
    }
    #endregion

    #region Light Attack Functions
    private void LightAttackWindupActions()
    {
        swordRb.position += Vector2.right * -direction * Time.deltaTime * lightAttackWindupSpeed;
    }

    private void LightAtackWindupTransitions()
    {
        StartCoroutine(LightAttackWindup());
    }

    private void LightAttackActions()
    {
        swordRb.position += Vector2.right * direction * Time.deltaTime * lightAttackThrustSpeed;
    }

    private void LightAttackTransitions()
    {
        StartCoroutine(LightAttack());
    }

    private IEnumerator LightAttackWindup()
    {
        swordRb.isKinematic = false;
        yield return new WaitForSeconds(lightAttackWindupDuration);
        state = PlayerState.lightAttack;
    }

    private IEnumerator LightAttack()
    {
        yield return new WaitForSeconds(lightAttackDuration);
        swordRb.isKinematic = true;
        ResetSwordPosition();
        state = PlayerState.idle;
    }

    #endregion

    #region Heavy Attack Functions
    private void HeavyLungeWindupActions()
    {
        if (heavyLungeWindupTime <= heavyLungeMaximumWindupTime)
        {
            heavyLungeWindupTime += Time.deltaTime;
            swordRb.position += Vector2.right * -direction * Time.deltaTime * heavyLungeWindupSpeed;
        }
    }

    private void HeavyLungeWindupTransitions()
    {
        
        if (kb.pKey.wasReleasedThisFrame)
        {
            if (heavyLungeWindupTime >= heavyLungeMinimumWindupTime)
            {
                state = PlayerState.heavyLunge;
                swordRb.position += Vector2.down*heavyLungeLowerSwordScale;
                heavyLungeThrustTime = heavyLungeWindupTime * heavyLungeWindupThrustScale;
                heavyLungeThrustSpeed+= heavyLungeWindupTime * heavyLungeWindupThrustScale;
            }
            else
            {
                state = PlayerState.idle;
                ResetSwordPosition();
            }
        }
    }

    private void HeavyLungeActions()
    {
        swordRb.position += Vector2.right * direction * Time.deltaTime * heavyLungeThrustSpeed;
    }

    private void HeavyLungeTransitions()
    {
        StartCoroutine(HeavyLunge());
    }

    private IEnumerator HeavyLunge()
    {
        yield return new WaitForSeconds(heavyLungeThrustTime);
        state = PlayerState.heavyLungeStun;
        ResetSwordPosition();
        swordRb.isKinematic = true;
    }

    private void HeavyLungeStunActions()
    {

    }

    private void HeavyLungeStunTransitions()
    {
        StartCoroutine(HeavyLungeStun());
    }

    private IEnumerator HeavyLungeStun()
    {
        yield return new WaitForSeconds(heavyLungeStunDuration);
        state = PlayerState.idle;
    }
    #endregion

    #region Parry Functions
    private void ParryActions()
    {

    }

    private void ParryTransitions()
    {

    }
    #endregion

    #region Get Hit Functions
    private void GetHitActions()
    {

    }

    private void GetHitTransitions()
    {

    }
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            state = PlayerState.idle;
            ResetSwordPosition();
            swordRb.isKinematic = true;
        }
    }
    #endregion
    void Flip()
    {
        isFacingLeft = !isFacingLeft;
        if (isFacingLeft)
        {
            direction = -1f;
        }
        else
        {
            direction = 1f;
        }
        transform.Rotate(0f, 180f, 0f);
    }

    private void ResetSwordPosition()
    {
        sword.transform.position = new Vector3(transform.position.x + 2f, transform.position.y + 1f, transform.position.z);
        sword.transform.localEulerAngles = new Vector3(0f, 0f, -75f);
        swordPivot.localEulerAngles = Vector3.zero;
    }
    public void checkMovement()
    {
        if (kb.aKey.isPressed)
        {
            rb.position += Vector2.left * Time.deltaTime * stepSpeed;
            direction = -1;
            if (!isFacingLeft)
            {
                Flip();
            }
        }
        if (kb.dKey.isPressed)
        {
            rb.position += Vector2.right * Time.deltaTime * stepSpeed;
            direction = 1;
            if (isFacingLeft)
            {
                Flip();
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
        if (kb.pKey.wasPressedThisFrame && canSwing && !isSwinging && canJump)
        {
            StartCoroutine(HeavySwing());
        }
        if (kb.spaceKey.wasPressedThisFrame && !isJumping && canJump)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            canJump = false;
            isJumping = true;
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

    public IEnumerator HeavySwing()
    {
        rb.velocity = new Vector2(0, 0);
        canMove = false;
        canJump = false;
        canDash = false;
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
        canDash = true;
    }

    
}
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
        dashWindup, dash,
        parry,
        getHit,
    }

    [SerializeField] PlayerState state;
    #endregion

    #region Player Variables and Components
    Keyboard kb;

    [HideInInspector] public bool isFacingLeft;

    private Rigidbody2D rb;

    [SerializeField] private float stepSpeed;
    [SerializeField] private float stepDuration;
    [SerializeField] private float stepCooldown;
    [SerializeField] private float shuffleSpeed;

    [SerializeField] private bool canShift;

    //[SerializeField] private float stepDistance;

    #region Timers
    private float stepLeftTimer;
    private float stepRightTimer;
    #endregion
    #endregion

    // Start is called before the first frame update
    [SerializeField] private float dashDistance;
    [SerializeField] private float thrustSpeed;
    [SerializeField] private float jumpSpeed;
    public GameObject sword;
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
    private int direction = 1;

    [SerializeField] private float lungeSpeed;
    bool canMove = true;
    void Start()
    {
        initPosition = sword.transform.position;
        swordDistance = Vector3.Distance(transform.position, initPosition);

        #region Initialize Variables
        kb = Keyboard.current;
        rb = gameObject.GetComponent<Rigidbody2D>();

        isFacingLeft = false;

        canShift = true;

        
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
            case PlayerState.dash:
                DashActions();
                DashTransitions();
                break;
            case PlayerState.dashWindup:
                DashWindupActions();
                DashWindupTransitions();
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

            if (!isFacingLeft)
            {
                Flip();
            }
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
            
            if (isFacingLeft)
            {
                Flip();
            }
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
        */

        stepLeftTimer += Time.deltaTime;
        //Debug.Log(stepLeftTimer);
        if (stepLeftTimer >= 0.15f)
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
        */

        stepRightTimer += Time.deltaTime;
        //Debug.Log(stepRightTimer);
        if (stepRightTimer >= 0.15f)
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
        if (kb.aKey.wasReleasedThisFrame)
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

            if (isFacingLeft)
            {
                Flip();
            }
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
        if (kb.dKey.wasReleasedThisFrame)
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

            if (!isFacingLeft)
            {
                Flip();
            }
        }
        else if (kb.sKey.isPressed && canShift)
        {
            state = PlayerState.stepRight;

            stepRightTimer = 0f;
        }
    }

    private void DashWindupActions()
    {

    }

    private void DashWindupTransitions()
    {

    }

    private void DashActions()
    {

    }

    private void DashTransitions()
    {

    }
    #endregion

    #region Jump Functions
    private void JumpActions()
    {

    }

    private void JumpTransitions()
    {

    }

    private void JumpAttackActions()
    {

    }

    private void JumpAttackTransitions()
    {

    }
    #endregion

    #region Light Attack Functions
    private void LightAttackWindupActions()
    {

    }

    private void LightAtackWindupTransitions()
    {

    }

    private void LightAttackActions()
    {

    }

    private void LightAttackTransitions()
    {

    }
    #endregion

    #region Heavy Attack Functions
    private void HeavyLungeWindupActions()
    {

    }

    private void HeavyLungeWindupTransitions()
    {

    }

    private void HeavyLungeActions()
    {

    }

    private void HeavyLungeTransitions()
    {

    }

    private void HeavyLungeStunActions()
    {

    }

    private void HeavyLungeStunTransitions()
    {

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

    void Flip()
    {
        isFacingLeft = !isFacingLeft;
        transform.Rotate(0f, 180f, 0f);
    }

    private IEnumerator ResetCanShift()
    {
        yield return new WaitForSeconds(stepCooldown);
        canShift = true;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
            isJumping = false;
        }
    }
}
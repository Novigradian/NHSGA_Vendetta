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
        getHit
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
    private Collider2D swordCollider;

    [Header("Health")]
    public PlayerHealthBar playerHealthBar;
    [SerializeField] private float maxPlayerHealth;
    private float playerHealth;

    [Header("Stamina")]
    public PlayerStaminaBar playerStaminaBar;
    [SerializeField] private float maxPlayerStamina;
    [SerializeField] private float playerStaminaRecoverySpeed;
    private float playerStamina;
    private Coroutine recoverStamina;
    private bool isOutOfStamina;
    [SerializeField] private float playerStaminaRecoveryDelay;

    [Header("Stamina Cost")]
    [SerializeField] private float playerStepStaminaCost;
    [SerializeField] private float playerJumpStaminaCost;
    [SerializeField] private float playerJumpAttackStaminaCost;
    [SerializeField] private float playerLightAttackStaminaCost;
    [SerializeField] private float playerHeavyLungeBaseStaminaCost;
    [SerializeField] private float playerHeavyLungeExtraStaminaCostScale;

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

    [Header("Parry")]
    [SerializeField] private float parryDuration;

    private float direction;
    #region Timers
    private float stepLeftTimer;
    private float stepRightTimer;
    #endregion
    #endregion
    void Start()
    {
        #region Initialize Variables
        kb = Keyboard.current;
        rb = gameObject.GetComponent<Rigidbody2D>();

        playerHealth = maxPlayerHealth;
        playerStamina = maxPlayerStamina;
        playerHealthBar.SetMaxHealth(maxPlayerHealth);
        playerStaminaBar.SetMaxStamina(maxPlayerStamina);
        isOutOfStamina = false;

        swordRb = sword.GetComponent<Rigidbody2D>();
        swordCollider = sword.GetComponent<BoxCollider2D>();
        swordCollider.enabled = false;
        swordPivot = sword.transform.parent;

        isFacingLeft = false;

        canShift = true;

        direction = 1f;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

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
            if (kb.sKey.isPressed && canShift &&!isOutOfStamina)
            {
                state = PlayerState.stepLeft;
                UseStamina(playerStepStaminaCost);
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
            if (kb.sKey.isPressed && canShift && !isOutOfStamina)
            {
                state = PlayerState.stepRight;
                UseStamina(playerStepStaminaCost);
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
        else if (kb.spaceKey.isPressed && !isOutOfStamina)
        {
            state = PlayerState.jump;
            UseStamina(playerJumpStaminaCost);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (kb.oKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.lightAttackWindup;
        }
        else if (kb.pKey.wasPressedThisFrame && !isOutOfStamina)
        {
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
        }
        else if (kb.iKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.parry;
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
        if (kb.oKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.lightAttackWindup;
        }
        else if (kb.pKey.wasPressedThisFrame && !isOutOfStamina)
        {
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
        }
        else if (kb.iKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.parry;
        }
        else if (kb.spaceKey.isPressed && !isOutOfStamina)
        {
            state = PlayerState.jump;
            UseStamina(playerJumpStaminaCost);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (kb.aKey.wasReleasedThisFrame)
        {
            state = PlayerState.idle;
        }
        else if (kb.dKey.isPressed)
        {
            if (kb.sKey.isPressed && canShift && !isOutOfStamina)
            {
                state = PlayerState.stepRight;
                UseStamina(playerStepStaminaCost);

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
        else if (kb.sKey.isPressed && canShift && !isOutOfStamina)
        {
            state = PlayerState.stepLeft;
            UseStamina(playerStepStaminaCost);

            stepLeftTimer = 0f;
        }
    }

    private void ShuffleRightActions()
    {
        rb.position += Vector2.right * Time.deltaTime * shuffleSpeed;
    }

    private void ShuffleRightTransitions()
    {
        if (kb.oKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.lightAttackWindup;
        }
        else if (kb.pKey.wasPressedThisFrame && !isOutOfStamina)
        {
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
        }
        else if (kb.iKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.parry;
        }
        else if (kb.spaceKey.isPressed && !isOutOfStamina)
        {
            state = PlayerState.jump;
            UseStamina(playerJumpStaminaCost);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (kb.dKey.wasReleasedThisFrame)
        {
            state = PlayerState.idle;
        }
        else if (kb.aKey.isPressed)
        {
            if (kb.sKey.isPressed && canShift && !isOutOfStamina)
            {
                state = PlayerState.stepLeft;
                UseStamina(playerStepStaminaCost);

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
        else if (kb.sKey.isPressed && canShift && !isOutOfStamina)
        {
            state = PlayerState.stepRight;
            UseStamina(playerStepStaminaCost);

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
        if (kb.oKey.wasPressedThisFrame && !isOutOfStamina)
        {
            state = PlayerState.jumpAttack;
            UseStamina(playerJumpAttackStaminaCost);
            swordRb.isKinematic = false;
            swordCollider.enabled = true;
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
        if (state != PlayerState.idle && state==PlayerState.jumpAttack)
        {
            ResetSwordPosition();
            swordRb.isKinematic = true;
            state = PlayerState.jump;
            swordCollider.enabled = false;
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
        if (state == PlayerState.lightAttackWindup)
        {
            UseStamina(playerLightAttackStaminaCost);
            Debug.Log("used stamina, stamina remaining" + playerStamina);
            state = PlayerState.lightAttack;
            swordCollider.enabled = true;
        }
    }

    private IEnumerator LightAttack()
    {
        yield return new WaitForSeconds(lightAttackDuration);
        if (state == PlayerState.lightAttack)
        {
            swordRb.isKinematic = true;
            swordCollider.enabled = false;
            ResetSwordPosition();
            state = PlayerState.idle;
        }
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
                swordCollider.enabled = true;
                state = PlayerState.heavyLunge;
                UseStamina(playerHeavyLungeBaseStaminaCost+heavyLungeWindupTime*playerHeavyLungeExtraStaminaCostScale);
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
        if (state == PlayerState.heavyLunge)
        {
            state = PlayerState.heavyLungeStun;
            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordCollider.enabled = false;
        }
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
        if (state == PlayerState.heavyLungeStun)
        {
            state = PlayerState.idle;
        }
    }
    #endregion

    #region Parry Functions
    private void ParryActions()
    {

    }

    private void ParryTransitions()
    {
        StartCoroutine(Parry());
    }

    private IEnumerator Parry()
    {
        yield return new WaitForSeconds(parryDuration);
        if (state == PlayerState.parry)
        {
            state = PlayerState.idle;
        }
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

    #region Stamina
    private void UseStamina(float cost)
    {
        playerStamina -= cost;
        playerStaminaBar.SetStamina(playerStamina);

        if (recoverStamina != null)
        {
            StopCoroutine(recoverStamina);
        }

        recoverStamina = StartCoroutine(RecoverStamina());

        if (playerStamina <= 0f)
        {
            playerStamina = 0f;
            isOutOfStamina = true;
        }
    }
    private IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(playerStaminaRecoveryDelay);
        isOutOfStamina = false;
        while (playerStamina < maxPlayerStamina)
        {
            playerStamina += playerStaminaRecoverySpeed;
            playerStaminaBar.SetStamina(playerStamina);
            yield return new WaitForSeconds(0.025f);
        }
        playerStamina = maxPlayerStamina;
        playerStaminaBar.SetStamina(maxPlayerStamina);
        recoverStamina = null;
    }
    #endregion

    #region Health
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemySword")
        {
            //Hit by any attack
            if (state == PlayerState.idle || state == PlayerState.shuffleRight || state == PlayerState.stepLeft || state == PlayerState.stepRight || state == PlayerState.lightAttackWindup || state == PlayerState.heavyLungeWindup || state == PlayerState.heavyLungeStun || state == PlayerState.jump || state == PlayerState.lightAttack)
            {

            }
            //Block
            else if (state==PlayerState.shuffleLeft)
            {

            }
            //Hit by anything except heavy lunge
            else if (state == PlayerState.jumpAttack)
            {

            }
            //Parry, if light attack gain riposte
            else if (state == PlayerState.parry)
            {

            }
            //Hit by jump flick
            else if (state == PlayerState.heavyLunge)
            {

            }
            //just take damage
            else if (state == PlayerState.getHit)
            {

            }
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
}
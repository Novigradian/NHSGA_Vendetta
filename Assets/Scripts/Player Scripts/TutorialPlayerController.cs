using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPlayerController : MonoBehaviour
{
    #region Player States
    public enum PlayerState
    {
        idle,
        stepLeft, stepRight, shuffleLeft, shuffleRight,
        jump, jumpAttack,
        lightAttackWindup, lightAttack,
        heavyLungeWindup, heavyLunge, heavyLungeStun,
        cancel,
        parry,
        parried,
        block,
        getHit,
        dead
    }

    public PlayerState state;
    #endregion

    #region Player Variables and Components
    Keyboard kb;

    [HideInInspector] public bool isFacingLeft;
    public Animator animator;
    private Rigidbody2D rb;
    public GameObject sword;
    private Rigidbody2D swordRb;
    private Transform swordPivot;
    private Rigidbody2D swordPivotRb;
    private Collider2D swordCollider;
    private bool hasPlayed = false;

    //public GameManager gameManager;
    //public DialogueManager dialogueManager;
    //public UIManager UIManager;
    //private float minimumPlayerEnemyDistance;

    public TutorialManager tutorialManager;
    private float minimumPlayerDummyDistance;

    public Transform controllerTransform;
    public AudioManager audioManager;

    [Header("Misc UI")]
    public PlayerHeavyLungeChargeBar playerHeavyLungeChargeBar;

    [Header("Misc Particles")]
    public ParticleSystem stepLeftDustParticle;
    public ParticleSystem stepRightDustParticle;

    [Header("Health")]
    public PlayerHealthBar playerHealthBar;
    [SerializeField] private float maxPlayerHealth;
    private float playerHealth;

    [Header("Player Damage")]
    public float playerLightAttackDamage;
    [HideInInspector] public float playerLightAttackBaseDamage;
    public float playerHeavyLungeBaseDamage;
    public float playerHeavyLungeExtraDamageScale;
    [HideInInspector] public float playerHeavyLungeDamage;
    public float playerJumpAttackDamage;

    [HideInInspector] public float currentDamageValue;

    [Header("Rally")]
    [SerializeField] private float rallyScale;
    [SerializeField] private float rallyDuration;
    public bool isRallyOn;

    [Header("Stamina")]
    public PlayerStaminaBar playerStaminaBar;
    [SerializeField] private float maxPlayerStamina;
    [SerializeField] private float playerStaminaRecoverySpeed;
    private float playerStamina;
    private Coroutine recoverStamina;
    private bool isOutOfStamina;
    [SerializeField] private float playerStaminaRecoveryDelay;
    public OutOfStaminaColorTransition outOfStaminaColorTransition;

    [Header("Stamina Cost")]
    [SerializeField] private float playerStepStaminaCost;
    [SerializeField] private float playerJumpStaminaCost;
    [SerializeField] private float playerJumpAttackStaminaCost;
    [SerializeField] private float playerLightAttackStaminaCost;
    [SerializeField] private float playerHeavyLungeBaseStaminaCost;
    [SerializeField] private float playerHeavyLungeExtraStaminaCostScale;

    [Header("Dummy Enemy")]
    public GameObject dummy;
    private DummyController dummyController;

    [Header("Get Hit")]
    [SerializeField] private float getHitStunDuration;
    [SerializeField] private float getHitKnockBackSpeed;
    [SerializeField] private float getParriedStunDuration;

    [Header("Block")]
    [SerializeField] private float blockDuration;
    [SerializeField] private float blockDamageNegationScale;
    [SerializeField] private float blockStaminaDrainScale;
    [SerializeField] private float blockPushBack;

    [Header("Movement")]
    [SerializeField] private float stepSpeed;
    [SerializeField] private float stepDuration;
    [SerializeField] private float stepCooldown;
    [SerializeField] private float shuffleSpeed;
    [SerializeField] private bool canShift;
    [SerializeField] private bool canMoveTowardsEnemy;
    private bool completedShuffleLeft;
    private bool completedShuffleRight;
    private bool completedStepLeft;
    private bool completedStepRight;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHorizontalSpeed;
    [HideInInspector] public bool isJumping;

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
    [SerializeField] private float jumpAttackSwordPivotRotation;
    [SerializeField] private float jumpAttackThrustSpeed;

    [Header("Parry")]
    [SerializeField] private float parryDuration;
    [SerializeField] private float riposteDamageBonus;

    private float direction;
    private string bufferState;
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

        playerHeavyLungeChargeBar.SetMaxValue(heavyLungeMaximumWindupTime);

        audioManager = FindObjectOfType<AudioManager>();
        minimumPlayerDummyDistance = tutorialManager.minimumPlayerDummyDistance;

        playerHealth = maxPlayerHealth;
        playerStamina = maxPlayerStamina;
        playerHealthBar.SetMaxHealth(maxPlayerHealth);
        playerStaminaBar.SetMaxStamina(maxPlayerStamina);
        isOutOfStamina = false;
        isRallyOn = false;

        playerLightAttackBaseDamage = playerLightAttackDamage;
        playerHeavyLungeDamage = playerHeavyLungeBaseDamage;

        dummyController = dummy.GetComponent<DummyController>();

        swordRb = sword.GetComponent<Rigidbody2D>();
        swordCollider = sword.GetComponent<BoxCollider2D>();
        swordCollider.enabled = false;
        swordPivot = sword.transform.parent;
        swordPivotRb = swordPivot.GetComponent<Rigidbody2D>();

        controllerTransform = this.gameObject.transform.GetChild(2);
        animator = controllerTransform.GetComponent<Animator>();

        isFacingLeft = false;

        canShift = true;
        canMoveTowardsEnemy = true;

        direction = 1f;

        bufferState = "None";

        completedShuffleLeft = false;
        completedShuffleRight = false;
        completedStepLeft = false;
        completedStepRight = false;
        isJumping = false;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(state);
        CheckCanMoveTowardsEnemy();

        string gameState = tutorialManager.gameState;
        if (gameState == "Fight"||gameState=="Practice"||gameState== "SpecialTutorialInstructions")
        {
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
                    animator.Play("HeavyWindup");
                    audioManager.Play("HeavyCharge");
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

                #region Cancel Actions and Transitions
                case PlayerState.cancel:
                    CancelActions();
                    CancelTransitions();
                    break;
                #endregion

                #region Parry Actions and Transitions
                case PlayerState.parry:
                    ParryActions();
                    ParryTransitions();
                    break;
                #endregion

                #region Parry Actions and Transitions
                case PlayerState.parried:
                    ParriedActions();
                    ParriedTransitions();
                    break;
                #endregion

                #region Get Hit Actions and Transitions
                case PlayerState.getHit:
                    GetHitActions();
                    GetHitTransitions();
                    break;
                #endregion

                #region Block Actions and Transitions
                case PlayerState.block:
                    BlockActions();
                    BlockTransitions();
                    break;
                #endregion

                #region Dead Actions and Transitions
                case PlayerState.dead:
                    DeadActions();
                    DeadTransitions();
                    break;
                    #endregion
            }
        }
    }

    #region Idle Functions
    private void IdleActions()
    {
        animator.Play("Idle");
        ResetSwordPosition();
        swordRb.isKinematic = true;
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
                stepLeftDustParticle.Play();
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
                stepRightDustParticle.Play();
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
        else if ((kb.spaceKey.isPressed||bufferState=="Jump") && !isOutOfStamina)
        {
            
            bufferState = "None";
            state = PlayerState.jump;
            UseStamina(playerJumpStaminaCost);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if ((kb.oKey.wasPressedThisFrame||bufferState=="LightAttack") && !isOutOfStamina)
        {
            bufferState = "None";
            state = PlayerState.lightAttackWindup;
        }
        else if ((kb.pKey.wasPressedThisFrame||bufferState=="HeavyAttack") && !isOutOfStamina)
        {
            bufferState = "None";
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
            tutorialManager.ShowChargeBar(transform.position);
        }
        else if ((kb.iKey.wasPressedThisFrame||bufferState=="Parry") && !isOutOfStamina)
        {
            bufferState = "None";
            state = PlayerState.parry;
        }
    }

    public void ResetToIdle()
    {
        state = PlayerState.idle;
        animator.Play("Idle");
        //Debug.Log("Reset");
    }
    #endregion

    #region Movement Functions
    private void StepLeftActions()
    {
        animator.Play("StepLeft");
        audioManager.Play("Dash");
        rb.position += Vector2.left * Time.deltaTime * stepSpeed;
        CheckBuffer();

        if (tutorialManager.tutorialInstructionIndex == 1 && !completedStepLeft)
        {
            completedStepLeft = true;
            tutorialManager.UpdateInstructionsCompleted();
        }
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

            stepLeftDustParticle.Stop();
        }
    }

    private void StepRightActions()
    {
        if (canMoveTowardsEnemy)
        {
            animator.Play("StepRight");
            audioManager.Play("Dash");
            rb.position += Vector2.right * Time.deltaTime * stepSpeed;
        }
        CheckBuffer();

        if (tutorialManager.tutorialInstructionIndex == 1 && !completedStepRight)
        {
            completedStepRight = true;
            tutorialManager.UpdateInstructionsCompleted();
        }
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
            stepRightDustParticle.Stop();
        }
    }
    
    private void ShuffleLeftActions()
    {
        audioManager.Play("Shuffle");
        animator.Play("ShuffleLeft");
        rb.position += Vector2.left * Time.deltaTime * shuffleSpeed;

        if (tutorialManager.tutorialInstructionIndex == 0 &&!completedShuffleLeft) 
        {
            completedShuffleLeft = true;
            Debug.Log("completed");
            tutorialManager.UpdateInstructionsCompleted();
        }
    }

    private void ShuffleLeftTransitions()
    {
        if ((kb.oKey.wasPressedThisFrame || bufferState=="LightAttack") && !isOutOfStamina)
        {
            bufferState = "None";
            state = PlayerState.lightAttackWindup;
        }
        else if ((kb.pKey.wasPressedThisFrame||bufferState=="HeavyAttack") && !isOutOfStamina)
        {
            bufferState = "None";
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
            tutorialManager.ShowChargeBar(transform.position);
        }
        else if ((kb.iKey.wasPressedThisFrame||bufferState=="Parry") && !isOutOfStamina)
        {
            bufferState = "None";
            state = PlayerState.parry;
        }
        else if ((kb.spaceKey.isPressed||bufferState=="Jump") && !isOutOfStamina)
        {
            bufferState = "None";
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
                stepRightDustParticle.Play();
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
            stepLeftDustParticle.Play();
        }
    }

    private void ShuffleRightActions()
    {
        if (canMoveTowardsEnemy)
        {
            audioManager.Play("Shuffle");
            animator.Play("ShuffleRight");
            rb.position += Vector2.right * Time.deltaTime * shuffleSpeed;
        }

        if (tutorialManager.tutorialInstructionIndex == 0 && !completedShuffleRight)
        {
            completedShuffleRight = true;
            tutorialManager.UpdateInstructionsCompleted();
        }
    }

    private void ShuffleRightTransitions()
    {
        if ((kb.oKey.wasPressedThisFrame||bufferState=="LightAttack") && !isOutOfStamina)
        {
            bufferState = "None";
            state = PlayerState.lightAttackWindup;
        }
        else if ((kb.pKey.wasPressedThisFrame||bufferState=="HeavyAttack") && !isOutOfStamina)
        {
            bufferState = "None";
            heavyLungeWindupTime = 0f;
            swordRb.isKinematic = false;
            state = PlayerState.heavyLungeWindup;
            tutorialManager.ShowChargeBar(transform.position);
        }
        else if ((kb.iKey.wasPressedThisFrame||bufferState=="Parry") && !isOutOfStamina)
        {
            bufferState = "None";
            state = PlayerState.parry;
        }
        else if ((kb.spaceKey.isPressed||bufferState=="Jump") && !isOutOfStamina)
        {
            bufferState = "None";
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
                stepLeftDustParticle.Play();
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
            stepRightDustParticle.Play();
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
        animator.Play("Jump");
        if (tutorialManager.tutorialInstructionIndex==5 && !isJumping)
        {
            tutorialManager.UpdateInstructionsCompleted();
        }
        isJumping = true;
        if (hasPlayed == false)
        {
            audioManager.Play("Jump");
            
            hasPlayed = true;
        }
        if (kb.dKey.isPressed && canMoveTowardsEnemy)
        {
            
            rb.position += Vector2.right * Time.deltaTime * jumpHorizontalSpeed;
        }
        else if (kb.aKey.isPressed)
        {
            rb.position += Vector2.left * Time.deltaTime * jumpHorizontalSpeed;
        }
        CheckBuffer();
    }

    private void JumpTransitions()
    {
        if ((kb.oKey.wasPressedThisFrame||bufferState=="JumpAttack") && !isOutOfStamina)
        {
            audioManager.Play("JumpAttack");
            animator.Play("JumpAttack");
            bufferState = "None";
            state = PlayerState.jumpAttack;
            swordPivot.localEulerAngles = new Vector3(0f, 0f, jumpAttackSwordPivotRotation);
            UseStamina(playerJumpAttackStaminaCost);
            swordRb.isKinematic = false;
            swordPivotRb.isKinematic = false;
            swordCollider.enabled = true;
            if (tutorialManager.tutorialInstructionIndex == 6)
            {
                tutorialManager.UpdateInstructionsCompleted();
            }
        }
    }

    private void JumpAttackActions()
    {
        sword.transform.localPosition += new Vector3(jumpAttackThrustSpeed  * Time.deltaTime,0f,0f);

        if (canMoveTowardsEnemy)
        {
            //rb.position += Vector2.right * direction * Time.deltaTime * jumpAttackPlayerHorizontalSpeed;
        }
        CheckBuffer();

        currentDamageValue = playerJumpAttackDamage;

    }

    private void JumpAttackTransitions()
    {
        StartCoroutine(JumpAttack());
    }

    private IEnumerator JumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackDuration);
        if (state==PlayerState.jumpAttack)
        {
            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordPivotRb.isKinematic = true;
            state = PlayerState.jump;
            swordCollider.enabled = false;
        }
        
    }
    #endregion

    #region Light Attack Functions
    private void LightAttackWindupActions()
    {
        animator.Play("LightAttack");
        audioManager.Play("LightAttack");
        swordRb.position += Vector2.right * -direction * Time.deltaTime * lightAttackWindupSpeed;
        rb.position += Vector2.right * -direction * Time.deltaTime * lightAttackWindupSpeed*0.1f;
        swordRb.position += Vector2.right * -direction * Time.deltaTime * lightAttackWindupSpeed*0.7f;
        CheckBuffer();
    }

    private void LightAtackWindupTransitions()
    {
        StartCoroutine(LightAttackWindup());
        if (kb.wKey.wasPressedThisFrame)
        {
            audioManager.Play("Cancel");
            swordRb.isKinematic = true;
            ResetSwordPosition();
            tutorialManager.ShowFientText(transform.position);
            state = PlayerState.idle;
            if (tutorialManager.tutorialInstructionIndex == 3)
            {
                tutorialManager.UpdateInstructionsCompleted();
            }
        }
    }

    private void LightAttackActions()
    {
        if (canMoveTowardsEnemy)
        {
            rb.position += Vector2.right * direction * Time.deltaTime * lightAttackThrustSpeed * 0.25f;
            
        }
        swordRb.position += Vector2.right * direction * Time.deltaTime * lightAttackThrustSpeed * 0.75f;
        CheckBuffer();
        currentDamageValue = playerLightAttackDamage;
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
            
            //swordCollider.enabled = true;
        }
    }

    private IEnumerator LightAttack()
    {
        swordCollider.enabled = true;
        yield return new WaitForSeconds(lightAttackDuration);
        if (state == PlayerState.lightAttack)
        {
            swordRb.isKinematic = true;
            swordCollider.enabled = false;
            ResetSwordPosition();
            state = PlayerState.idle;
            playerLightAttackDamage = playerLightAttackBaseDamage;
            if (tutorialManager.tutorialInstructionIndex == 2)
            {
                tutorialManager.UpdateInstructionsCompleted();
            }
        }
    }

    #endregion

    #region Heavy Attack Functions
    private void HeavyLungeWindupActions()
    {
        if (heavyLungeWindupTime <= heavyLungeMaximumWindupTime)
        {
            heavyLungeWindupTime += Time.deltaTime;
            playerHeavyLungeChargeBar.SetValue(heavyLungeWindupTime);
        }
        if (heavyLungeWindupTime >= heavyLungeMinimumWindupTime)
        {
            playerHeavyLungeChargeBar.ChangeColor();
        }
        swordRb.position += Vector2.right * -direction * Time.deltaTime * heavyLungeWindupSpeed;
    }

    private void HeavyLungeWindupTransitions()
    {
        
        if (kb.pKey.wasReleasedThisFrame)
        {
            tutorialManager.HideChargeBar();
            if (heavyLungeWindupTime >= heavyLungeMinimumWindupTime)
            {
                swordCollider.enabled = true;
                state = PlayerState.heavyLunge;
                UseStamina(playerHeavyLungeBaseStaminaCost+heavyLungeWindupTime*playerHeavyLungeExtraStaminaCostScale);
                playerHeavyLungeDamage = playerHeavyLungeBaseDamage + heavyLungeWindupTime * playerHeavyLungeExtraDamageScale;
                swordRb.position += Vector2.down*heavyLungeLowerSwordScale;
                heavyLungeThrustTime = heavyLungeWindupTime * heavyLungeWindupThrustScale;
                heavyLungeThrustSpeed+= heavyLungeWindupTime * heavyLungeWindupThrustScale;
                currentDamageValue = playerHeavyLungeDamage;
                if (tutorialManager.tutorialInstructionIndex == 4)
                {
                    tutorialManager.UpdateInstructionsCompleted();
                }
            }
            else
            {
                state = PlayerState.idle;
                ResetSwordPosition();
            }
        }
        else if (!kb.pKey.isPressed)
        {
            state = PlayerState.idle;
            ResetSwordPosition();
        }
    }

    private void HeavyLungeActions()
    {
        audioManager.Play("HeavyAttack");
        animator.Play("HeavyAttack");
        if (canMoveTowardsEnemy)
        {
            
            rb.position += Vector2.right * direction * Time.deltaTime * heavyLungeThrustSpeed;
            
        }
        swordRb.position += Vector2.right * direction * Time.deltaTime * heavyLungeThrustSpeed;
        CheckBuffer();
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
            heavyLungeThrustSpeed -= heavyLungeWindupTime * heavyLungeWindupThrustScale;
            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordCollider.enabled = false;
            
        }
    }

    private void HeavyLungeStunActions()
    {
        CheckBuffer();
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

    #region Cancel Functions
    private void CancelActions()
    {

    }

    private void CancelTransitions()
    {

    }
    #endregion

    #region Parry Functions
    private void ParryActions()
    {
        CheckBuffer();
    }

    private void ParryTransitions()
    {
        StartCoroutine(Parry());
    }

    private IEnumerator Parry()
    {
        //audioManager.Play("Parry");
        animator.Play("Parry");
        yield return new WaitForSeconds(parryDuration);
        if (state == PlayerState.parry)
        {
            state = PlayerState.idle;
        }
    }
    #endregion

    #region Parried Functions
    private void ParriedActions()
    {

    }

    private void ParriedTransitions()
    {
        StartCoroutine(Parried());
    }

    private IEnumerator Parried()
    {
        Debug.Log("parried");
        yield return new WaitForSeconds(getParriedStunDuration);
        if (state == PlayerState.parried)
        {
            state = PlayerState.idle;
        }

    }

    public void ActivateParried()
    {
        state = PlayerState.parried;
    }
    #endregion

    #region Get Hit Functions
    private void GetHitActions()
    {
        CheckBuffer();
        rb.position += Vector2.left * getHitKnockBackSpeed * Time.deltaTime;
    }

    private void GetHitTransitions()
    {
        StartCoroutine(GetHit());
    }

    private IEnumerator GetHit()
    {
        audioManager.Play("LightDamageHit");
        animator.Play("GetHit");
        
        yield return new WaitForSeconds(getHitStunDuration);
        if (state == PlayerState.getHit)
        {
            state = PlayerState.idle;
        }
    }

    private void TakeHitDamage(float damage)
    {
        tutorialManager.HideChargeBar();
        playerHealth -= damage;
        playerHealthBar.SetHealth(playerHealth);
        tutorialManager.SpawnLeftBloodParticle(transform.position);
        Debug.Log("hit, remaining health: "+playerHealth+" damage dealt was: "+damage);
        tutorialManager.ShowDamageText(transform.position, damage);
        tutorialManager.getHitVolume.SetActive(true);
        tutorialManager.ResetGetHitUI();
        state = PlayerState.getHit;
        ActivateRally();
        CheckDead();

        stepLeftDustParticle.Stop();
        stepRightDustParticle.Stop();
    }

    #endregion

    #region Block Functions
    private void BlockActions()
    {
        rb.velocity += Vector2.right * direction * blockPushBack;
    }

    private void BlockTransitions()
    {
        //StartCoroutine(Block());
    }

    private IEnumerator ResetBlock()
    {
        yield return new WaitForSeconds(blockDuration);
        if (state == PlayerState.block)
        {
            state = PlayerState.idle;
        }
    }

    private void ActivateBlock()
    {
        animator.Play("Block");
        audioManager.Play("Block");
        state = PlayerState.block;
        StopCoroutine(ResetBlock());
        StartCoroutine(ResetBlock());
    }

    private void TakeBlockDamage(float baseDamage)
    {
        ActivateBlock();
        float blockedDamage = baseDamage * (1f - blockDamageNegationScale);
        playerHealth -= blockedDamage;
        playerHealthBar.SetHealth(playerHealth);
        tutorialManager.ShowDamageText(transform.position, blockedDamage);
        tutorialManager.ShowBlockText(transform.position);
        UseStamina(baseDamage * blockStaminaDrainScale);
        CheckDead();
    }
    #endregion

    #region Dead Functions
    private void DeadActions()
    {

    }

    private void DeadTransitions()
    {

    }

    private void CheckDead()
    {
        if (playerHealth <= 0f)
        {
            playerHealth = 0f;
            state = PlayerState.dead;
            //Time.timeScale = 0;
            tutorialManager.getHitVolume.SetActive(false);
        }
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
            outOfStaminaColorTransition.isOutOfStamina = true;
            tutorialManager.outOfStaminaTextUI.SetActive(true);
        }
    }
    private IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(playerStaminaRecoveryDelay);
        isOutOfStamina = false;
        outOfStaminaColorTransition.isOutOfStamina = false;
        tutorialManager.outOfStaminaTextUI.SetActive(false);
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

    #region Rally
    private IEnumerator ResetRally()
    {
        yield return new WaitForSeconds(rallyDuration);
        isRallyOn = false;
    }

    private void ActivateRally()
    {
        isRallyOn = true;
        StopCoroutine(ResetRally());
        StartCoroutine(ResetRally());
    }

    public void AddRallyHealth(float baseHealth)
    {
        playerHealth += baseHealth * rallyScale;
        playerHealthBar.SetHealth(playerHealth);
    }
    #endregion

    #region Collisions

    #region Ground Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            state = PlayerState.idle;
            //Debug.Log(isOutOfStamina);
            isJumping = false;
            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordPivotRb.isKinematic = true;
            swordCollider.enabled = false;
        }
        
    }
    #endregion

    #region Enemy Sword Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
       //dummy doesnt attack
    }
    #endregion
    #endregion

    //void Flip()
    //{
    //    isFacingLeft = !isFacingLeft;
    //    if (isFacingLeft)
    //    {
    //        direction = -1f;
    //    }
    //    else
    //    {
    //        direction = 1f;
    //    }
    //    transform.Rotate(0f, 180f, 0f);
    //}

    public void ResetSwordPosition()
    {
        swordPivot.localEulerAngles = Vector3.zero;
        swordPivot.position = transform.position+new Vector3(0f, 0.4f, 0f);
        sword.transform.position = new Vector3(swordPivot.position.x + 2.6f, swordPivot.position.y + 0.9f, transform.position.z);
        sword.transform.localEulerAngles = new Vector3(0f, 0f, -70f);
    }

    private void CheckCanMoveTowardsEnemy()
    {
        if (Mathf.Abs(transform.position.x - dummy.transform.position.x)<=minimumPlayerDummyDistance)
        {
            canMoveTowardsEnemy = false;
        }
        else
        {
            canMoveTowardsEnemy = true;
        }
    }

    private void CheckBuffer()
    {
        if (kb.spaceKey.wasPressedThisFrame)
            {
                bufferState = "Jump";
            }
        else if (kb.oKey.wasPressedThisFrame && state!=PlayerState.jump)
            {
            if (state == PlayerState.jumpAttack)
            {
                bufferState = "JumpAttack";
            }
            else
            {
                bufferState = "LightAttack";
            }
            }
         else if (kb.pKey.wasPressedThisFrame)
            {
                bufferState = "HeavyAttack";
            }
          else if (kb.iKey.wasPressedThisFrame)
            {
                bufferState = "Parry";
            }
        
    }
}
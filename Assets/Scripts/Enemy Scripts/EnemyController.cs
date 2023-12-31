using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static PlayerController;
using System.Linq;

public class EnemyController : MonoBehaviour
{

    #region Enemy States
    public enum EnemyState
    {
        idle,
        stepLeft, stepRight, shuffleLeft, shuffleRight,
        jump, jumpAttack,
        lightAttackWindup, lightAttack,
        heavyLungeWindup, heavyLunge, heavyLungeStun,
        parry,
        parried,
        getHit,
        block,
        dead
    }

    public EnemyState state;
    #endregion

    #region Enemy Variables and Components
    public GameManager gameManager;
    public DialogueManager dialogueManager;
    public UIManager UIManager;
    public MusicManager musicManager;
    private float minimumPlayerEnemyDistance;
    public Rigidbody2D rb;
    public Rigidbody2D swordRb;
    public GameObject sword;
    public enemy enemy;
    private Transform swordPivot;
    private Rigidbody2D swordPivotRb;
    private Collider2D swordCollider;
    public Animator animator;
    public Transform controllerTransform;
    public AudioManager audioManager;
    private int block;
    public float shuffleTransitionDistance;

    [Header("Misc Particles")]
    public ParticleSystem stepLeftDustParticle;
    public ParticleSystem stepRightDustParticle;

    [Header("Health")]
    public EnemyHealthBar enemyHealthBar;
    [SerializeField] private float maxEnemyHealth;
    private float enemyHealth;

    [Header("Movement")]
    [SerializeField] private float stepSpeed;
    [SerializeField] private float stepDuration;
    [SerializeField] private float stepCooldown;
    [SerializeField] private float shuffleSpeed;
    [SerializeField] private bool canShift;
    [SerializeField] private bool canMoveTowardsEnemy;

    [Header("Enemy Damage")]
    public float enemyLightAttackDamage;
    [HideInInspector] public float enemyLightAttackBaseDamage;
    public float enemyHeavyLungeBaseDamage;
    public float enemyHeavyLungeExtraDamageScale;
    [HideInInspector] public float enemyHeavyLungeDamage;
    public float enemyJumpAttackDamage;

    [Header("Player")]
    public GameObject player;
    private PlayerController playerController;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHorizontalSpeed;

    [Header("Get Hit")]
    [SerializeField] private float getHitStunDuration;
    [SerializeField] private float getHitKnockBackSpeed;
    [SerializeField] private float getParriedStunDuration;

    [Header("Light Attack")]

    [SerializeField] private float lightAttackWindupDuration;
    [SerializeField] private float lightAttackWindupSpeed;
    [SerializeField] private float lightAttackDuration;
    [SerializeField] private float lightAttackThrustSpeed;
    private bool isLightAttackOnCooldown;

    [Header("Block")]
    [SerializeField] private float blockDuration;
    [SerializeField] private float blockDamageNegationScale;
    [SerializeField] private float blockPushback;

    [Header("Jump Attack")]
    [SerializeField] private float jumpAttackDuration;
    [SerializeField] private float jumpAttackSwordPivotRotation;
    [SerializeField] private float jumpAttackThrustSpeed;

    [Header("Heavy Attack")]
    [SerializeField] private float heavyLungeWindupSpeed;
    [SerializeField] private float heavyLungeWindupThrustScale;
    private float heavyLungeWindupTime;
    private float heavyLungeThrustTime;
    [SerializeField] private float heavyLungeLowerSwordScale;
    [SerializeField] private float heavyLungeThrustSpeed;
    private float baseHeavyLungeThrustSpeed;
    [SerializeField] private float heavyLungeStunDuration;
    [SerializeField] private float minHeavyLungeWindupDuration;
    [SerializeField] private float maxHeavyLungeWindupDuration;

    [Header("Parry")]
    [SerializeField] private float parryDuration;
    [SerializeField] private float riposteDamageBonus;
    [SerializeField] private float parryChance;


    [Header("Enemy AI")]
    [SerializeField] private float baseIdleChance;
    [SerializeField] private float aggressiveness;
    [SerializeField] private float difficulty;
    [SerializeField] private float shuffleLeftDistanceDivideScale;
    [SerializeField] private float minimumStepLeftDistance;
    [SerializeField] private float stepLeftDistanceDivideScale;
    [SerializeField] private float playerLightAttackRetreatChance;
    [SerializeField] private float playerJumpAttackRetreatChance;
    [SerializeField] private float playerHeavyLungeWindupAdvanceChance;
    [SerializeField] private float chaseAfterEnemyDistanceDivideScale;
    [SerializeField] private float unableToChangeDirectionDuration;
    [SerializeField] private bool isAbleToChangeDirection;
    [SerializeField] private float baseLightAttackChance;
    [SerializeField] private float baseHeavyLungeChance;
    [SerializeField] private float lightAttackRange;
    [SerializeField] private float heavyLungeRange;
    [SerializeField] private float heavyLungeDistanceScale;

    [SerializeField] private bool canParry;
    [SerializeField] private bool canJumpAttack;
    //[SerializeField] private float playerHeavyLungeJumpChance;
    //[SerializeField] private float baseJumpAttackChance;
    //[SerializeField] private float playerHeavyLungeJumpAttackChance;

    private Dictionary<string, float> chanceDict = new Dictionary<string, float>();
    [SerializeField] private string stateToEnter;

    private float direction;
    private float distance;

    private bool isGrounded;
    public bool isParrying;

    #region Timers
    private float stepLeftTimer;
    private float stepRightTimer;

    
    #endregion
#endregion

    void Start()
    {
        #region Initialize Variables
        isGrounded = true;
        isParrying = false;

        isLightAttackOnCooldown = false;
        isAbleToChangeDirection = true;
        canMoveTowardsEnemy = true;
        audioManager = FindObjectOfType<AudioManager>();
        canShift = true;
        playerController = player.GetComponent<PlayerController>();
        swordRb = sword.GetComponent<Rigidbody2D>();
        enemyHealth = maxEnemyHealth;
        enemyHealthBar.SetMaxHealth(maxEnemyHealth);
        swordCollider = sword.GetComponent<BoxCollider2D>();
        swordCollider.enabled = false;
        swordPivot = sword.transform.parent;
        swordPivotRb = swordPivot.GetComponent<Rigidbody2D>();
        enemyHeavyLungeDamage = enemyHeavyLungeBaseDamage;
        minimumPlayerEnemyDistance = gameManager.minimumPlayerEnemyDistance;
        direction = -1f;
        enemyLightAttackBaseDamage = enemyLightAttackDamage;
        baseHeavyLungeThrustSpeed = heavyLungeThrustSpeed;

        controllerTransform = this.gameObject.transform.GetChild(2);
        animator = controllerTransform.GetComponent<Animator>();
        heavyLungeWindupTime = 0f;

        block = 1;

        stateToEnter = "idleChance";

        chanceDict["shuffleLeftChance"] = 0f;
        chanceDict["shuffleRightChance"] = 0f;
        chanceDict["stepLeftChance"] = 0f;
        chanceDict["stepRightChance"] = 0f;
        chanceDict["idleChance"] = baseIdleChance;
        chanceDict["lightAttackChance"] = baseLightAttackChance;
        //chanceDict["jumpChance"] = 0f;
        chanceDict["parryChance"] = 0f;
        //chanceDict["jumpAttackChance"] = 0f;
        //chanceDict["heavyAttackChance"] = baseHeavyLungeChance;

        //swordRb.isKinematic = false;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        string gameState = gameManager.gameState;
        //Debug.Log(chanceDict["jumpAttackChance"]);
        if (gameState == "Fight")
        {
            UpdateDistance();
            CheckCanMoveTowardsEnemy();
            EnemyAI();
            UpdateState();
            switch (state)
            {
                #region Idle Actions and Transitions
                case EnemyState.idle:
                    IdleActions();
                    IdleTransitions();
                    break;
                #endregion

                #region Movement Actions and Transitions
                case EnemyState.stepLeft:
                    StepLeftActions();
                    StepLeftTransitions();
                    break;
                case EnemyState.stepRight:
                    StepRightActions();
                    StepRightTransitions();
                    break;
                case EnemyState.shuffleLeft:
                    ShuffleLeftActions();
                    ShuffleLeftTransitions();
                    break;
                case EnemyState.shuffleRight:
                    ShuffleRightActions();
                    ShuffleRightTransitions();
                    break;
                #endregion

                #region Jump Actions and Transitions
                case EnemyState.jump:
                    JumpActions();
                    JumpTransitions();
                    break;
                case EnemyState.jumpAttack:
                    JumpAttackActions();
                    JumpAttackTransitions();
                    break;
                #endregion

                #region Light Attack Actions and Transitions
                case EnemyState.lightAttackWindup:
                    LightAttackWindupActions();
                    LightAtackWindupTransitions();
                    break;
                case EnemyState.lightAttack:
                    LightAttackActions();
                    LightAttackTransitions();
                    break;
                #endregion

                #region Heavy Attack Actions and Transitions
                case EnemyState.heavyLungeWindup:
                    HeavyLungeWindup();
                    break;
                case EnemyState.heavyLunge:
                    HeavyLunge();
                    break;
                case EnemyState.heavyLungeStun:
                    HeavyLungeStunTransitions();
                    break;
                #endregion

                #region Parry Actions and Transitions
                case EnemyState.parry:
                    ParryActions();
                    ParryTransitions();
                    break;
                #endregion

                #region Parried Actions and Transitions
                case EnemyState.parried:
                    ParriedActions();
                    ParriedTransitions();
                    break;
                #endregion]

                #region Get Hit Actions and Transitions
                case EnemyState.getHit:
                    GetHitActions();
                    GetHitTransitions();
                    break;
                #endregion

                #region Block Actions and Transitions
                case EnemyState.block:
                    BlockActions();
                    BlockTransitions();
                    break;
                #endregion

                #region Dead Actions and Transitions
                case EnemyState.dead:
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
        ResetSwordPosition();
        swordRb.isKinematic = true;
    }

    private void IdleTransitions()
    {
        
    }
    public void ResetToIdle()
    {
        state = EnemyState.dead;
    }
    #endregion

    #region Movement Functions
    private void StepLeftActions()
    {
        if (canMoveTowardsEnemy)
        {
            //animator.Play("StepLeft");
            //audioManager.Play("BossDash");
            rb.position += Vector2.left * Time.deltaTime * stepSpeed;
        }
    }

    private void StepLeftTransitions()
    {
        stepLeftTimer += Time.deltaTime;
        //Debug.Log(stepLeftTimer);
        if (stepLeftTimer >= stepDuration)
        {
            state = EnemyState.idle;
            //canShift = false;
            StartCoroutine(ResetCanShift());
            stepLeftDustParticle.Stop();
        }
    }

    private void StepRightActions()
    {
        //animator.Play("StepRight");
        //audioManager.Play("BossDash");
        rb.position += Vector2.right * Time.deltaTime * stepSpeed;
    }

    private void StepRightTransitions()
    {
        stepRightTimer += Time.deltaTime;
        //Debug.Log(stepRightTimer);
        if (stepRightTimer >= stepDuration)
        {
            state = EnemyState.idle;
            //canShift = false;
            StartCoroutine(ResetCanShift());
            stepRightDustParticle.Stop();
        }
    }

    private void ShuffleLeftActions()
    {
        if (canMoveTowardsEnemy)
        {
            if(Vector2.Distance(gameObject.transform.position, player.transform.position) <= shuffleTransitionDistance)
            {
                animator.Play("CloseShuffleLeft");
            }
            else
            {
                animator.Play("ShuffleLeft");
            }
            
            rb.position += Vector2.left * Time.deltaTime * shuffleSpeed;
        }
    }
    //Write Transitions
    private void ShuffleLeftTransitions()
    {
        
    }

    private void ShuffleRightActions()
    {
        if (Vector2.Distance(gameObject.transform.position, player.transform.position) <= shuffleTransitionDistance)
        {
            animator.Play("CloseShuffleRight");
        }
        else
        {
            animator.Play("ShuffleRight");
        }
        rb.position += Vector2.right * Time.deltaTime * shuffleSpeed;
    }
    //Write Transitions
    private void ShuffleRightTransitions()
    {
        
    }
    private IEnumerator ResetCanShift()
    {
        canShift = false;
        yield return new WaitForSeconds(stepCooldown);
        canShift = true;
    }

    #endregion

    #region Jump Functions
    private void JumpActions()
    {
        //TODO: IN AIR MOVEMENT
        //if (rb.velocity.x > 0)
        //{
        // rb.position += Vector2.right * Time.deltaTime * jumpHorizontalSpeed;
        //}
        //else if (rb.velocity.x < 0)
        //{
        // rb.position += Vector2.left * Time.deltaTime * jumpHorizontalSpeed;
        //}
        //if (!hasPlayed)
        //{
        //    //animator.Play("Jump");
        //    hasPlayed = true;
        //}
    }

    private void JumpTransitions()
    {
        /*
        {
            state = EnemyState.jumpAttack;
            swordPivot.position = transform.position + new Vector3(0.5f, -1f, 0f);
            swordRb.isKinematic = false;
            swordCollider.enabled = true;
        }
        */
    }

    private void JumpAttackActions()
    {
        sword.transform.localPosition += new Vector3(jumpAttackThrustSpeed * Time.deltaTime, 0f, 0f);

    }

    private void JumpAttackTransitions()
    {
        StartCoroutine(JumpAttack());
    }

    private IEnumerator JumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackDuration);
        if (state == EnemyState.jumpAttack)
        {
            //animator.Play("JumpAttack");
            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordPivotRb.isKinematic = true;
            state = EnemyState.jump;
            swordCollider.enabled = false;
        }
    }
    #endregion

    #region Light Attack Functions
    private void LightAttackWindupActions()
    {
        if (!isParrying)
        {
            animator.Play("LightAttack");
            audioManager.Play("BossLightAttack");
            rb.position += Vector2.right * -direction * Time.deltaTime * lightAttackWindupSpeed * 0.1f;
            swordRb.position += Vector2.right * -direction * Time.deltaTime * lightAttackWindupSpeed * 0.7f;
        }
    }

    private void LightAtackWindupTransitions()
    {
        if (!isParrying)
        {
            StartCoroutine(LightAttackWindup());
        }
        
    }

    private void LightAttackActions()
    {
        if (canMoveTowardsEnemy)
        {
            rb.position += Vector2.right * direction * Time.deltaTime * lightAttackThrustSpeed * 0.35f;
            
        }
        swordRb.position += Vector2.right * direction * Time.deltaTime * lightAttackThrustSpeed * 0.75f;
    }

    private void LightAttackTransitions()
    {
        //StartCoroutine(LightAttack());
    }

    private IEnumerator LightAttackWindup()
    {
        
        yield return new WaitForSeconds(lightAttackWindupDuration);
        if (state == EnemyState.lightAttackWindup)
        {
            state = EnemyState.lightAttack;
            swordCollider.enabled = true;
            StartCoroutine(LightAttack());
        }
    }

    private IEnumerator LightAttack()
    {
        yield return new WaitForSeconds(lightAttackDuration);
        if (state == EnemyState.lightAttack)
        {
            swordRb.isKinematic = true;
            swordCollider.enabled = false;
            ResetSwordPosition();
            state = EnemyState.idle;
            enemyLightAttackDamage = enemyLightAttackBaseDamage;
            StartCoroutine(ResetLightAttackCooldown());
        }
    }

    private IEnumerator ResetLightAttackCooldown()
    {
        isLightAttackOnCooldown = true;
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        isLightAttackOnCooldown = false;
    }
    #endregion

    #region Heavy Attack Functions
    public void HeavyLungeWindup()
    {
        //StartCoroutine(HeavyLungeWindupCoroutine());
        swordRb.position += Vector2.right * -direction * Time.deltaTime * heavyLungeWindupSpeed;
    }
    public IEnumerator HeavyLungeWindupCoroutine()
    {
        animator.Play("HeavyWindup");
        audioManager.Play("BossHeavyCharge");
        yield return new WaitForSeconds(heavyLungeWindupTime);
        
        if (state == EnemyState.heavyLungeWindup)
        {
            heavyLungeThrustTime = heavyLungeWindupTime * heavyLungeWindupThrustScale;
            heavyLungeThrustSpeed = baseHeavyLungeThrustSpeed + heavyLungeWindupTime * heavyLungeWindupThrustScale;
            state = EnemyState.heavyLunge;
            swordCollider.enabled = true;
            StartCoroutine(HeavyLungeCoroutine());
        }
    }

    private IEnumerator ShowHeavyLungeExclaimationText()
    {
        yield return new WaitForSeconds(heavyLungeWindupTime - UIManager.showExclaimationTextDuration);
        UIManager.ShowExclaimationText(transform.position);
        yield return new WaitForSeconds(UIManager.showExclaimationTextDuration);
    }

    public void HeavyLunge()
    {
        //StartCoroutine(HeavyLungeCoroutine());
        if (canMoveTowardsEnemy)
        {
            audioManager.Play("BossHeavy");
            animator.Play("HeavyAttack");
            rb.position += Vector2.right * direction * Time.deltaTime * heavyLungeThrustSpeed;
            
        }
        if (gameManager.isSceneTwo)
        {
            swordRb.position += Vector2.right * direction * Time.deltaTime * heavyLungeThrustSpeed * 1.05f;
        }
        else
        {
            swordRb.position += Vector2.right * direction * Time.deltaTime * heavyLungeThrustSpeed * 1.35f;
        }
        

    }
    public IEnumerator HeavyLungeCoroutine()
    {
        yield return new WaitForSeconds(heavyLungeThrustTime);
        if (state == EnemyState.heavyLunge)
        {
            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordCollider.enabled = false;
            state = EnemyState.heavyLungeStun;
        }
    }

    private void HeavyLungeStunTransitions()
    {
        StartCoroutine(HeavyLungeStun());
    }

    private IEnumerator HeavyLungeStun()
    {
        yield return new WaitForSeconds(heavyLungeStunDuration);
        if (state == EnemyState.heavyLungeStun)
        {
            state = EnemyState.idle;
        }
    }
    #endregion

    #region Parry Functions
    private void ParryActions()
    {
        
    }
    private void ParryTransitions()
    {
        
    }

    private IEnumerator Parry()
    {
        animator.Play("Parry");
        yield return new WaitForSeconds(parryDuration);
        if (state == EnemyState.parry)
        {
            state = EnemyState.idle;
            isParrying = false;
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
        //Debug.Log("parried");
        animator.Play("Parried");
        yield return new WaitForSeconds(getParriedStunDuration);
        if (state == EnemyState.parried)
        {
            state = EnemyState.idle;
            
        }
    }

    public void ActivateParried()
    {
        state = EnemyState.parried;
    }
    #endregion

    #region Get Hit Functions
    private void GetHitActions()
    {
        //TakeHitDamage(playerController.currentDamageValue);
        rb.position += Vector2.right * getHitKnockBackSpeed * Time.deltaTime;
    }

    private void GetHitTransitions()
    {
        StartCoroutine(GetHit());
    }
    private IEnumerator GetHit()
    {
        
        yield return new WaitForSeconds(getHitStunDuration);
        if (state == EnemyState.getHit)
        {
            state = EnemyState.idle;
        }
    }

    private void TakeHitDamage(float damage)
    {
        damage += gameManager.RandomDamageModifier();

        animator.Play("GetHit");
        gameManager.SpawnRightBloodParticle(transform.position);
        enemyHealth -= damage;
        enemyHealthBar.SetHealth(enemyHealth);
        UIManager.ShowDamageText(transform.position, damage);
        //gameManager.getHitVolume.SetActive(true);
        //gameManager.ResetGetHitUI();
        //Debug.Log("enemy damaged, remaining health: " + enemyHealth);
        state = EnemyState.getHit;

        if (playerController.isRallyOn)
        {
            playerController.AddRallyHealth(damage);
        }

        stepLeftDustParticle.Stop();
        stepRightDustParticle.Stop();

        CheckDead();
    }
    #endregion

    #region Block Functions
    private void BlockActions()
    {
        rb.velocity += Vector2.right *-direction* blockPushback;
    }

    private void BlockTransitions()
    {

    }

    private IEnumerator ResetBlock()
    {
        if (block == 1)
        {
            animator.Play("Block");
            block = 2;
        }
        else if (block == 2)
        {
            animator.Play("Block2");
            block = 1;
        }
        yield return new WaitForSeconds(blockDuration);
        if (state == EnemyState.block)
        {
            state = EnemyState.idle;
        }
    }

    private void ActivateBlock()
    {
        state = EnemyState.block;
        StopCoroutine(ResetBlock());
        StartCoroutine(ResetBlock());
    }

    private void TakeBlockDamage(float baseDamage)
    {
        ActivateBlock();
        float blockedDamage = baseDamage * (1f - blockDamageNegationScale);
        enemyHealth -= blockedDamage;
        enemyHealthBar.SetHealth(enemyHealth);
        UIManager.ShowDamageText(transform.position, blockedDamage);
        UIManager.ShowBlockText(transform.position);
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
        if (enemyHealth <= 0f)
        {
            playerController.ResetToIdle();
            animator.Play("Death");
            audioManager.Play("BossDeath");
            enemyHealth = 0f;
            state = EnemyState.dead;
            playerController.ResetToIdle();
            playerController.ResetSwordPosition();
            player.transform.position = new Vector3(player.transform.position.x, -2.1f, player.transform.position.z);
            //Time.timeScale = 0;
            gameManager.getHitVolume.SetActive(false);
            gameManager.gameState = "PlayerWinDialogue";
            dialogueManager.ShowPlayerWinDialogue(0);
            gameManager.dialogueVolume.SetActive(true);
            gameManager.combatVolume.SetActive(false);

            musicManager.StartMuffle();
        }
    }
    #endregion

    #region Collisions

    #region Ground Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if(state == EnemyState.jump)
            {
                animator.Play("Land");
                audioManager.Play("Land");
            }
            state = EnemyState.idle;

            ResetSwordPosition();
            swordRb.isKinematic = true;
            swordPivotRb.isKinematic = true;
            swordCollider.enabled = false;

            isGrounded = true;
        }
    }
    #endregion

    #region Enemy Sword Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerSword")
        {
            PlayerController.PlayerState playerState = playerController.state;

            #region Enemy is Idle/Moving/Jumping/Windup/Stun
            if (state == EnemyState.idle || state == EnemyState.shuffleLeft || state == EnemyState.stepLeft || state == EnemyState.stepRight || state == EnemyState.lightAttackWindup || state == EnemyState.heavyLungeWindup || state == EnemyState.heavyLungeStun || state == EnemyState.jump || state == EnemyState.lightAttack || state==EnemyState.parried)
            {
                //state = PlayerState.getHit;
                //ActivateRally();
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    TakeHitDamage(playerController.playerLightAttackDamage);
                    audioManager.Play("BossLightHit");
                    if (playerController.playerLightAttackDamage != playerController.playerLightAttackBaseDamage)
                    {
                        UIManager.ShowRiposteText(player.transform.position);
                        audioManager.Play("Riposte");
                    }
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge && !(state == EnemyState.jump))
                {
                    UIManager.ShowCritText(transform.position);
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                    audioManager.Play("BossHeavyHit");
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                    audioManager.Play("BossLightHit");
                }
            }
            #endregion

            #region Enemy is Blocking
            else if (state == EnemyState.shuffleRight)
            {
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    
                    if (playerController.playerLightAttackDamage != playerController.playerLightAttackBaseDamage)
                    {
                        UIManager.ShowRiposteText(player.transform.position);
                        TakeHitDamage(playerController.playerLightAttackDamage);
                        audioManager.Play("BossHeavyHit");
                        audioManager.Play("Riposte");
                    }
                    else
                    {
                        TakeBlockDamage(playerController.playerLightAttackDamage);
                        audioManager.Play("Block");
                    }
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeBlockDamage(playerController.playerJumpAttackDamage);
                    audioManager.Play("Block");
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge && !(state == EnemyState.jump))
                {
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                }
            }
            #endregion

            #region Enemy is Jump Attacking
            else if (state == EnemyState.jumpAttack)
            {
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    TakeHitDamage(playerController.playerLightAttackDamage);
                    if (playerController.playerLightAttackDamage != playerController.playerLightAttackBaseDamage)
                    {
                        UIManager.ShowRiposteText(player.transform.position);
                        audioManager.Play("Riposte");
                    }
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
            }
            #endregion

            #region Enemy is Parrying
            else if (state == EnemyState.parry)
            {
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    playerController.ActivateParried();
                    UIManager.ShowParryText(transform.position);
                    audioManager.Play("Parry");
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                    UIManager.ShowCritText(transform.position);
                    audioManager.Play("BossHeavyHit");
                }
            }
            #endregion

            #region Enemy is Heavy Lunging
            else if (state == EnemyState.heavyLunge)
            {
                if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                }
            }
            #endregion

            #region Enemy is already in Get Hit
            else if (state == EnemyState.getHit)
            {
                
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    enemyHealth -= playerController.playerLightAttackDamage;
                    enemyHealthBar.SetHealth(enemyHealth);
                    UIManager.ShowDamageText(transform.position, playerController.playerLightAttackDamage);
                    if (playerController.playerLightAttackDamage != playerController.playerLightAttackBaseDamage)
                    {
                        UIManager.ShowRiposteText(player.transform.position);
                    }
                    audioManager.Play("BossLightHit");
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    enemyHealth -= playerController.playerJumpAttackDamage;
                    enemyHealthBar.SetHealth(enemyHealth);
                    UIManager.ShowDamageText(transform.position, playerController.playerJumpAttackDamage);
                    audioManager.Play("BossLightHit");
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    enemyHealth -= playerController.playerHeavyLungeDamage;
                    enemyHealthBar.SetHealth(enemyHealth);
                    UIManager.ShowDamageText(transform.position, playerController.playerHeavyLungeDamage);
                    UIManager.ShowCritText(transform.position);
                    audioManager.Play("BossHeavyHit");
                }
                
            }
            #endregion
        }
    }
    #endregion

    #endregion

    public void ResetSwordPosition()
    {
        swordPivot.localEulerAngles = Vector3.zero;
        swordPivot.position = transform.position + new Vector3(0f, 0.4f, 0f);
        sword.transform.position = new Vector3(swordPivot.position.x -2f, swordPivot.position.y + 0.6f, transform.position.z);
        sword.transform.localEulerAngles = new Vector3(0f, 180f, -75f);
    }

    private void CheckCanMoveTowardsEnemy()
    {
        if (distance <= minimumPlayerEnemyDistance)
        {
            canMoveTowardsEnemy = false;
        }
        else
        {
            canMoveTowardsEnemy = true;
        }
    }

    private void UpdateDistance()
    {
        distance = Mathf.Abs(transform.position.x - player.transform.position.x);
        //Debug.Log(distance);
    }

    private void EnemyAI()
    {
        PlayerController.PlayerState playerState = playerController.state;
        if ((state == EnemyState.idle || state==EnemyState.shuffleLeft||state==EnemyState.shuffleRight))
        {
            #region Adjust Idle Chances
            if (isAbleToChangeDirection)
            {
                if (distance < heavyLungeRange)
                {
                    chanceDict["heavyLungeChance"] = 0f;
                }
                else
                {
                    chanceDict["heavyLungeChance"] = baseHeavyLungeChance * aggressiveness * (distance / heavyLungeDistanceScale);
                }

                if (distance > lightAttackRange || isLightAttackOnCooldown)
                {
                    chanceDict["lightAttackChance"] = 0f;
                }
                else
                {
                    chanceDict["lightAttackChance"] = baseLightAttackChance * aggressiveness;
                }

                chanceDict["shuffleLeftChance"] = (distance / shuffleLeftDistanceDivideScale) * aggressiveness;
                if (distance <= minimumStepLeftDistance)
                {
                    chanceDict["stepLeftChance"] = 0f;
                }
                else
                {
                    chanceDict["stepLeftChance"] = (distance / stepLeftDistanceDivideScale) * aggressiveness;
                }
                chanceDict["shuffleRightChance"] = (1f - chanceDict["shuffleLeftChance"]);
                if (chanceDict["shuffleRightChance"] < 0f)
                {
                    chanceDict["shuffleRightChance"] = 0f;
                }
                chanceDict["stepRightChance"] = 0f;
                
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    chanceDict["shuffleRightChance"] += playerLightAttackRetreatChance * (2f - aggressiveness);
                    chanceDict["stepRightChance"] += playerLightAttackRetreatChance * (2f - aggressiveness) * difficulty*4f;
                    
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    chanceDict["shuffleRightChance"] += playerJumpAttackRetreatChance * (2f - aggressiveness);
                    chanceDict["stepRightChance"] += playerJumpAttackRetreatChance * (2f - aggressiveness) * difficulty;
                }
                else if (playerState == PlayerController.PlayerState.heavyLungeWindup)
                {
                    chanceDict["shuffleLeftChance"] += playerHeavyLungeWindupAdvanceChance * aggressiveness;
                    chanceDict["stepLeftChance"] += playerHeavyLungeWindupAdvanceChance * aggressiveness * difficulty;
                }
                else if (playerState == PlayerController.PlayerState.stepLeft)
                {
                    chanceDict["stepLeftChance"] += (distance / chaseAfterEnemyDistanceDivideScale) * aggressiveness;
                }
                if (playerState == PlayerController.PlayerState.parried)
                {
                    state = EnemyState.lightAttackWindup;
                }

                if (!canShift)
                {
                    chanceDict["stepLeftChance"] = 0f;
                    chanceDict["stepRightChance"] = 0f;
                }
            }

            chanceDict["parryChance"] = 0f;
            if (playerState == PlayerController.PlayerState.lightAttack)
            {
                if (canParry)
                {
                    chanceDict["parryChance"] = parryChance * difficulty;
                    Debug.Log("parry chance: " + chanceDict["parryChance"]);
                }
            }

                #endregion

                #region Select Idle Action
                float totalChance = chanceDict.Values.Sum();
            float chanceSelected = Random.Range(0, totalChance);

            float temp= 0f;

            foreach (KeyValuePair<string, float> item in chanceDict)
            {
                if (temp <= chanceSelected && chanceSelected < (temp + item.Value))
                {
                    stateToEnter = item.Key;
                }
                else
                {
                    temp += item.Value;
                }
            }

            #endregion

            if (playerState == PlayerController.PlayerState.heavyLunge)
            {
                //Debug.Log("state to enter set to jump");
                stateToEnter = "jumpChance";
            }
        }
    }

    private void UpdateState()
    {
        //Debug.Log("state to enter:" + stateToEnter);
        if (stateToEnter == "shuffleLeftChance" &&isGrounded &&isAbleToChangeDirection && !isParrying)
        {
            state = EnemyState.shuffleLeft;
            StartCoroutine(UnableToChangeDirection());
            Debug.Log("switched to shuffleLeft");
            stateToEnter = "";
        }
        else if (stateToEnter == "stepLeftChance" &&isGrounded && isAbleToChangeDirection && !isParrying)
        {
            stepLeftTimer = 0f;
            state = EnemyState.stepLeft;
            Debug.Log("switched to stepLeft");
            stateToEnter = "";
            stepLeftDustParticle.Play();
            animator.Play("StepLeft");
            audioManager.Play("BossDash");
        }
        else if (stateToEnter == "shuffleRightChance" && isGrounded && isAbleToChangeDirection && !isParrying)
        {
            state = EnemyState.shuffleRight;
            StartCoroutine(UnableToChangeDirection());
            Debug.Log("switched to shuffleRight");
            stateToEnter = "";
        }
        else if (stateToEnter == "stepRightChance" && isGrounded && isAbleToChangeDirection && !isParrying)
        {
            stepRightTimer = 0f;
            state = EnemyState.stepRight;
            Debug.Log("switched to stepRight");
            stateToEnter = "";
            stepRightDustParticle.Play();
            animator.Play("StepRight");
            audioManager.Play("BossDash");
        }
        else if (stateToEnter == "idleChance" && isGrounded && isAbleToChangeDirection && !isParrying)
        {
            state = EnemyState.idle;
            Debug.Log("switched to idle");
            stateToEnter = "";
        }
        else if (stateToEnter == "lightAttackChance" && isGrounded && isAbleToChangeDirection && !isParrying)
        {
            state = EnemyState.lightAttackWindup;
            UIManager.ShowExclaimationText(transform.position);
            Debug.Log("switched to light attack");
            stateToEnter = "";
            swordRb.isKinematic = false;
            audioManager.Play("BossLightAttack");
            if (enemyLightAttackDamage != enemyLightAttackBaseDamage)
            {
                StartCoroutine(ShowEnemyRiposteText());
            }
        }
        else if (stateToEnter == "heavyLungeChance" && isGrounded && isAbleToChangeDirection && !isParrying)
        {
            state = EnemyState.heavyLungeWindup;
            Debug.Log("switched to heavy lunge");
            stateToEnter = "";
            swordRb.isKinematic = false;
            //sword.transform.localPosition += Vector3.left * 0.27f;
            heavyLungeWindupTime = Random.Range(minHeavyLungeWindupDuration, maxHeavyLungeWindupDuration);
            StartCoroutine(ShowHeavyLungeExclaimationText());
            StartCoroutine(HeavyLungeWindupCoroutine());
        }
        else if (stateToEnter == "jumpChance" && isGrounded &&!isParrying)
        {
            state = EnemyState.jump;
            Debug.Log("switched to jump");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            stateToEnter = "";
            isGrounded = false;
            animator.Play("Jump");
        }
        else if (stateToEnter == "jumpAttackChance" && state==EnemyState.jump)
        {
            state = EnemyState.jumpAttack;
            Debug.Log("switched to jump attack");
            audioManager.Play("BossJumpAttack");
            stateToEnter = "";
            swordPivot.localEulerAngles = new Vector3(0f, 0f, jumpAttackSwordPivotRotation);
            swordRb.isKinematic = false;
            swordPivotRb.isKinematic = false;
            swordCollider.enabled = true;
            animator.Play("JumpAttack");
        }
        else if (stateToEnter == "parryChance" && isGrounded)
        {
            state = EnemyState.parry;
            StartCoroutine(Parry());
            Debug.Log("switched to parry");
            stateToEnter = "";
            isParrying = true;

            enemyLightAttackDamage += riposteDamageBonus;
        }
    }


    private IEnumerator UnableToChangeDirection()
    {
        isAbleToChangeDirection = false;
        yield return new WaitForSeconds(unableToChangeDirectionDuration);
        isAbleToChangeDirection = true;
    }

    private IEnumerator ShowEnemyRiposteText()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.ShowRiposteText(transform.position);
    }
}

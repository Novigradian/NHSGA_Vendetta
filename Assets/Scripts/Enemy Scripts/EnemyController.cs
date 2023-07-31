using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        getHit,
        block,
        dead
    }

    public EnemyState state;
    #endregion

    #region Enemy Variables and Components
    public GameManager gameManager;
    public DialogueManager dialogueManager;

    [Header("Health")]
    public EnemyHealthBar enemyHealthBar;
    [SerializeField] private float maxEnemyHealth;
    private float enemyHealth;

    [Header("Enemy Damage")]
    public float enemyLightAttackDamage;
    private float enemyLightAttackBaseDamage;
    public float enemyHeavyLungeBaseDamage;
    public float enemyHeavyLungeExtraDamageScale;
    [HideInInspector] public float enemyHeavyLungeDamage;
    public float enemyJumpAttackDamage;

    [Header("Player")]
    public GameObject player;
    private PlayerController playerController;

    [Header("Get Hit")]
    [SerializeField] private float getHitStunDuration;

    [Header("Block")]
    [SerializeField] private float blockDuration;
    [SerializeField] private float blockDamageNegationScale;
    #endregion

    void Start()
    {
        #region Initialize Variables
        playerController = player.GetComponent<PlayerController>();

        enemyHealth = maxEnemyHealth;
        enemyHealthBar.SetMaxHealth(maxEnemyHealth);

        enemyHeavyLungeDamage = enemyHeavyLungeBaseDamage;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
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
                HeavyLungeWindupActions();
                HeavyLungeWindupTransitions();
                break;
            case EnemyState.heavyLunge:
                HeavyLungeActions();
                HeavyLungeTransitions();
                break;
            case EnemyState.heavyLungeStun:
                HeavyLungeStunActions();
                HeavyLungeStunTransitions();
                break;
            #endregion

            #region Parry Actions and Transitions
            case EnemyState.parry:
                ParryActions();
                ParryTransitions();
                break;
            #endregion

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

    #region Idle Functions
    private void IdleActions()
    {

    }

    private void IdleTransitions()
    {
        
    }
    #endregion

    #region Movement Functions
    private void StepLeftActions()
    {
        
    }

    private void StepLeftTransitions()
    {
        
    }

    private void StepRightActions()
    {
       
    }

    private void StepRightTransitions()
    {
        
    }

    private void ShuffleLeftActions()
    {
        
    }

    private void ShuffleLeftTransitions()
    {
        
    }

    private void ShuffleRightActions()
    {
        
    }

    private void ShuffleRightTransitions()
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
        enemyHealth -= damage;
        enemyHealthBar.SetHealth(enemyHealth);
        //Debug.Log("enemy damaged, remaining health: " + enemyHealth);
        state = EnemyState.getHit;

        if (playerController.isRallyOn)
        {
            playerController.AddRallyHealth(damage);
        }

        CheckDead();
    }
    #endregion

    #region Block Functions
    private void BlockActions()
    {

    }

    private void BlockTransitions()
    {

    }

    private IEnumerator ResetBlock()
    {
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
        enemyHealth -= baseDamage * blockDamageNegationScale;
        enemyHealthBar.SetHealth(enemyHealth);
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
            enemyHealth = 0f;
            state = EnemyState.dead;
            playerController.state = PlayerController.PlayerState.idle;
            playerController.ResetSwordPosition();
            player.transform.position = new Vector3(transform.position.x, -2f, transform.position.z);
            //Time.timeScale = 0;
            dialogueManager.playerDialogue.SetActive(true);
            gameManager.gameState = "PlayerWinDialogue";
        }
    }
    #endregion

    #region Collisions

    #region Ground Collision
    #endregion

    #region Player Sword Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerSword")
        {
            PlayerController.PlayerState playerState = playerController.state;

            #region Player is Idle/Moving/Jumping/Windup/Stun
            if (state == EnemyState.idle || state == EnemyState.shuffleRight || state == EnemyState.stepLeft || state == EnemyState.stepRight || state == EnemyState.lightAttackWindup || state == EnemyState.heavyLungeWindup || state == EnemyState.heavyLungeStun || state == EnemyState.jump || state == EnemyState.lightAttack)
            {
                //state = PlayerState.getHit;
                //ActivateRally();
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    TakeHitDamage(playerController.playerLightAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
            }
            #endregion

            #region Player is Blocking
            else if (state == EnemyState.shuffleLeft)
            {
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    TakeBlockDamage(playerController.playerLightAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeBlockDamage(playerController.playerJumpAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                }
            }
            #endregion

            #region Player is Jump Attacking
            else if (state == EnemyState.jumpAttack)
            {
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    TakeHitDamage(playerController.playerLightAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
            }
            #endregion

            #region Player is Parrying
            else if (state == EnemyState.parry)
            {
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    TakeHitDamage(playerController.playerHeavyLungeDamage);
                }
            }
            #endregion

            #region Player is Heavy Lunging
            else if (state == EnemyState.heavyLunge)
            {
                if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    TakeHitDamage(playerController.playerJumpAttackDamage);
                }
            }
            #endregion

            #region Player is already in Get Hit
            else if (state == EnemyState.getHit)
            {
                
                if (playerState == PlayerController.PlayerState.lightAttack)
                {
                    enemyHealth -= playerController.playerLightAttackDamage;
                    enemyHealthBar.SetHealth(enemyHealth);
                }
                else if (playerState == PlayerController.PlayerState.jumpAttack)
                {
                    enemyHealth -= playerController.playerJumpAttackDamage;
                    enemyHealthBar.SetHealth(enemyHealth);
                }
                else if (playerState == PlayerController.PlayerState.heavyLunge)
                {
                    enemyHealth -= playerController.playerHeavyLungeDamage;
                    enemyHealthBar.SetHealth(enemyHealth);
                }
                
            }
            #endregion
        }
    }
    #endregion

    #endregion
}

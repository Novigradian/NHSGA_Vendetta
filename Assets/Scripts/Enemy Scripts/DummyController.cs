using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public TutorialPlayerController playerController;
    public TutorialManager tutorialManager;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TakeHitDamage(float damage)
    {
        tutorialManager.SpawnRightBloodParticle(transform.position+new Vector3(0f,0.5f,0f));
        tutorialManager.ShowDamageText(transform.position, damage);
        //Debug.Log(transform.position);
        //gameManager.getHitVolume.SetActive(true);
        //gameManager.ResetGetHitUI();
        //Debug.Log("enemy damaged, remaining health: " + enemyHealth);
    }

    #region Enemy Sword Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string gameState = tutorialManager.gameState;
        if (collision.gameObject.tag == "PlayerSword" && (gameState=="Fight"||gameState=="Practice"))
        {
            TutorialPlayerController.PlayerState playerState = playerController.state;

            if (playerState == TutorialPlayerController.PlayerState.lightAttack)
            {
                TakeHitDamage(playerController.playerLightAttackDamage);
                if (playerController.playerLightAttackDamage != playerController.playerLightAttackBaseDamage)
                {
                    tutorialManager.ShowRiposteText(player.transform.position);
                }
            }
            else if (playerState == TutorialPlayerController.PlayerState.heavyLunge)
            {
                tutorialManager.ShowCritText(transform.position);
                TakeHitDamage(playerController.playerHeavyLungeDamage);
            }
            else if (playerState == TutorialPlayerController.PlayerState.jumpAttack)
            {
                TakeHitDamage(playerController.playerJumpAttackDamage);
            }
        }
    }
    
    #endregion
}

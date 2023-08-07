using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region GameManager Components and Variables
    private DataHolder dataHolder;

    Keyboard kb;
    public PlayerController playerController;
    public GameObject enemy;
    public GameObject player;

    public float minimumPlayerEnemyDistance;

    public string gameState;

    public GameObject dialogueVolume;
    public GameObject getHitVolume;
    public GameObject combatVolume;
    public GameObject mysteriousVoiceVolume;

    [SerializeField] private float getHitVolumeShowDuration;

    public GameObject leftBloodParticlePrefab;
    public GameObject rightBloodParticlePrefab;

    [HideInInspector] public bool isSceneTwo;

    #endregion

    void Awake()
    {
        #region Initialize Variables
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            isSceneTwo = true;
        }
        else
        {
            isSceneTwo = false;
        }

        dataHolder = FindObjectOfType<DataHolder>();
        if ((dataHolder.hasPlayerDiedLevelOne && !isSceneTwo) || (dataHolder.hasPlayerDiedLevelTwo && isSceneTwo)) 
        {
            gameState = "FightText";
            enemy.SetActive(true);
            player.SetActive(true);
            combatVolume.SetActive(true);

            //dataHolder.hasPlayerDied = false;
        }
        else
        {
            gameState = "MysteriousVoice";
            enemy.SetActive(false);
            player.SetActive(false);
            mysteriousVoiceVolume.SetActive(true);
            getHitVolume.SetActive(false);
            
        }
        
        

        kb = Keyboard.current;
        #endregion

        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == "PlayerWin")
        {
            if (kb.rKey.wasPressedThisFrame)
            {
                if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
                {
                    PlayerPrefs.SetFloat("playerHealth", playerController.playerHealth);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    Debug.Log("GAME OVER");
                }
            }
        }
        else if (gameState == "EnemyWin")
        {
            if (kb.rKey.wasPressedThisFrame)
            {
                if (!isSceneTwo)
                {
                    dataHolder.hasPlayerDiedLevelOne = true;
                }
                else
                {
                    dataHolder.hasPlayerDiedLevelTwo = true;
                }
                SceneManager.LoadScene("Level1");
            }
        }
    }

    public void ResetGetHitUI()
    {
        StartCoroutine(ResetGetHitUICoroutine());
    }

    private IEnumerator ResetGetHitUICoroutine()
    {
        yield return new WaitForSeconds(getHitVolumeShowDuration);
        if (playerController.playerHealth >= 20)
        {
            getHitVolume.SetActive(false);
        }
        
    }

    public void SpawnLeftBloodParticle(Vector3 WorldPos)
    {
        GameObject bloodParticle = Instantiate(leftBloodParticlePrefab, WorldPos,Quaternion.identity);
        StartCoroutine(DestroyBloodParticle(bloodParticle));
    }

    public void SpawnRightBloodParticle(Vector3 WorldPos)
    {
        GameObject bloodParticle = Instantiate(rightBloodParticlePrefab, WorldPos, Quaternion.identity);
        StartCoroutine(DestroyBloodParticle(bloodParticle));
    }

    private IEnumerator DestroyBloodParticle(GameObject particle)
    {
        yield return new WaitForSeconds(15f);
        Destroy(particle);
    }
}

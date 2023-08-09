using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region GameManager Components and Variables

    Keyboard kb;
    public PlayerController playerController;
    public GameObject enemy;
    public GameObject player;

    public UIManager uiManager;
    public AudioManager audioManager;

    public float minimumPlayerEnemyDistance;

    public string gameState;

    public GameObject dialogueVolume;
    public GameObject getHitVolume;
    public GameObject combatVolume;
    public GameObject mysteriousVoiceVolume;

    public MusicManager musicManager;

    [SerializeField] private float getHitVolumeShowDuration;

    public GameObject leftBloodParticlePrefab;
    public GameObject rightBloodParticlePrefab;

    public bool isSceneTwo;
    [HideInInspector] public string killChoice;
    private bool hasPlayed;
    [SerializeField] private float maxDamageModifier;
    [SerializeField] private float minDamageModifier;

    #endregion

    void Awake()
    {
        #region Initialize Variables
        uiManager = FindObjectOfType<UIManager>();
        audioManager = FindObjectOfType<AudioManager>();
        musicManager = FindObjectOfType<MusicManager>();
        hasPlayed = false;
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            isSceneTwo = true;
        }
        else
        {
            isSceneTwo = false;
        }
        Debug.Log("has level one dialogue shown: " + DataHolder.hasLevelOneDialogueShown);
        Debug.Log("has level two dialogue shown: " + DataHolder.hasLevelTwoDialogueShown);
        Debug.Log("isSceneTwo: " + isSceneTwo);
        if ((DataHolder.hasLevelOneDialogueShown && !isSceneTwo) || (DataHolder.hasLevelTwoDialogueShown && isSceneTwo)) 
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
            if (!hasPlayed)
            {
                audioManager.Play("Victory");
                musicManager.StartMuffle();
                hasPlayed = true;
            }
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
                    DataHolder.hasLevelOneDialogueShown = false;
                    DataHolder.hasLevelTwoDialogueShown = false;
                    SceneManager.LoadScene(0);
                }
            }
        }
        else if (gameState == "EnemyWin")
        {
            if (!hasPlayed)
            {
                audioManager.Play("Defeat");
                musicManager.StartMuffle();
                hasPlayed = true;
            }
            
            if (kb.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Level1");
            }
        }
        else if (gameState == "KillChoice")
        {
            if (kb.pKey.wasPressedThisFrame)
            {
                uiManager.killChoice.SetActive(false);
                //killChoice = "KillScene";
                //gameState = "PlayerWinDialogue";
                SceneManager.LoadScene("KillScene");
            }
            else if (kb.oKey.wasPressedThisFrame)
            {
                uiManager.killChoice.SetActive(false);
                //killChoice = "SpareScene";
                //gameState = "PlayerWinDialogue";
                SceneManager.LoadScene("SpareScene");
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

    public float RandomDamageModifier()
    {
        return Random.Range(minDamageModifier, maxDamageModifier);
    }
}

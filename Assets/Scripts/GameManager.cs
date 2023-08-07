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
        gameState = "MysteriousVoice";
        enemy.SetActive(false);
        player.SetActive(false);

        kb = Keyboard.current;
        #endregion

        mysteriousVoiceVolume.SetActive(true);
        getHitVolume.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            isSceneTwo = true;
        }
        else
        {
            isSceneTwo = false;
        }
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
        getHitVolume.SetActive(false);
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

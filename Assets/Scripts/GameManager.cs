using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region GameManager Components and Variables
    public float minimumPlayerEnemyDistance;

    public string gameState;

    public GameObject dialogueVolume;
    public GameObject getHitVolume;

    [SerializeField] private float getHitVolumeShowDuration;
    #endregion

    void Awake()
    {
        #region Initialize Variables
        gameState = "PreFight";
        #endregion

        dialogueVolume.SetActive(true);
        getHitVolume.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}

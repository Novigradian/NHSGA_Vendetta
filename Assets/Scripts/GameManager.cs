using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region GameManager Components and Variables
    public float minimumPlayerEnemyDistance;

    public string gameState;

    public GameObject dialogueVolume;
    public GameObject fightVolume;
    #endregion

    void Awake()
    {
        #region Initialize Variables
        gameState = "PreFight";
        #endregion

        dialogueVolume.SetActive(true);
        fightVolume.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

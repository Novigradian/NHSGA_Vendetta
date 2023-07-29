using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxHealth;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int playerAttackValue;
    public player player;
    public BossHealthBar bossHealthBar;
    public PlayerHealthBar playerHealthBar;
    private Vector2 initPosition;
    public GameObject sword;
    private int direction = -1;
    private float swordDistance;
    void Start()
    {
        currentHealth = maxHealth;
        bossHealthBar.SetMaxHealth(currentHealth);
        initPosition = sword.transform.position;
        swordDistance = Vector3.Distance(transform.position, initPosition);
    }

    // Update is called once per frame
    void Update()
    {
        checkHealth();
        sword.GetComponent<Rigidbody2D>().position = new Vector2(gameObject.transform.position.x + swordDistance * direction, gameObject.transform.position.y + 1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("sword"))
        {
            currentHealth = currentHealth - playerAttackValue;
            bossHealthBar.SetHealth(currentHealth);
            //Debug.Log(playerAttackValue);
            //Debug.Log(currentHealth);
            if(player.rallyOn == true)
            {
                player.currentHealth += (int)Mathf.Round(playerAttackValue * player.rallyPercentage);
                //Debug.Log((int)Mathf.Round(playerAttackValue * player.rallyPercentage));
                playerHealthBar.SetHealth(player.currentHealth);
            }
        }
    }
    private void checkHealth()
    {
        if (currentHealth == 0)
        {
            gameObject.SetActive(false);
        }
    }
}

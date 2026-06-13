using NUnit.Framework;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Statistiques")]
    [SerializeField] 
    public float maxHealth = 100f;
    [SerializeField]
    private float currentHealth;

    public bool isInvulnerable = false;

    public bool isInvincible = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].setHealth((int)currentHealth);
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].makeInvincible(isInvincible);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;
        if (isInvincible) return;

        currentHealth -= damage;
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].setHealth((int)currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GetComponent<PlayerMovement>().Die();
    }

    public void makeInvulnerable(bool value)
    {
        isInvulnerable = value;
        FindObjectsByType<GameSession>(FindObjectsSortMode.None)[0].makeInvincible(isInvincible);
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    public void makeInvincible(bool value)
    {
        isInvincible = value;
    }
    public bool IsInvincible()
    {
        return isInvincible;
    }
}

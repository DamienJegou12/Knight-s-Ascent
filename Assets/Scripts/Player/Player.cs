using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Statistiques")]
    [SerializeField] 
    public float maxHealth = 100f;
    [SerializeField]
    private float currentHealth;

    public bool isInvulnerable = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

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
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
}

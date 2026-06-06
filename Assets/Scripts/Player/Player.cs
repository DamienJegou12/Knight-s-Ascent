using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Statistiques")]
    [SerializeField] 
    public float maxHealth = 100f;
    [SerializeField]
    private float currentHealth;
    
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
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        // if (currentHealth <= 0)
        // {
        //     Die();
        // }
    }
}

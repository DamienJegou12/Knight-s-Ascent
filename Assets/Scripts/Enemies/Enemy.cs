using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public int health = 10;
    private LootBag lootBag;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lootBag = GetComponent<LootBag>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        lootBag.InstantiateLoot(transform.position);
        Destroy(gameObject);
    }
}

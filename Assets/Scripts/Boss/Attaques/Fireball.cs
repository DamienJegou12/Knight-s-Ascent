using UnityEngine;
using UnityEngine.Tilemaps;

// Code de Damien JEGOU 

public class Fireball : Bullet
{
    private Animator animator;
    /*[Header("Effet à appliquer au contact (optionnel)")]
    public Ieffet effectOnHitPrefab; // prefab qui contient un composant dérivant de Ieffet (ex: effet_Brulure)
    public int effectStacks = 1;*/
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        c1 = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        // Lancer l'animation de la boule de feu
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Launch");
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // V�rifie si l'objet entrant en collision a bien le tag "ennemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {

            Player Ennemi = collision.gameObject.GetComponent<Player>();
            if (Ennemi != null)
            {
                Debug.Log("Collision avec un ennemi !");
                Ennemi.TakeDamage(damage);

                Destroy(gameObject);
            }
        }
        else if (collision.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject); // D�truit le projectile sans appliquer de d�g�ts
        }
    }
}

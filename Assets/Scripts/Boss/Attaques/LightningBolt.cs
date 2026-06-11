using UnityEngine;
using System.Collections.Generic;

// Code fait par Damien JEGOU
public class LightningBolt : MonoBehaviour
{
    [Header("Combat")]
    public int damage = 6;
    public int speed = 10;
    
    [Header("Environnement")]
    public LayerMask groundLayer; // À assigner dans l'inspecteur pour définir ce qu'est le sol

    private Rigidbody2D rb;
    private CapsuleCollider2D myCollider;
    private Animator anim;
    
    private bool hasStruck = false; // Permet de savoir si l'éclair a déjà touché sa cible

    void Start()
    {
        myCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (myCollider == null)
            Debug.LogError("Attention : Il manque un CapsuleCollider2D sur l'éclair !");
        
        if (anim == null)
            Debug.LogError("Attention : Il manque un Animator sur l'éclair !");
    }

    void Update()
    {
        // On ne fait descendre l'éclair que s'il n'a pas encore explosé
        if (!hasStruck)
        {
            rb.linearVelocity = new Vector2(0, -speed); 
        }
    }

    // ---------------------------------------------------------
    // DÉTECTION DE L'IMPACT (Quand la capsule verte touche)
    // ---------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si l'éclair a déjà frappé, on ignore les autres collisions
        if (hasStruck) return;

        // On vérifie si on a touché le joueur...
        Player playerHit = collision.GetComponent<Player>();
        
        // ... ou si on a touché le sol (grâce au Layer de l'objet)
        bool isGround = ((1 << collision.gameObject.layer) & groundLayer) != 0;

        if (playerHit != null || isGround)
        {
            hasStruck = true;                 // On bloque le mouvement
            rb.linearVelocity = Vector2.zero;       // On arrête la vélocité nette
            anim.SetTrigger("End");           // On lance l'animation d'explosion
        }
    }

    // ---------------------------------------------------------
    // EVENT D'ANIMATION : DÉGÂTS (Frame d'impact)
    // ---------------------------------------------------------
    public void ApplyDamageInstant()
    {
        if (myCollider == null) return;

        List<Collider2D> hits = new List<Collider2D>();
        myCollider.Overlap(new ContactFilter2D().NoFilter(), hits);

        HashSet<Player> damagedEntitiesGeneral = new HashSet<Player>();

        foreach (Collider2D hit in hits)
        {
            Player entity = hit.gameObject.GetComponent<Player>();
            
            if (entity != null && !damagedEntitiesGeneral.Contains(entity))
            {
                entity.TakeDamage(damage);
                damagedEntitiesGeneral.Add(entity);
            }
        }
    }

    // ---------------------------------------------------------
    // EVENT D'ANIMATION : FIN (Dernière frame)
    // ---------------------------------------------------------
    public void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
}
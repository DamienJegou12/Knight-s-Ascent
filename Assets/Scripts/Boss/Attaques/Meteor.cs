using UnityEngine;
using System.Collections.Generic;

// Code fait par Damien JEGOU
public class Meteor : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 15f; // Vitesse du projectile
    
    [Header("Combat")]
    public int damage = 20;

    private Vector3 targetPosition;
    private bool hasLanded = false;
    private Animator animator;
    private CircleCollider2D explosionRadius;

    void Awake()
    {
        animator = GetComponent<Animator>();
        explosionRadius = GetComponent<CircleCollider2D>();
        
        // Sécurité : On s'assure que le collider est éteint pendant le vol
        if (explosionRadius) explosionRadius.enabled = false;
    }

    public void Initialize(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        targetPosition = endPos;
        
        // Calcule l'angle pour orienter parfaitement le sprite du météore vers sa trajectoire
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Note : Si la pointe de ton sprite ne regarde pas dans la bonne direction, 
        // c'est ici qu'il faut ajuster le "+ 140" (ou l'enlever selon l'orientation de ton image d'origine).
        transform.rotation = Quaternion.Euler(0, 0, angle + 140);
    }

    void Update()
    {
        if (hasLanded) return; 

        // Avance de façon fluide vers la cible
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Vérifie si on a atteint le point d'impact
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Land();
        }
    }

    void Land()
    {
        hasLanded = true;
        transform.position = targetPosition; // Force la position exacte au sol
        
        if (animator != null)
        {
            // Lance l'animation d'explosion
            animator.SetTrigger("land");
        }
        else
        {
            // Sécurité si pas d'animator
            Explode();
            OnAnimationFinish();
        }
    }

    // ---------------------------------------------------------
    // ANIMATION EVENTS 
    // ---------------------------------------------------------
    
    // À placer sur la frame "BOUM" de l'animation
    public void Explode()
    {
        if (explosionRadius == null) return; 
        
        explosionRadius.enabled = true; 
        
        List<Collider2D> hits = new List<Collider2D>();
        explosionRadius.Overlap(new ContactFilter2D().NoFilter(), hits);
        
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

    // À placer sur la toute dernière frame de l'animation
    public void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
}
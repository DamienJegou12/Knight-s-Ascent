using UnityEngine;
using System.Collections.Generic;

// Code fait par Damien JEGOU
public class Meteor : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 15f; // Vitesse du projectile
    
    [Header("Combat")]
    public int damage = 20;

    [Header("Visuel")]
    public GameObject warningPrefab;

    private Vector3 targetPosition;
    private bool hasLanded = false;
    private Animator animator;
    private CircleCollider2D explosionRadius;
    private GameObject activeWarning;


    void Awake()
    {
        animator = GetComponent<Animator>();
        explosionRadius = GetComponent<CircleCollider2D>();
        
        // Sécurité : On s'assure que le collider est éteint pendant le vol
        if (explosionRadius) explosionRadius.enabled = false;
    }

    // Cette fonction sera appelée par le Controller pour donner la trajectoire
    public void Initialize(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        targetPosition = endPos;
        
        // Optionnel : Orienter le météore vers sa cible (rotation)
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 140);

        if(warningPrefab != null)
        {
            activeWarning = Instantiate(warningPrefab, targetPosition, Quaternion.identity);
        }
    }

    void Update()
    {
        if (hasLanded) return; // Si on a atterri, on ne bouge plus

        // On avance vers la cible
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Vérifier si on est arrivé (très proche de la cible)
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Land();
        }
    }

    void Land()
    {
        hasLanded = true;
        transform.position = targetPosition; // On force la position exacte
        if(activeWarning != null)
        {
            Destroy(activeWarning);
        }
        // On dit à l'Animator de jouer l'explosion
        if (animator != null)
        {
            animator.SetTrigger("land");
        }
        else
        {
            // Si pas d'animator, on explose direct (pour tester)
            Explode();
            OnAnimationFinish();
        }
    }

    // ---------------------------------------------------------
    // ANIMATION EVENTS (Identique à avant)
    // ---------------------------------------------------------
    
    // Placer cet event sur la frame "BOUM" de l'animation d'impact
    public void Explode()
    {
        if (explosionRadius == null) return; // Sécurité si pas de collider
        explosionRadius.enabled = true; // Active le collider d'explosion
        // On récupère tous les colliders dans le rayon d'explosion
        List<Collider2D> hits = new List<Collider2D>();
        explosionRadius.Overlap(new ContactFilter2D().NoFilter(), hits);
        
        // Pour éviter d'infliger des dégâts multiples à la même entité on prepare une liste
        // HashSet<Entity> damagedEntitiesGeneral = new HashSet<Entity>();

        // foreach (Collider2D hit in hits)
        // {
            
        //     Entity entity = hit.gameObject.GetComponent<Entity>();
        //     // SI c'est une entité ET qu'on ne l'a pas encore blessée dans cette boucle
        //     if (entity != null && !damagedEntitiesGeneral.Contains(entity))
        //     {
        //         // On applique les dégâts
        //         entity.dealDammage(damage);
                
                

        //         // On l'ajoute à la liste "Déjà touché"
        //         damagedEntitiesGeneral.Add(entity);
        //     }
        // }
    }

    // Placer cet event à la toute fin de l'animation
    public void OnAnimationFinish()
    {
        Destroy(gameObject);
    }
}
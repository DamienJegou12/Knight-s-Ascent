using UnityEngine;
using UnityEngine.Tilemaps;

// Code fait par Damien JEGOU
public class Bullet : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    public PolygonCollider2D parentCollider;
    public int damage;
    // public Ieffet effectOnHitPrefab;
    // public int effectStacks = 1;
    public int rebondsRestants;
    public int bulletDistance;
    public bool useBulletDistance;
    public GameObject nextCible; // La prochaine cible pour le rebond
    public GameObject actualCible; // La cible avant le rebond
    protected Vector3 startPosition; // Position de départ pour le calcul de distance

    public PolygonCollider2D c1;
    public Rigidbody2D rb;


    private void Start()
    {
        c1 = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Enregistre la position de départ

    }

    void Update()
    {
        if (nextCible == null) // Pas de cible pour le rebond, tir droit
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        else // Rebond vers la prochaine cible
        {
            // Calcul de la direction vers la cible
            Vector2 directionToCible = (nextCible.transform.position - transform.position).normalized;
            rb.linearVelocity = directionToCible * speed;
            float angle = Mathf.Atan2(directionToCible.y, directionToCible.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        if (useBulletDistance && Vector2.Distance(transform.position, startPosition) >= bulletDistance) // Vérifie la distance maximale
        {
            Destroy(gameObject); // Détruit le projectile
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
        
    //     // Vérifier d'abord si ce n'est pas le joueur (pour éviter de se tirer dessus)
    //     Player player = collision.gameObject.GetComponent<Player>();
    //     if (player != null)
    //     {
    //         return; // Ignorer la collision avec le joueur
    //     }
    //     if (actualCible != null && collision.gameObject == actualCible) 
    //     {
    //         return;
    //     }
        
    //     // Vérifie si l'objet a le tag "Enemy" et le composant Ennemi
    //     if (collision.gameObject.CompareTag("Enemy"))
    //     {
    //         Ennemi ennemi = collision.gameObject.GetComponent<Ennemi>();
    //         if (ennemi != null)
    //         {
    //             ennemi.dealDammage(damage); // Applique les dégâts
    //             if (effectOnHitPrefab != null)
    //             {
    //                 var manager = collision.gameObject.GetComponent<effetManager>();
    //                 if (manager != null)
    //                 {
    //                     manager.AddEffect(effectOnHitPrefab, effectStacks); // Applique l'effet
    //                 }
    //             }
    //             if (rebondsRestants > 0) // Gère les rebonds
    //             {
    //                 actualCible = collision.gameObject; // Met à jour la cible actuelle
    //                 rebond();
    //                 return;
    //             }
    //             else // Pas de rebond restant, détruit le projectile
    //             {
    //                 Destroy(gameObject); // Détruit le projectile après impact
    //                 return;
    //             }
    //         }
    //     }
        
    //     // Vérifie si c'est une entité (pour les Boids et autres ennemis sans tag "Enemy")
    //     Entity entity = collision.gameObject.GetComponent<Entity>();
    //     if (entity != null && player == null) // Double vérification pour ne pas toucher le joueur
    //     {
    //         entity.dealDammage(damage);
    //         if (effectOnHitPrefab != null)
    //             {
    //                 var manager = collision.gameObject.GetComponent<effetManager>();
    //                 if (manager != null)
    //                 {
    //                     manager.AddEffect(effectOnHitPrefab, effectStacks); // Applique l'effet
    //                 }
    //             }
    //             if (rebondsRestants > 0) // Gère les rebonds
    //             {
    //                 actualCible = collision.gameObject; // Met à jour la cible actuelle
    //                 rebond();
    //                 return;
    //             }
    //             else // Pas de rebond restant, détruit le projectile
    //             {
    //                 Destroy(gameObject); // Détruit le projectile après impact
    //                 return;
    //             }
    //         Destroy(gameObject);
    //         return;
    //     }
        
    //     // Collision avec un obstacle (Tilemap)
    //     if (collision.GetComponent<TilemapCollider2D>() != null)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
        
    // }

    GameObject EnnemisPlusProche()
    {
        // Renvoie un tableau contenant tous les objets actifs avec le tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0)
        {
            return null;
        }

        // Si il n'y a qu'un seul ennemi et que c'est la cible actuelle, il n'y a pas de nouvelle cible
        if (enemies.Length == 1)
        {
            if (enemies[0] == actualCible) return null;
            return enemies[0];
        }

        // Plus d'une cible: ignore la cible actuelle lors de la recherche
        GameObject closestEnemy = null;
        float bestDistance = float.MaxValue;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == actualCible) continue;
            if (enemies[i] == null) continue;
            float d = Vector3.Distance(transform.position, enemies[i].transform.position);
            if (d < bestDistance)
            {
                bestDistance = d;
                closestEnemy = enemies[i];
            }
        }

        return closestEnemy;
    }

    private void rebond()
    {
        // Gère le rebond vers la prochaine cible
        if(useBulletDistance)
        {
            if (bulletDistance > 0)
            {
                bulletDistance -= (int)Vector2.Distance(startPosition, actualCible.transform.position); // Réduit la distance restante
                startPosition = actualCible.transform.position; // Met à jour la position de départ après le rebond
            }
            if (bulletDistance <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }
        nextCible = EnnemisPlusProche(); // Trouve la prochaine cible
        if (nextCible == null) // Pas de cible trouvée, détruit le projectile
        {
            Destroy(gameObject);
            return;
        }
        rebondsRestants -= 1; // Décrémente le nombre de rebonds restants
    }
}

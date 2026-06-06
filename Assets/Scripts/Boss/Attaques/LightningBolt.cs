using UnityEngine;
using System.Collections.Generic;

// Code fait par Damien JEGOU
public class LightningBolt : MonoBehaviour
{
    [Header("Combat")]
    public int damage = 6;
    public int speed = 10;


    // public Ieffet effectOnHitPrefab;
    // public int effectStacks = 1;
    private Rigidbody2D rb;
    
    // On garde la variable privée car on va la remplir automatiquement
    private CapsuleCollider2D myCollider;

    void Start()
    {
        // 1. On récupère le collider qui est sur le MÊME objet que ce script
        myCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (myCollider == null)
        {
            Debug.LogError("Attention : Il manque un CapsuleCollider2D sur l'éclair !");
        }
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(0, -speed); 
    }

    // ---------------------------------------------------------
    // EVENT D'ANIMATION : DÉGÂTS (Frame d'impact)
    // ---------------------------------------------------------
    public void ApplyDamageInstant()
    {
        if (myCollider == null) return;

        // Préparer une liste pour stocker tout ce que la capsule touche
        List<Collider2D> hits = new List<Collider2D>();
        
        // Cette fonction regarde INSTANTANÉMENT ce qu'il y a dans la capsule
        // Elle ne dépend pas de la physique (Rigidbody), c'est une vérification géométrique
        myCollider.Overlap(new ContactFilter2D().NoFilter(), hits);

        // Cette liste sert à se souvenir des entités déjà touchées PAR CET ÉCLAIR PRÉCIS
        // (Utile si un ennemi a 2 colliders, ex: corps + tête, pour ne pas prendre 2x les dégâts)
        HashSet<Player> damagedEntitiesGeneral = new HashSet<Player>();

        foreach (Collider2D hit in hits)
        {
            

            // On essaie de récupérer le script Entity sur l'objet touché
            Player entity = hit.gameObject.GetComponent<Player>();
            // SI c'est une entité ET qu'on ne l'a pas encore blessée dans cette boucle
            if (entity != null && !damagedEntitiesGeneral.Contains(entity))
            {
                // On applique les dégâts
                entity.TakeDamage(damage);

                // On l'ajoute à la liste "Déjà touché"
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
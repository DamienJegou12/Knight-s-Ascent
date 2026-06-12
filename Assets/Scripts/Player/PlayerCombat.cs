using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    public int degats = 10;
    public float delaiMaximumCombo = 0.6f; // Temps max pour enchaîner

    [Header("Détection des coups")]
    public Transform pointDAttaque; // Un objet vide placé devant le joueur
    public float rayonAttaque = 0.5f;
    public LayerMask layerEnnemi;   // Pour ne détecter que les ennemis

    // Variables d'état
    private int etapeCombo = 0;
    private float tempsDerniereAttaque = 0f;
    private bool veutEnchainer = false;
    private bool estEnTrainDAttaquer = false;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Chronomètre pour savoir si on a attendu trop longtemps
        tempsDerniereAttaque += Time.deltaTime;
        
        if (tempsDerniereAttaque > delaiMaximumCombo && !estEnTrainDAttaquer)
        {
            etapeCombo = 0; // Réinitialise le combo
        }

        
    }

    void OnAttack(InputValue value)
    {
        if (!value.isPressed) { return; }
        Debug.Log("Attack Input: " + value);
        GererInputAttaque();
    }

    void GererInputAttaque()
    {
        if(GetComponent<PlayerMovement>().isAlive == false) { return; }
        if (!estEnTrainDAttaquer)
        {
            // Début de la première attaque
            estEnTrainDAttaquer = true;
            etapeCombo = 1;
            tempsDerniereAttaque = 0f;
            anim.SetTrigger("Attack");
        }
        else if (etapeCombo == 1 && tempsDerniereAttaque < delaiMaximumCombo)
        {
            // Le joueur clique PENDANT l'attaque 1 : on mémorise qu'il veut enchaîner
            veutEnchainer = true;
        }
    }

    // --------------------------------------------------------
    // FONCTIONS APPELÉES PAR LES ÉVÉNEMENTS D'ANIMATION
    // --------------------------------------------------------

    // À placer dans l'animation, au moment précis où l'arme frappe
    public void AppliquerDegats()
    {
        // Crée un cercle invisible pour détecter tout ce qui s'y trouve
        Collider2D[] ennemisTouches = Physics2D.OverlapCircleAll(pointDAttaque.position, rayonAttaque, layerEnnemi);
        Debug.Log("Ennemis touchés : " + ennemisTouches.Length);
        // Applique les dégâts à chaque ennemi touché
        foreach (Collider2D ennemi in ennemisTouches)
        {
            // On appelle le script de l'ennemi (à adapter selon le nom de ton script)
            

            WizardManager wizardManager = ennemi.GetComponent<WizardManager>();
            // WizardManager wizardManager = ennemi.GetComponentInParent<WizardManager>();
            Debug.Log("WizardManager script found: " + (wizardManager != null));
            if (wizardManager != null)
            {
                wizardManager.TakeDamage(degats);
                return;
            }

            Enemy ennemiScript = ennemi.GetComponent<Enemy>();
            if (ennemiScript != null)
            {
                ennemiScript.TakeDamage(degats);
            }
        }
    }

    // À placer vers la fin de l'animation "Attaque_1"
    public void VerifierCombo()
    {
        if (veutEnchainer)
        {
            // Le joueur a recliqué : on lance la deuxième attaque
            etapeCombo = 2;
            veutEnchainer = false;
            anim.SetTrigger("Attack 2");
        }
    }

    // À placer à la TOUTE FIN des animations "Attaque_1" et "Attaque_2"
    public void FinAttaque()
    {
        estEnTrainDAttaquer = false;
        veutEnchainer = false;
        
        if (etapeCombo == 2)
        {
            etapeCombo = 0; // On a fini le combo max, on repart à zéro
        }
    }

    // (Optionnel) Dessine un cercle rouge dans l'éditeur pour t'aider à placer ta zone de frappe
    void OnDrawGizmosSelected()
    {
        if (pointDAttaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pointDAttaque.position, rayonAttaque);
    }
}


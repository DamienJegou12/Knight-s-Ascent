using System.Collections;
using UnityEngine;

public class WizardCombat : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    public float attackCooldown = 3f;
    public Transform firePoint;
    public Transform player;

    [Header("Prefabs de Sorts")]
    public GameObject fireballPrefab;
    public GameObject StormControllerPrefab;
    public GameObject meteorPrefab;

    [Header("Nombre de foudres par phase")]
    public int maxLightningCountPhase1 = 3;
    public int maxLightningCountPhase2 = 5;
    public int maxLightningCountPhase3 = 8;

    private WizardManager.BossPhase currentPhase;
    private float nextAttackTime = 0f;

    // Ajout d'une référence au mouvement pour le bloquer
    private WizardMovement movementScript;

    [SerializeField]
    private float meteorExhaustionTimeMin = 10f; 
    [SerializeField]
    private float meteorExhaustionTimeMax = 20f; 

    void Start()
    {
        movementScript = GetComponent<WizardMovement>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // On récupère le cooldown spécifique à l'attaque qui vient d'être lancée
            Transform playerTransform = movementScript.GetPlayerTransform();
            if (playerTransform.position.x - transform.position.x > 8f)
            {
                // Si le joueur est trop loin, on ne lance pas d'attaque et on attend un peu
                nextAttackTime = Time.time + 1f; // Attente de 1 seconde avant de réessayer
                return;
            }
            float cooldownToApply = ExecuteRandomAttack();
            nextAttackTime = Time.time + cooldownToApply;
        }
    }

    public void SetPhase(WizardManager.BossPhase phase)
    {
        currentPhase = phase;
        
        // Optionnel : réduire le cooldown d'attaque quand il s'énerve
        if (phase == WizardManager.BossPhase.Phase2) attackCooldown = 2.5f;
        if (phase == WizardManager.BossPhase.Phase3)
        {
            attackCooldown = 1.8f;
            StopLightning(); // Arrête les foudres en cours
        }
    }

    // Changement : La fonction retourne maintenant un float (le temps d'attente avant la prochaine attaque)
    private float ExecuteRandomAttack()
    {
        int attackChoice = 0;

        switch (currentPhase)
        {
            case WizardManager.BossPhase.Phase1:
                attackChoice = 1; // Uniquement boule de feu
                break;
            case WizardManager.BossPhase.Phase2:
                attackChoice = Random.Range(1, 3); // Boule de feu ou Éclair
                break;
            case WizardManager.BossPhase.Phase3:
                attackChoice = Random.Range(1, 4); // Boule de feu, Éclair ou Météores
                break;
        }

        switch (attackChoice)
        {
            case 1:
                CastFireball();
                return attackCooldown; // Cooldown normal
            case 2:
                CastLightning();
                return attackCooldown; // Cooldown normal
            case 3:
                return CastMeteor();   // Cooldown spécial d'épuisement !
        }
        
        return attackCooldown;
    }

    private void CastFireball()
    {
        Debug.Log("Attaque: Boule de feu !, pos player = " + player.position);
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        
        Vector2 direction = (player.position - firePoint.position).normalized;
        Fireball bulletScript = fireball.GetComponent<Fireball>();
        if (bulletScript != null)
        {
            bulletScript.direction = direction;
        }
    }

    private void CastLightning()
    {
        Debug.Log("Attaque: Éclair du ciel !");
        StormControllerPrefab.GetComponent<StormController>().LaunchLightning();
    }

    private float CastMeteor()
    {
        Debug.Log("Attaque Ultime: Météore Géant !");

        // 1. Définir le point de départ (très haut dans le ciel, un peu aléatoire sur l'axe X)
        Vector3 startPos = new Vector3(player.position.x + Random.Range(-3f, 3f), player.position.y + 15f, 0);
        
        // 2. Définir la cible (la position actuelle du joueur)
        Vector3 targetPos = player.position;

        // 3. Créer le météore et l'initialiser
        GameObject meteorObj = Instantiate(meteorPrefab, startPos, Quaternion.identity);
        Meteor meteorScript = meteorObj.GetComponent<Meteor>();
        if (meteorScript != null)
        {
            meteorScript.Initialize(startPos, targetPos);
        }

        // 4. Calculer le temps d'épuisement (entre 10 et 15 secondes)
        float exhaustionTime = Random.Range(meteorExhaustionTimeMin, meteorExhaustionTimeMax);

        // 5. Lancer la Coroutine pour bloquer les mouvements du boss
        StartCoroutine(ExhaustionRoutine(exhaustionTime));

        // On retourne ce grand temps pour bloquer les autres attaques
        return exhaustionTime; 
    }

    private IEnumerator ExhaustionRoutine(float duration)
    {
        Debug.Log("Le boss est épuisé pendant " + duration + " secondes !");
        
        // On désactive le script de mouvement (le boss s'arrête de fuir/dash)
        if (movementScript != null) movementScript.enabled = false;

        yield return new WaitForSeconds(duration);

        // On réactive le mouvement à la fin
        if (movementScript != null) movementScript.enabled = true;
        Debug.Log("Le boss a récupéré son énergie !");
    }

    public int GetMaxLightningCount()
    {
        switch (currentPhase)
        {
            case WizardManager.BossPhase.Phase1: return maxLightningCountPhase1;
            case WizardManager.BossPhase.Phase2: return maxLightningCountPhase2;
            case WizardManager.BossPhase.Phase3: return maxLightningCountPhase3;
            default: return maxLightningCountPhase1;
        }
    }

    public void StopLightning()
    {
        StormControllerPrefab.GetComponent<StormController>().StopStorm();
    }
}
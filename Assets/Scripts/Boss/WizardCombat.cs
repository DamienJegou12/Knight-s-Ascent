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
    public GameObject lightningPrefab;
    public GameObject meteorPrefab;

    private WizardManager.BossPhase currentPhase;
    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            ExecuteRandomAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void SetPhase(WizardManager.BossPhase phase)
    {
        currentPhase = phase;
        
        // Optionnel : réduire le cooldown d'attaque quand il s'énerve
        if (phase == WizardManager.BossPhase.Phase2) attackCooldown = 2.5f;
        if (phase == WizardManager.BossPhase.Phase3) attackCooldown = 1.8f;
    }

    private void ExecuteRandomAttack()
    {
        // Sélection aléatoire basée sur la phase
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
                break;
            case 2:
                CastLightning();
                break;
            case 3:
                CastMeteorShower();
                break;
        }
    }

    private void CastFireball()
    {
        Debug.Log("Attaque: Boule de feu !");
        // Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        // La logique de déplacement de la boule de feu doit être gérée dans un script sur le prefab
    }

    private void CastLightning()
    {
        Debug.Log("Attaque: Éclair du ciel !");
        // On fait apparaitre l'éclair au-dessus de la position actuelle du joueur
        Vector3 lightningPos = new Vector3(player.position.x, player.position.y + 5f, 0);
        // Instantiate(lightningPrefab, lightningPos, Quaternion.identity);
    }

    private void CastMeteorShower()
    {
        Debug.Log("Attaque Ultime: Météores !");
        StartCoroutine(MeteorRoutine());
    }

    private IEnumerator MeteorRoutine()
    {
        // Fait tomber 5 météores aléatoirement autour du joueur
        for (int i = 0; i < 5; i++)
        {
            float randomX = player.position.x + Random.Range(-5f, 5f);
            Vector3 meteorPos = new Vector3(randomX, player.position.y + 10f, 0);
            // Instantiate(meteorPrefab, meteorPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f); // Délai entre chaque météore
        }
    }
}
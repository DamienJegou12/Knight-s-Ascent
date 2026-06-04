using UnityEngine;

public class WizardManager : Enemy
{
    
    [Header("Statistiques")]
    public float maxHealth = 1000f;
    private float currentHealth;

    [Header("Références")]
    public WizardCombat combatScript;
    public WizardMovement movementScript;

    // Définition des phases
    public enum BossPhase { Phase1, Phase2, Phase3 }
    public BossPhase currentPhase = BossPhase.Phase1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        UpdatePhase();
    }

    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        CheckHealthThresholds();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckHealthThresholds()
    {
        float healthPercentage = currentHealth / maxHealth;

        // Phase 2 à 60% HP, Phase 3 à 30% HP
        if (healthPercentage <= 0.3f && currentPhase != BossPhase.Phase3)
        {
            currentPhase = BossPhase.Phase3;
            UpdatePhase();
        }
        else if (healthPercentage <= 0.6f && currentPhase == BossPhase.Phase1)
        {
            currentPhase = BossPhase.Phase2;
            UpdatePhase();
        }
    }

    private void UpdatePhase()
    {
        // On informe le script de combat que la phase a changé
        combatScript.SetPhase(currentPhase);
    }

    private void Die()
    {
        Debug.Log("Le boss est vaincu !");
        // Ajouter ici les animations de mort, loot, etc.
        Destroy(gameObject);
    }
}

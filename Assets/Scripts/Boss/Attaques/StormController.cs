using System.Collections;
using UnityEngine;

// Code fait par Damien JEGOU
public class StormController : MonoBehaviour
{
    [Header("Paramètres de l'Orage")]
    public GameObject[] lightningPrefabs; // Ta liste d'éclairs
    
    [Header("Références")]
    public WizardManager wizard; // Le wizard qui lance l'orage
    public Transform playerTransform; // Le joueur (à assigner dans l'inspecteur)

    [Header("Paramètres de la Cascade")]
    public float spaceBetweenLightnings = 2.0f; // Distance fixe (en X) entre chaque éclair
    public float timeBetweenLightnings = 0.2f;  // Temps fixe entre l'apparition de chaque éclair
    public float spawnHeightOffset = 10f;       // Hauteur d'apparition au-dessus du sol

    private bool isStorming = false;
    private int launchedLightningCount = 0;
    private Coroutine stormCoroutine;

    void Start()
    {
        if (wizard == null)
        {
            Debug.LogError("StormController: La référence Wizard n'est pas assignée !");
        }
        if (playerTransform == null)
        {
            Debug.LogError("StormController: La référence Player n'est pas assignée !");
        }
    }

    // Plus besoin de la fonction Update pour ça, la Coroutine gère le temps toute seule.

    public void LaunchLightning()
    {
        if (isStorming) return; // Évite de lancer deux orages en même temps
        
        isStorming = true;
        launchedLightningCount = 0;

        // On lance la séquence de la cascade
        if (stormCoroutine != null) StopCoroutine(stormCoroutine);
        stormCoroutine = StartCoroutine(StormWaveRoutine());
    }
    
    public void StopStorm()
    {
        isStorming = false;
        if (stormCoroutine != null)
        {
            StopCoroutine(stormCoroutine);
        }
    }

    private IEnumerator StormWaveRoutine()
    {
        if (wizard == null || playerTransform == null || lightningPrefabs.Length == 0)
        {
            isStorming = false;
            yield break;
        }

        // 1. Enregistrer les positions au moment précis du lancement de l'attaque
        float startX = wizard.transform.position.x;
        float targetX = playerTransform.position.x;

        // 2. Calculer la distance et la direction (-1 vers la gauche, 1 vers la droite)
        float totalDistance = Mathf.Abs(targetX - startX);
        float direction = Mathf.Sign(targetX - startX); 

        // 3. Calculer combien d'éclairs il faut pour parcourir cette distance
        // Comme la distance (spaceBetweenLightnings) est constante, le nombre d'éclairs s'adapte à la distance du joueur.
        int strikeCount = Mathf.FloorToInt(totalDistance / spaceBetweenLightnings);

        // On s'assure qu'il y a au moins 1 éclair, même si le joueur est très près
        if (strikeCount < 1) strikeCount = 1;

        // 4. Boucle d'apparition des éclairs
        for (int i = 0; i <= strikeCount; i++) // <= pour s'assurer que le dernier frappe bien la zone du joueur
        {
            // Sécurité : si on arrête le combat, on stoppe la boucle
            if (!isStorming) yield break; 

            // Calcul de la position du prochain éclair
            float spawnX = startX + (direction * spaceBetweenLightnings * i);
            
            // La position finale avec la hauteur
            Vector3 spawnPos = new Vector3(spawnX, wizard.transform.position.y + spawnHeightOffset, 0);

            // Choix aléatoire du visuel de l'éclair et apparition
            int randomIndex = Random.Range(0, lightningPrefabs.Length);
            Instantiate(lightningPrefabs[randomIndex], spawnPos, Quaternion.identity);

            launchedLightningCount++;

            // On attend le temps exact défini avant de faire tomber le prochain
            yield return new WaitForSeconds(timeBetweenLightnings);
        }

        // L'attaque est terminée
        isStorming = false;
    }
}
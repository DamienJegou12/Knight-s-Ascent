using UnityEngine;

// Code fait par Damien JEGOU
public class StormController : MonoBehaviour
{
    [Header("Paramètres de l'Orage")]
    public GameObject[] lightningPrefabs; // Ta liste d'éclairs
    
    [Header("Référence Wizard")]
    public WizardManager wizard; // Le wizard qui lance l'orage

    [Header("Fréquence")]
    public float minSpawnRate = 0.5f;
    public float maxSpawnRate = 2.0f;
    

    private bool isStorming = false;
    private int launchedLightningCount = 0;
    private float spawnTimer;

    public void SetStormActive(bool active)
    {
        isStorming = active;
        if (active) spawnTimer = 0f;
    }

    void Start()
    {
        if (wizard == null)
        {
            Debug.LogError("StormController: Wizard reference is not set!");
        }
        
    }

    void Update()
    {
        if (!isStorming || wizard == null || lightningPrefabs.Length == 0) return;

        if (launchedLightningCount >= wizard.GetMaxLightningCount())
        {
            isStorming = false;
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnLightningInCameraView();
            spawnTimer = Random.Range(minSpawnRate, maxSpawnRate);
        }
    }

    void SpawnLightningInCameraView()
    {
        // // 1. CALCULER LA TAILLE DE L'ÉCRAN EN UNITÉS DU MONDE
        // // La hauteur totale = OrthographicSize * 2
        // float screenHeight = cam.orthographicSize * 2f;
        // // La largeur totale = Hauteur * Ratio de l'écran (ex: 16/9)
        // float screenWidth = screenHeight * cam.aspect;

        // // 2. DÉFINIR LA ZONE DE SPAWN (avec une petite marge de sécurité)
        // float spawnHeight = screenHeight - (edgePadding * 2);
        // float spawnWidth = screenWidth - (edgePadding * 2);

        // // 3. CALCULER UNE POSITION ALÉATOIRE
        // // On part du centre de la caméra (cam.transform.position)
        // float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        // float randomY = Random.Range(-spawnHeight / 2f, spawnHeight / 2f);

        // Vector3 spawnPos = cam.transform.position + new Vector3(randomX, randomY, 10f);
        // spawnPos.z = 0; // On remet Z à 0 pour être sur le plan du jeu

        // // 4. CHOISIR ET CRÉER L'ÉCLAIR
        // int randomIndex = Random.Range(0, lightningPrefabs.Length);
        // Instantiate(lightningPrefabs[randomIndex], spawnPos, Quaternion.identity);
    }

    public void LaunchLightning()
    {
        isStorming = true;
        launchedLightningCount = 0;
    }
    
    public void StopStorm()
    {
        isStorming = false;
    }
}
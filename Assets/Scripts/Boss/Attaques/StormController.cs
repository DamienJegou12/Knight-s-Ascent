using UnityEngine;

// Code fait par Damien JEGOU
public class StormController : MonoBehaviour
{
    [Header("Paramètres de l'Orage")]
    public GameObject[] lightningPrefabs; // Ta liste d'éclairs
    
    [Header("Référence Caméra")]
    public Camera cam; // La caméra qui filme le jeu

    [Header("Fréquence")]
    public float minSpawnRate = 0.5f;
    public float maxSpawnRate = 2.0f;
    
    [Header("Marge (Optionnel)")]
    // Réduis cette valeur (ex: 1.0) pour que les éclairs ne spawnent pas
    // collés au bord de l'écran, mais bien à l'intérieur.
    public float edgePadding = 0.5f; 

    private bool isStorming = false;
    private float spawnTimer;

    public void SetStormActive(bool active)
    {
        isStorming = active;
        if (active) spawnTimer = 0f;
    }

    void Start()
    {
        // Si on a oublié de mettre la caméra, on prend la principale par défaut
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (!isStorming || cam == null || lightningPrefabs.Length == 0) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnLightningInCameraView();
            spawnTimer = Random.Range(minSpawnRate, maxSpawnRate);
        }
    }

    void SpawnLightningInCameraView()
    {
        // 1. CALCULER LA TAILLE DE L'ÉCRAN EN UNITÉS DU MONDE
        // La hauteur totale = OrthographicSize * 2
        float screenHeight = cam.orthographicSize * 2f;
        // La largeur totale = Hauteur * Ratio de l'écran (ex: 16/9)
        float screenWidth = screenHeight * cam.aspect;

        // 2. DÉFINIR LA ZONE DE SPAWN (avec une petite marge de sécurité)
        float spawnHeight = screenHeight - (edgePadding * 2);
        float spawnWidth = screenWidth - (edgePadding * 2);

        // 3. CALCULER UNE POSITION ALÉATOIRE
        // On part du centre de la caméra (cam.transform.position)
        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float randomY = Random.Range(-spawnHeight / 2f, spawnHeight / 2f);

        Vector3 spawnPos = cam.transform.position + new Vector3(randomX, randomY, 10f);
        spawnPos.z = 0; // On remet Z à 0 pour être sur le plan du jeu

        // 4. CHOISIR ET CRÉER L'ÉCLAIR
        int randomIndex = Random.Range(0, lightningPrefabs.Length);
        Instantiate(lightningPrefabs[randomIndex], spawnPos, Quaternion.identity);
    }
    
}
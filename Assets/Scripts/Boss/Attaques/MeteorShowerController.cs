using UnityEngine;
using System.Collections;

// Code fait par Damien JEGOU
public class MeteorShowerController : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] meteorPrefabs;
    public Camera cam;
    
    [Header("Paramètres de la pluie")]
    public int minMeteors = 5;
    public int maxMeteors = 10;
    public float spawnInterval = 0.3f; // Plus rapide car ils ont du trajet à faire
    
    [Header("Zone d'arrivée")]
    public float edgePadding = 1.0f; // Marge pour ne pas taper trop au bord

    [Header("Départ (Haut-Droite)")]
    public float spawnDistance = 15f; // Distance hors champ d'où ils partent

    void Start()
    {
        if (cam == null) cam = Camera.main; // Assure que la caméra principale est assignée
    }

    public void TriggerMeteorShower()
    {
        StartCoroutine(SpawnMeteorsRoutine()); // Lance la coroutine pour spawn les météores

    }

    IEnumerator SpawnMeteorsRoutine()
    {
        int count = Random.Range(minMeteors, maxMeteors + 1);
        
        for (int i = 0; i < count; i++)
        {
            SpawnSingleMeteor();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSingleMeteor()
    {
        if (cam == null || meteorPrefabs.Length == 0) return;

        // 1. CALCUL DE LA CIBLE (Sur l'écran)
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        
        float safeW = width - (edgePadding * 2);
        float safeH = height - (edgePadding * 2);

        float targetX = Random.Range(-safeW / 2f, safeW / 2f);
        float targetY = Random.Range(-safeH / 2f, safeH / 2f);
        
        Vector3 targetPos = cam.transform.position + new Vector3(targetX, targetY, 0);
        targetPos.z = 0;

        // 2. CALCUL DU DÉPART (Haut-Droite hors champ)
        // On prend un point décalé vers la droite (+X) et le haut (+Y)
        // Tu peux ajuster (1f, 1f) pour changer l'angle d'arrivée (ex: 0.5f, 1f pour plus vertical)
        Vector3 directionFrom = new Vector3(1f, 1f, 0).normalized; 
        Vector3 startPos = targetPos + (directionFrom * spawnDistance);

        // 3. CRÉATION
        GameObject prefab = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];
        GameObject meteorObj = Instantiate(prefab, startPos, Quaternion.identity);

        // 4. INITIALISATION
        Meteor meteorScript = meteorObj.GetComponent<Meteor>();
        if (meteorScript != null)
        {
            meteorScript.Initialize(startPos, targetPos);
        }
    }
}

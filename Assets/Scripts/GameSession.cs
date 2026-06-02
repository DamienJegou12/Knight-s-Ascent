using UnityEngine;

public class GameSession : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake() {
        int gameSessionCount = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;
        if(gameSessionCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

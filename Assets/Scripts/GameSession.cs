using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSession : MonoBehaviour
{
    [SerializeField]
    private int playerLives = 3;

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

    public void ProcessPlayerDeath()
    {
        if(playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            Invoke("ResetGameSession", 1f);
        }
    }

    void TakeLife()
    {
        playerLives--;
        Invoke("ResetLevel", 1f);
    }

    void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(gameObject);
    }

    void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}

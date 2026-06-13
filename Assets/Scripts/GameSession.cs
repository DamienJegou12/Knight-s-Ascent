using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField]
    private int playerLives = 3;
    [SerializeField]
    private int coins = 0;
    [SerializeField]
    private int currentCoins = 0;
    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI livesText;
    [SerializeField]
    TextMeshProUGUI coinsText;
    [SerializeField]
    TextMeshProUGUI dashText;
    [SerializeField]
    TextMeshProUGUI rollText;

    private bool canDash = true;
    private bool canRoll = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        livesText.text = "Lives : " + playerLives.ToString();
        coinsText.text = "Coins : " + coins.ToString();
        dashText.text = "Dash : " + (canDash ? "Yes" : "No");
        rollText.text = "Roll : " + (canRoll ? "Yes" : "No");
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
        livesText.text = "Lives : " + playerLives.ToString();
        coinsText.text = "Coins : " + coins.ToString();
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
        livesText.text = "Lives : " + playerLives.ToString();
        currentCoins = coins;
        coinsText.text = "Coins : " + currentCoins.ToString();
        Invoke("ResetLevel", 1f);
    }

    void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Destroy(gameObject);
    }

    void ResetGameSession()
    {
        SceneManager.LoadScene("GameOver");
        Destroy(gameObject);
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        coinsText.text = "Coins : " + currentCoins.ToString();
    }

    public void SaveCoins()
    {
        coins = currentCoins;
    }

    public void AddLife()
    {
        playerLives++;
        livesText.text = "Lives : " + playerLives.ToString();
    }

    public void setCanDash(bool value)
    {
        canDash = value;
        dashText.text = "Dash : " + (canDash ? "Yes" : "No");
    }

    public void setCanRoll(bool value)
    {
        canRoll = value;
        rollText.text = "Roll : " + (canRoll ? "Yes" : "No");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuOLD : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}

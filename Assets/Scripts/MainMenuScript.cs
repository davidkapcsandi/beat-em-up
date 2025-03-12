using UnityEngine;
using UnityEngine.SceneManagement; // For loading scenes
using UnityEngine.UI; // For button UI

public class MainMenuScript : MonoBehaviour
{
    public void Start()
    {
        SceneManager.LoadScene("ErrolLevelDesignunity");
    }

    public void OpenOptions()
    {
        // Load options menu (you can set this up similarly as a separate scene or panel)
        Debug.Log("Options clicked");
    }

    public void QuitGame()
    {
        // Exit the game (won't work in the editor)
        Application.Quit();
    }
}

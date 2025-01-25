using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    /***********************MOVE TO UI MANAGER ***********************/
    [Header("UI Elements")]
    [SerializeField] private GameObject endGameUI; // Assign in the Unity Inspector
    [SerializeField] private UnityEngine.UI.Text scoreText; // Assign a Text element for the score
    /***********************MOVE TO UI MANAGER ***********************/


    [SerializeField] private int score = 0; // Example score tracker

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }//end Awake

    /// <summary>
    /// Handles the player death scenario by stopping the game, showing the end game UI, and displaying the score.
    /// </summary>
    public void PlayerDeath()
    {
        Debug.Log("Player has died. Handling end-game...");

        // Stop game time
        Time.timeScale = 0f;

        // Display the end game UI
        if (endGameUI != null)
        {
            endGameUI.SetActive(true);
        }

        // Display the final score
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        // Add additional functionality here, e.g., saving scores or logging analytics



    }//end PlayerDeath


    /// <summary>
    /// Restart the game by reloading the current scene.
    /// </summary>
    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;
       
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //------------------------------------------Access the LoadManager.Instance.LoadLevel(SceneManager.GetActiveScene().name);

        // Reset score (optional)
        score = 0;

        Debug.Log("Game restarted.");
    }//end RestartGame


    /// <summary>
    /// Update the player's score.
    /// </summary>
    /// <param name="points">Points to add to the current score.</param>
    public void AddScore(int points)
    {
        score += points;

        //Add / Display score the the HUD UI -> UIManager.Instance.UpdateScore(score)
    }

}

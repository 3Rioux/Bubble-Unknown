using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public StateMachine<GameState> GameStateMachine;

    public List<int> enemyPerLevel = new List<int>(5);

    private int _currentLevel = 0;
    private int _enemiesKilledThisLevel = 0;


    [SerializeField] private int score = 0; // Example score tracker

    public int CurrentLevel { get => _currentLevel; set => _currentLevel = value; }
    public int EnemiesKilledThisLevel { get => _enemiesKilledThisLevel; set => _enemiesKilledThisLevel = value; }

    /***********************MOVE TO UI MANAGER ***********************/
    //[Header("UI Elements")]
    ////[SerializeField] private GameObject endGameUI; // Assign in the Unity Inspector
    ////[SerializeField] private TextMeshProUGUI scoreText; // Assign a Text element for the score

    //[Header("UI Elements")]
    //[SerializeField] private GameObject mainMenuUI;
    //[SerializeField] private GameObject pauseMenuUI;
    //[SerializeField] private GameObject gameOverUI;

    /***********************MOVE TO UI MANAGER ***********************/

    [SerializeField] private TextMeshProUGUI _displayCurrentState;
    [SerializeField] private TextMeshProUGUI _displayCurrentEnemyKilled;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

      


        // Initialize the state machine with the default state
        GameStateMachine = new StateMachine<GameState>(GameState.MAINMENU);
        GameStateMachine.OnStateChanged += HandleStateChanged;

    }//end Awake

    private void Start()
    {
        // Start in MAINMENU
        GameStateMachine.ChangeState(GameState.MAINMENU);

        //init enemy level count
        enemyPerLevel[0] = 5;
        enemyPerLevel[1] = 7;
        enemyPerLevel[2] = 10;
        enemyPerLevel[3] = 13; //...
    }
    private void Update()
    {
        _displayCurrentState.text = GameStateMachine.CurrentState.ToString();

        //Prevent Pausing in the GameOver State 
        if (GameManager.Instance.GameStateMachine.CurrentState == GameState.GAMEOVER) return;

        // Handle game state transitions (example hotkeys)
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (GameStateMachine.CurrentState == GameState.PLAYING)
        //    {
        //        PauseGame();
        //    }
        //    else if (GameStateMachine.CurrentState == GameState.PAUSED)
        //    {
        //        ResumeGame();
        //    }
        //}

        //Test Back To Main Menu 
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    ReturnToMainMenu();
        //}
    }

    //===================Game States===============================

    private void HandleStateChanged(GameState newState)
    {
        // Deactivate all UIs initially
        //mainMenuUI?.SetActive(false);
        //pauseMenuUI?.SetActive(false);
        //gameOverUI?.SetActive(false);

        // Handle UI and logic for each state
        switch (newState)
        {
            case GameState.MAINMENU:
                // Reset score (optional)
                score = 0;
                CurrentLevel = 0; //reset level to 1 
                EnemiesKilledThisLevel = 0;
                //UIManager.Instance.ShowMainMenu(true);
                //Time.timeScale = 0f; // Pause the game
                break;

            case GameState.PLAYING:
                UIManager.Instance.ShowMainMenu(false);
                EnemiesKilledThisLevel = 0;
                //UIManager.Instance.SetPanelVisibility(UIManager.Instance._pauseMenuPanel, false);
                Time.timeScale = 1f; // Resume the game
                break;

            case GameState.PAUSED:
                //UIManager.Instance.SetPanelVisibility(UIManager.Instance._pauseMenuPanel, true);
                //UIManager.Instance.TogglePauseMenu();

                //Time.timeScale = 0f; // Pause the game
                break;

            case GameState.GAMEOVER:
                UIManager.Instance.ToggleGameOver();
                // Reset score (optional)
                score = 0;
                break;
        }
    }

    public void StartGame()
    {
        GameStateMachine.ChangeState(GameState.PLAYING);
    }

    public void PauseGame()
    {
        GameStateMachine.ChangeState(GameState.PAUSED);
    }

    public void ResumeGame()
    {
        GameStateMachine.ChangeState(GameState.PLAYING);
    }

    public void ReturnToMainMenu()
    {
        GameStateMachine.ChangeState(GameState.MAINMENU);
    }

    public void GameOver()
    {
        GameStateMachine.ChangeState(GameState.GAMEOVER);
    }

    //===================Game States===============================

    /// <summary>
    /// Handles the player death scenario by stopping the game, showing the end game UI, and displaying the score.
    /// </summary>
    public void PlayerDeath()
    {
        Debug.Log("Player has died. Handling end-game...");

        // Stop game time
        Time.timeScale = 0f;

        // Display the end game UI
        //if (gameOverUI != null)
        //{
        //    gameOverUI.SetActive(true);
        //}

        GameOver();
        // Display the final score
        //if (scoreText != null)
        //{
        //    scoreText.text = $"Score: {score}";
        //}

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
        UIManager.Instance.UpdateScore(score);
    }

    public void AddEnemyKilled()
    {
        _enemiesKilledThisLevel++;

        //Add / Display score the the HUD UI -> UIManager.Instance.UpdateScore(score)
        _displayCurrentEnemyKilled.text = _enemiesKilledThisLevel.ToString() + " / " + enemyPerLevel[_currentLevel].ToString();

        //EndGame Victory message:
        if(_enemiesKilledThisLevel == enemyPerLevel[_currentLevel])
        {
            //Win Game Logic


        }
    }//edn AddEnemyKilled


    public void LoadNextLevel()
    {
        CurrentLevel++;

        SceneManager.LoadScene(CurrentLevel + 1);

    }


}

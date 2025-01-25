using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _creditsPanel;
    //Added GameOver
    [SerializeField] private GameObject _gameOverPanel;

    [Header("Audio Components")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerSnapshot _normalSnapshot;
    [SerializeField] private AudioMixerSnapshot _inPauseSnapshot;

    [Header("Audio Settings")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    private InputSystem_Actions _inputActions;
    private bool _isPaused = false;
    private bool _isMainMenu => SceneManager.GetActiveScene().name == "MainMenu";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _inputActions = new InputSystem_Actions();  // Ensure this is initialized only once
    }


    private void OnEnable()
    {
        _inputActions.UI.Pause.Enable();
        _inputActions.UI.Back.Enable();
        _inputActions.Player.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (_inputActions != null)
        {
            _inputActions.UI.Pause.Disable();
            _inputActions.UI.Back.Disable();
            _inputActions.Player.Disable();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Start()
    {
        LoadSettings();
        HandleSceneStart();
    }

private void Update()
{
    // Prevent pausing in the GameOver state
    if (GameManager.Instance.GameStateMachine.CurrentState == GameState.GAMEOVER) return;

    // Prevent pausing in the main menu
    if (!_isMainMenu && _inputActions.UI.Pause.triggered)
    {
        TogglePauseMenu();
    }

    // Close settings or credits using Escape (Back button)
    if (_settingsPanel.activeSelf && _inputActions.UI.Back.triggered)
    {
        ToggleSettings();
    }
    else if (_creditsPanel.activeSelf && _inputActions.UI.Back.triggered)
    {
        ToggleCredits();
    }
}


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneStart();
    }

    private void HandleSceneStart()
    {
        if (_isMainMenu)
        {
            ShowMainMenu(true);
            SetPanelVisibility(_pauseMenuPanel, false);
            SetPanelVisibility(_settingsPanel, false);
            SetPanelVisibility(_creditsPanel, false);
            SetPanelVisibility(_gameOverPanel, false);//Added

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ShowMainMenu(false);
            SetPanelVisibility(_pauseMenuPanel, false);
            SetPanelVisibility(_settingsPanel, false);
            SetPanelVisibility(_gameOverPanel, false);//Added
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    // === GAME OVER HANDLING ===
    public void ToggleGameOver()
    {
        bool toggle = GameManager.Instance.GameStateMachine.CurrentState == GameState.GAMEOVER;
        SetPanelVisibility(_gameOverPanel, toggle);

        if (toggle)
        {
            //reset the pause: 
            _isPaused = true;
            _inputActions.Player.Disable();
            _inPauseSnapshot.TransitionTo(0.5f); // ****************Change to Game Over
        }
        else
        {
            _inputActions.Player.Enable();
            _normalSnapshot.TransitionTo(0.5f);
        }

        Time.timeScale = 0f; // Stop the game
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = toggle;
    }

    // === PAUSE SYSTEM ===
    private void TogglePauseMenu()
    {
        _isPaused = !_isPaused;
        SetPanelVisibility(_pauseMenuPanel, _isPaused);

        //---
        if (GameManager.Instance.GameStateMachine.CurrentState == GameState.PLAYING)
        {
            GameManager.Instance.PauseGame();
        }
        else if (GameManager.Instance.GameStateMachine.CurrentState == GameState.PAUSED)
        {
            GameManager.Instance.ResumeGame();
        }
        //---

        if (_isPaused)
        {
            _inputActions.Player.Disable();
            _inPauseSnapshot.TransitionTo(0.5f);
        }
        else
        {
            _inputActions.Player.Enable();
            _normalSnapshot.TransitionTo(0.5f);
        }

        Time.timeScale = _isPaused ? 0 : 1;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;
    }

    public void OnClick_Resume()
    {
        TogglePauseMenu();
    }

    public void OnClick_BackToMainMenu()
    {
        TogglePauseMenu();
        SceneManager.LoadSceneAsync("MainMenu");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Set State to Main Menu
        GameManager.Instance.ReturnToMainMenu();
    }

    // === SETTINGS HANDLING ===
    public void ToggleSettings()
    {
        bool isActive = _settingsPanel.activeSelf;
        SetPanelVisibility(_settingsPanel, !isActive);
        
        // If settings are closed, make sure to resume pause menu properly
        if (!isActive && _pauseMenuPanel != null && _pauseMenuPanel.activeSelf)
        {
            SetPanelVisibility(_pauseMenuPanel, true);
        }
    }

    public void OnClick_Settings()
    {
        ToggleSettings();
    }

    // === MENU HANDLING ===
    public void ShowMainMenu(bool show)
    {
        SetPanelVisibility(_mainMenuPanel, show);
    }

    public void ToggleCredits()
    {
        bool isActive = _creditsPanel.activeSelf;
        SetPanelVisibility(_creditsPanel, !isActive);

        // If credits are closed, make sure to resume pause menu properly
        if (!isActive && _pauseMenuPanel != null && _pauseMenuPanel.activeSelf)
        {
            SetPanelVisibility(_pauseMenuPanel, true);
        }
    }


    public void OnClick_StartGame()
    {
        // change state to Playing:
        GameManager.Instance.StartGame();

        //Load Level
        SceneManager.LoadScene("Level_1");
    }

    // === AUDIO SETTINGS ===
    public void OnClick_SetMasterVolume(float volume)
    {
        SoundManager.Instance.SetMasterVolume(volume);
        _masterVolumeSlider.value = volume;  // Update slider UI
    }

    public void OnClick_SetMusicVolume(float volume)
    {
        SoundManager.Instance.SetMusicVolume(volume);
        _musicVolumeSlider.value = volume;  // Update slider UI
    }

    public void OnClick_SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetSFXVolume(volume);
        _sfxVolumeSlider.value = volume;  // Update slider UI
    }

    private void LoadSettings()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }


    // === HELPER FUNCTION ===
    public void SetPanelVisibility(GameObject panel, bool visible)
    {
        if (panel == null)
        {
            Debug.LogError("Panel is not assigned in UIManager!");
            return;
        }

        if (!panel.activeSelf && visible)
        {
            panel.SetActive(true);
        }

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            panel.SetActive(visible);
            return;
        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        if (!visible)
        {
            panel.SetActive(false);
        }
    }
}

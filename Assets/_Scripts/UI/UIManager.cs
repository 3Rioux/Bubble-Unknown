using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;  // DOTween namespace

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _gameHUD;
    [SerializeField] private TextMeshProUGUI _gameHUDScoreText;

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

        _inputActions = new InputSystem_Actions();
    }


    private void OnEnable()
    {
        _inputActions.UI.Pause.Enable();
        _inputActions.UI.Back.Enable();
        _inputActions.Player.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;

        _inputActions.UI.Pause.performed += ctx => TogglePauseMenu();
        _inputActions.UI.Back.performed += ctx =>
        {
            if (_settingsPanel.activeSelf) ToggleSettings();
            else if (_creditsPanel.activeSelf) ToggleCredits();
            else if (_gameOverPanel.activeSelf) ToggleGameOver();
        };
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
        UpdateVolumeSliders();
        HandleSceneStart();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneStart();
    }

    private void HandleSceneStart()
    {
        if (_isMainMenu)
        {
            ToggleGameHUDOver();
            ShowMainMenu(true);
            SetPanelVisibility(_pauseMenuPanel, false);
            SetPanelVisibility(_settingsPanel, false);
            SetPanelVisibility(_creditsPanel, false);
            SetPanelVisibility(_gameOverPanel, false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ToggleGameHUDOver();
            ShowMainMenu(false);
            SetPanelVisibility(_pauseMenuPanel, false);
            SetPanelVisibility(_settingsPanel, false);
            SetPanelVisibility(_gameOverPanel, false);
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    // === GAME HUD HANDLING ===
    public void ToggleGameHUDOver()
    {
        bool isGame = GameManager.Instance.GameStateMachine.CurrentState == GameState.PLAYING;
        SetPanelVisibility(_gameHUD, isGame);
    }

    public void UpdateScore(int score)
    {
        if (score > 0)
        {
            _gameHUDScoreText.text = $"Score: {score}pts";
        }
    }


    // === GAME OVER HANDLING ===
    public void ToggleGameOver()
    {
        bool isGameOver = GameManager.Instance.GameStateMachine.CurrentState == GameState.GAMEOVER;
        SetPanelVisibility(_gameOverPanel, isGameOver);

        if (isGameOver)
        {
            _isPaused = true;
            _inputActions.Player.Disable();
            _inPauseSnapshot.TransitionTo(0.5f);  // Switch to game over audio
            Time.timeScale = 0f;
        }
        else
        {
            _inputActions.Player.Enable();
            _normalSnapshot.TransitionTo(0.5f);
            Time.timeScale = 1f;
        }

        Cursor.visible = isGameOver;
    }

    // === PAUSE SYSTEM ===
    private void TogglePauseMenu()
    {
        _isPaused = !_isPaused;
        SetPanelVisibility(_pauseMenuPanel, _isPaused);

        if (_isPaused)
        {
            _inputActions.Player.Disable();
            _inPauseSnapshot.TransitionTo(0.5f);
            Time.timeScale = 0f;
        }
        else
        {
            _inputActions.Player.Enable();
            _normalSnapshot.TransitionTo(0.5f);
            Time.timeScale = 1f;
        }

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

        if (_gameOverPanel.activeSelf)
        {
            SetPanelVisibility(_gameOverPanel, false);
        }

        GameManager.Instance.ReturnToMainMenu();
    }

    // === SETTINGS HANDLING ===
    public void ToggleSettings()
    {
        SetPanelVisibility(_settingsPanel, !_settingsPanel.activeSelf);
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
        SetPanelVisibility(_creditsPanel, !_creditsPanel.activeSelf);
    }

    public void OnClick_StartGame()
    {
        GameManager.Instance.StartGame();
        SceneManager.LoadScene("Level_1");
    }

    // === AUDIO SETTINGS ===
    public void OnClick_SetMasterVolume(float volume)
    {
        SoundManager.Instance.SetMasterVolume(volume);
        _masterVolumeSlider.value = volume;
    }

    public void OnClick_SetMusicVolume(float volume)
    {
        SoundManager.Instance.SetMusicVolume(volume);
        _musicVolumeSlider.value = volume;
    }

    public void OnClick_SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetSFXVolume(volume);
        _sfxVolumeSlider.value = volume;
    }

    private void UpdateVolumeSliders()
    {
        _masterVolumeSlider.value = SoundManager.Instance.GetMasterVolume();
        _musicVolumeSlider.value = SoundManager.Instance.GetMusicVolume();
        _sfxVolumeSlider.value = SoundManager.Instance.GetSFXVolume();
    }

    // === PANEL VISIBILITY WITH DOTWEEN ===
    public void SetPanelVisibility(GameObject panel, bool visible)
    {
        if (panel == null)
        {
            Debug.LogError("Panel is not assigned in UIManager!");
            return;
        }

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            panel.SetActive(visible);
            return;
        }

        panel.SetActive(true);
        panel.transform.DOKill();
        canvasGroup.DOKill();

        if (visible)
        {
            canvasGroup.alpha = 0f;  // Ensure it starts hidden
            canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad).SetUpdate(true);
            panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.DOFade(0f, 0.2f)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)
                .OnComplete(() => panel.SetActive(false));

            panel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).SetUpdate(true);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }


}

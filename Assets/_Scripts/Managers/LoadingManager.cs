using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{

    //Make it a singleton:
    public static LoadingManager Instance;
    
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen; // The loading screen canvas
    [SerializeField] private Slider progressBar; // Progress bar UI element
    [SerializeField] private TextMeshProUGUI progressText; // Text to show progress percentage


    private void Awake()
    {
        // Ensure only one instance of the LoadingManager exists
        if (Instance == null)
        {
            Instance = this;
            
            DontDestroyOnLoad(gameObject); // Persist across scenes
            //not parent rn so dont do much^^ but just as a safety ill always(if neded) include it 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads the specified scene asynchronously with a loading screen.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadingScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncByName(sceneName));
    }

    private IEnumerator LoadSceneAsyncByName(string sceneName)
    {
        // Activate the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent automatic scene activation

        // Update the loading screen progress
        while (!operation.isDone)
        {
            // Calculate the progress (Unity's progress value goes from 0.0 to 0.9 before activation)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Update progress bar and text
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{(progress * 100):0}%";
            }

            // Activate the scene when loading is complete
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Hide the loading screen after loading
        if (loadingScreen != null)
        {
            Debug.Log("Close Load Screen");
            loadingScreen.SetActive(false);
        }
        Debug.Log("Close Load Screen");
    }//end LoadSceneAsync
}

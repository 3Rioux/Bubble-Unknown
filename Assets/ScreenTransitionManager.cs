using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenTransitionManager : MonoBehaviour
{
    public static ScreenTransitionManager Instance { get; private set; }

    [SerializeField] private Image _transitionImage;
    [SerializeField] private float _transitionDuration = 1.2f;

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

        if (_transitionImage == null)
        {
            Debug.LogError("Transition image is not assigned in the inspector!", this);
            return;
        }

        _transitionImage.gameObject.SetActive(false);
    }


    // Call this method to transition to a new scene
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        _transitionImage.gameObject.SetActive(true);
        _transitionImage.rectTransform.localScale = Vector3.zero;

        // Expand circle effect
        _transitionImage.rectTransform.DOScale(Vector3.one * 50f, _transitionDuration)
            .SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(_transitionDuration);

        SceneManager.LoadScene(sceneName);

        // Shrink circle effect
        _transitionImage.rectTransform.DOScale(Vector3.zero, _transitionDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => _transitionImage.gameObject.SetActive(false));
    }
}
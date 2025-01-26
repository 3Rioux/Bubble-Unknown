using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

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

        LoadVolumeSettings();
    }

    // === MUSIC CONTROL ===
    public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
    {
        if (_musicSource.clip == clip) return;  // Avoid replaying the same track

        StartCoroutine(FadeInMusic(clip, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutMusic(fadeDuration));
    }

    // === SFX CONTROL ===
    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    // === VOLUME CONTROL ===
    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // Retrieve current volume settings for UI
    public float GetMasterVolume() => PlayerPrefs.GetFloat("MasterVolume", 0.75f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat("MusicVolume", 0.75f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat("SFXVolume", 0.75f);

    private void LoadVolumeSettings()
    {
        SetMasterVolume(GetMasterVolume());
        SetMusicVolume(GetMusicVolume());
        SetSFXVolume(GetSFXVolume());
    }

    // === AUDIO FADE HELPERS ===
    private IEnumerator FadeInMusic(AudioClip newClip, float duration)
    {
        _musicSource.clip = newClip;
        _musicSource.Play();
        _musicSource.volume = 0f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = _musicSource.volume;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        _musicSource.Stop();
    }
}

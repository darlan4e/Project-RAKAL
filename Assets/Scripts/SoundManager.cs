using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Music")]
    public AudioClip defaultMusic;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Enemy")]
    public AudioClip enemyHurt;
    
    [Header("Player")]
    public AudioClip playerHurt;
    public AudioClip bowShot;

    private List<AudioSource> _audioSources = new List<AudioSource>();
    private AudioSource _musicSource;
    private int _maxSources = 5;

    private bool isMenuScene = false; // Помечаем, если сцена — меню

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupAudioSources()
    {
        for (int i = 0; i < _maxSources; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            _audioSources.Add(newSource);
        }

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Проверяем, является ли сцена меню
        if (IsMenuScene(scene.name))
        {
            isMenuScene = true;
            StopMusic();
        }
        else
        {
            isMenuScene = false;
            // Если это не меню, запускаем музыку
            if (defaultMusic != null)
            {
                PlayMusic(defaultMusic);
            }
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        // При выгрузке сцены (например, при смене уровня) — останавливаем музыку
        if (!isMenuScene)
        {
            StopMusic();
        }
    }

    private bool IsMenuScene(string sceneName)
    {
        // Укажи сюда имена сцен, которые считаются меню
        return sceneName == "MainMenu" || sceneName == "Menu";
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        foreach (AudioSource source in _audioSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.volume = volume;
                source.pitch = pitch;
                source.Play();
                return;
            }
        }

        AudioSource oldestSource = _audioSources[0];
        oldestSource.Stop();
        oldestSource.clip = clip;
        oldestSource.volume = volume;
        oldestSource.pitch = pitch;
        oldestSource.Play();
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == null) return;

        _musicSource.clip = music;
        _musicSource.volume = musicVolume;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        _musicSource.volume = musicVolume;
    }

    public void PlayEnemyHurt()
    {
        if (enemyHurt != null)
        {
            PlaySound(enemyHurt, 0.3f, Random.Range(0.9f, 1.1f));
        }
    }

    public void PlayPlayerHurt()
    {
        if (playerHurt != null)
        {
            PlaySound(playerHurt, 1f, Random.Range(0.9f, 1.1f));
        }
    }
    
    public void PlayBowShot()
    {
        if (bowShot != null)
        {
            PlaySound(bowShot, 0.01f, Random.Range(0.7f, 1.1f));
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
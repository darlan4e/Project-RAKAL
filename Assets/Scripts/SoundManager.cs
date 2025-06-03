using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Делаем менеджер статическим, чтобы он был доступен отовсюду
    public static SoundManager Instance;

    [Header("Enemy")]
    public AudioClip enemyHurt;
    
    [Header("Player")]
    public AudioClip playerHurt;
    public AudioClip bowShot;

    private List<AudioSource> _audioSources = new List<AudioSource>();
    private int _maxSources = 5;
    private void Awake()
    {
        // Делаем это единственным экземпляром
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Чтобы не уничтожался при загрузке новой сцены
        }
        else
        {
            Destroy(gameObject); // Удаляем дубликаты
        }
        // Создаем пул AudioSource
        for (int i = 0; i < _maxSources; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            _audioSources.Add(newSource);
        }
    }
    
    // Проигрывает звук через свободный AudioSource
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

        // Если все источники заняты - выбираем первый и перезаписываем
        AudioSource oldestSource = _audioSources[0];
        oldestSource.Stop();
        oldestSource.clip = clip;
        oldestSource.volume = volume;
        oldestSource.pitch = pitch;
        oldestSource.Play();
    }


    // Метод для проигрывания звука урона врага
    public void PlayEnemyHurt()
    {
        if (enemyHurt != null)
        {
            PlaySound(enemyHurt, 0.3f, Random.Range(0.9f, 1.1f));
        }
    }

    // Другие методы для звуков...
    public void PlayPlayerHurt()
    {
        PlaySound(enemyHurt, 1f, Random.Range(0.9f, 1.1f));
    }
    
    public void PlayBowShot()
    {
        if (bowShot != null)
        {
            PlaySound(bowShot, 0.01f, Random.Range(0.7f, 1.1f));
        }
    }
}
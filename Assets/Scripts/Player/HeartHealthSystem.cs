using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHealthSystem : MonoBehaviour
{
    [Header("Heart Settings")]
    public int maxHearts = 3; // Максимальное количество сердечек
    public GameObject heartPrefab; // Префаб сердечка
    public Transform heartContainer; // Контейнер для сердечек
    
    [Header("Heart Sprites")]
    public Sprite fullHeart; // Полное сердце
    public Sprite halfHeart; // Половина сердца
    public Sprite emptyHeart; // Пустое сердце
    
    private Player _player;
    private List<Image> _heartImages = new List<Image>();
    
    void Awake()
    {
        _player = FindObjectOfType<Player>();
        CreateHearts();
    }
    
    void Update()
    {
        UpdateHearts();
    }
    
    private void CreateHearts()
    {
        // Очищаем существующие сердечки
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }
        _heartImages.Clear();
        
        // Создаем новые сердечки
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            Image heartImage = heart.GetComponent<Image>();
            _heartImages.Add(heartImage);
        }
    }
    
    private void UpdateHearts()
    {
        if (_player == null) return;
        
        float currentHealth = _player.health;
        float maxHealth = _player.MaxHealth;
        
        // Вычисляем количество полных сердечек
        float heartsFloat = (currentHealth / maxHealth) * maxHearts;
        int fullHearts = Mathf.FloorToInt(heartsFloat);
        bool hasHalfHeart = (heartsFloat % 1f) >= 0.5f;
        
        for (int i = 0; i < maxHearts; i++)
        {
            if (i < fullHearts)
            {
                // Полное сердце
                _heartImages[i].sprite = fullHeart;
            }
            else if (i == fullHearts && hasHalfHeart)
            {
                // Половина сердца
                _heartImages[i].sprite = halfHeart;
            }
            else
            {
                // Пустое сердце
                _heartImages[i].sprite = emptyHeart;
            }
        }
    }
    
    // Метод для обновления количества максимальных сердечек (если нужно)
    public void SetMaxHearts(int newMaxHearts)
    {
        maxHearts = newMaxHearts;
        CreateHearts();
    }
}

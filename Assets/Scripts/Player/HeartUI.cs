using UnityEngine;
using UnityEngine.UI;

// Простой компонент для префаба сердечка
public class HeartUI : MonoBehaviour
{
    [Header("Heart Components")]
    public Image heartImage;
    
    void Awake()
    {
        // Если Image компонент не назначен, пытаемся найти его
        if (heartImage == null)
        {
            heartImage = GetComponent<Image>();
        }
    }
    
    // Метод для установки спрайта сердечка
    public void SetHeartSprite(Sprite sprite)
    {
        if (heartImage != null)
        {
            heartImage.sprite = sprite;
        }
    }
}

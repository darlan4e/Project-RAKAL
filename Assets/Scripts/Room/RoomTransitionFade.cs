using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomTransitionFade : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private Image fadeImage;
    
    public static RoomTransitionFade Instance { get; private set; }
    
    // События для уведомления о начале и конце затемнения
    public System.Action OnFadeStart;
    public System.Action OnFadeComplete;
    
    private bool isTransitioning = false;
    
    void Awake()
    {
        Instance = this;
        
        // Если fadeImage не назначен в инспекторе, создаем его программно
        if (fadeImage == null)
        {
            CreateFadeImage();
        }
        
        // Убеждаемся, что изначально экран не затемнен
        SetFadeAlpha(0f);
    }
    
    private void CreateFadeImage()
    {
        // Создаем Canvas для затемнения
        GameObject canvasGO = new GameObject("FadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Помещаем поверх всего остального
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Создаем изображение для затемнения
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false; // Не блокируем клики
        
        // Растягиваем на весь экран
        RectTransform rt = fadeImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }
    
    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
    
    /// <summary>
    /// Выполняет переход с эффектом затемнения
    /// </summary>
    /// <param name="onMidTransition">Действие, выполняемое в момент полного затемнения</param>
    public void FadeTransition(System.Action onMidTransition)
    {
        if (isTransitioning) return;
        
        StartCoroutine(FadeRoutine(onMidTransition));
    }
    
    private IEnumerator FadeRoutine(System.Action onMidTransition)
    {
        isTransitioning = true;
        OnFadeStart?.Invoke(); // Уведомляем о начале затемнения
        
        // Затемнение
        float currentAlpha = 0f;
        while (currentAlpha < 1f)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, fadeSpeed * Time.unscaledDeltaTime);
            SetFadeAlpha(currentAlpha);
            yield return null;
        }
        
        // Выполняем переход на пике затемнения
        onMidTransition?.Invoke();
        
        // Небольшая пауза на пике затемнения (опционально)
        yield return new WaitForSecondsRealtime(0.1f);
        
        // Осветление
        while (currentAlpha > 0f)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, fadeSpeed * Time.unscaledDeltaTime);
            SetFadeAlpha(currentAlpha);
            yield return null;
        }
        
        isTransitioning = false;
        OnFadeComplete?.Invoke(); // Уведомляем о завершении
    }
    
    /// <summary>
    /// Проверяет, происходит ли сейчас переход
    /// </summary>
    public bool IsTransitioning => isTransitioning;
}
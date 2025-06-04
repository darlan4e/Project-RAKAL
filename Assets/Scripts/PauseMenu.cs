using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _blurPanel;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    private bool _isPaused = false;

    private void Start()
    {
        // Назначаем действия кнопкам
        _resumeButton.onClick.AddListener(ResumeGame);
        _restartButton.onClick.AddListener(RestartGame);
        _quitButton.onClick.AddListener(QuitGame);

        // Скрываем меню при старте
        _blurPanel.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    private void Update()
    {
        // Открываем/закрываем меню по ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f; // Останавливаем игру
        _blurPanel.SetActive(true);
        _pauseMenu.SetActive(true);
    }

    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f; // Возобновляем игру
        _blurPanel.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        SceneManager.LoadScene(0);
        
        // Для выхода из игры (в билде):
        //Application.Quit();
    }

    // Опционально: анимация плавного появления
    private IEnumerator FadeInMenu()
    {
        CanvasGroup canvasGroup = _pauseMenu.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = _pauseMenu.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        float fadeTime = 0.3f; // Длительность анимации
        float elapsed = 0;

        while (elapsed < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
            elapsed += Time.unscaledDeltaTime; // Используем unscaled, так как время на паузе
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
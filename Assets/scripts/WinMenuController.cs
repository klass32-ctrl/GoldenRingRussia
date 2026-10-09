using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenuController : MonoBehaviour
{
    [Header("Настройки сцен")]
    [Tooltip("Название вашей основной сцены с игрой")]
    public string gameplaySceneName = "SampleScene";

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            // Нажмите Enter (или Return на клавиатуре) для перезапуска
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                RestartGame();
            }

            // Нажмите Escape (Esc) для выхода из игры
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGame();
            }
        }
    }


    public void RestartGame()
    {
        Debug.Log("Перезапуск игры через скрипт...");

        // На всякий случай: если имя сцены пустое, перезагружаем ТЕКУЩУЮ активную сцену
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    public void ExitGame()
    {
        Debug.Log("Выход из игры...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}



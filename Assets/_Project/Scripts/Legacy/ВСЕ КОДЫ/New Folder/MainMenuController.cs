using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для управления сценами

public class MainMenuController : MonoBehaviour
{
    // Метод для запуска игры
    public void PlayGame()
    {
        // Загружает следующую сцену в очереди (индекс текущей + 1)
        // Или можете указать имя сцены строкой: SceneManager.LoadScene("GameScene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Метод для выхода из игры
    public void QuitGame()
    {
        Debug.Log("Выход из игры..."); // Появится в консоли Unity, чтобы проверить работу в редакторе
        Application.Quit(); // Работает в скомпилированной игре (.exe / .apk)
    }
}
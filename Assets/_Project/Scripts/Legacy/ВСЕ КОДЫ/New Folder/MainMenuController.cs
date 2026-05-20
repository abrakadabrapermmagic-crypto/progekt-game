using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Обязательно для работы со Slider
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Элементы экрана загрузки")]
    [SerializeField] private GameObject loadingScreen; // Объект панели загрузки
    [SerializeField] private Slider progressBar;       // Ссылка на ползунок

    // Этот метод теперь запускает корутину загрузки
    public void PlayGame()
    {
        // Запускаем фоновый процесс загрузки сцены под индексом 1
        StartCoroutine(LoadSceneAsync(1));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        // 1. Включаем экран загрузки поверх меню
        loadingScreen.SetActive(true);

        // 2. Начинаем асинхронную загрузку сцены
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // Пока сцена не загрузилась до конца, крутим этот цикл
        while (!operation.isDone)
        {
            // progress выдает значения от 0 до 0.9. 
            // Разделив на 0.9f, мы приводим его к красивому виду от 0 до 1.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Отдаем значение нашему ползунку
            progressBar.value = progress;

            // Ждем до следующего кадра, чтобы игра не зависала
            yield return null;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}
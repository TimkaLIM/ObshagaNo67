using UnityEngine;
using UnityEngine.SceneManagement; // ОБЯЗАТЕЛЬНО для смены сцен

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // В главном меню нам ВСЕГДА нужны курсор и мышка, чтобы кликать по кнопкам
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Метод для кнопки НАЧАТЬ ИГРУ
    public void PlayGame()
    {
        Debug.Log("Загружаем катсцену...");
        SceneManager.LoadScene("IntroCutscene");
    }

    // Метод для кнопки ВЫХОД
    public void ExitGame()
    {
        Debug.Log("Выходим из игры...");
        Application.Quit(); // Полностью закрывает запущенную .exe игру
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour
{
    [SerializeField] private Button _changeLocalizationBtn;
    [SerializeField] private TMP_Text _localizationText;

    public static int Language {
        get => PlayerPrefs.GetInt(nameof(Language), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(Language), value);
            MainMenu.Instance.UpdateUI();
        }
    }

    private void Awake()
    {
        _changeLocalizationBtn.onClick.AddListener(ChangeLocalization);

        if (Language == 0)
            _localizationText.text = "RU";
        else
            _localizationText.text = "EN";
    }

    private void ChangeLocalization()
    {
        if (Language == 0)
        {
            Language = 1;
            _localizationText.text = "EN";
        }
        else
        {
            Language = 0;
            _localizationText.text = "RU";
        }
    }

    public static string Round
    {
        get
        {
            if (Language == 0)
                return "Раунд";
            else
                return "Round";
        }
    }

    public static string Start
    {
        get
        {
            if (Language == 0)
                return "Старт";
            else
                return "Start";
        }
    }

    public static string Level
    {
        get
        {
            if (Language == 0)
                return "Уровень";
            else
                return "Level";
        }
    }

    public static string Multiplayer
    {
        get
        {
            if (Language == 0)
                return "Мультиплеер";
            else
                return "Multiplayer";
        }
    }

    public static string Records
    {
        get
        {
            if (Language == 0)
                return "Рекорды";
            else
                return "Records";
        }
    }

    public static string Rules
    {
        get
        {
            if (Language == 0)
                return "Правила";
            else
                return "Rules";
        }
    }

    public static string Settings
    {
        get
        {
            if (Language == 0)
                return "Настройки";
            else
                return "Settings";
        }
    }

    public static string Points
    {
        get
        {
            if (Language == 0)
                return "Очки";
            else
                return "Points";
        }
    }

    public static string Maximum
    {
        get
        {
            if (Language == 0)
                return "Максимальный";
            else
                return "Maximum";
        }
    }

    public static string Easy
    {
        get
        {
            if (Language == 0)
                return "Легко";
            else
                return "Easy";
        }
    }

    public static string Normal
    {
        get
        {
            if (Language == 0)
                return "Нормально";
            else
                return "Normal";
        }
    }

    public static string Hard
    {
        get
        {
            if (Language == 0)
                return "Сложно";
            else
                return "Hard";
        }
    }

    public static string Hardcore
    {
        get
        {
            if (Language == 0)
                return "Невозможно";
            else
                return "Hardcore";
        }
    }

    public static string WinsToOpen
    {
        get
        {
            if (Language == 0)
                return "Осталось побед на уровне";
            else
                return "There are still victories on the level";
        }
    }

    public static string LogIn
    {
        get
        {
            if (Language == 0)
                return "Войти";
            else
                return "Log In";
        }
    }

    public static string SignIn
    {
        get
        {
            if (Language == 0)
                return "Зарегистрироваться";
            else
                return "Sign In";
        }
    }

    public static string Guest
    {
        get
        {
            if (Language == 0)
                return "Гость";
            else
                return "Guest";
        }
    }

    public static string LoginMustConsistLatinAndNumb
    {
        get
        {
            if (Language == 0)
                return "Логин должен состоять из латинских букв и цифр!";
            else
                return "The login must consist of Latin letters and numbers!";
        }
    }

    public static string PasswordMustConsistLatinAndNumb
    {
        get
        {
            if (Language == 0)
                return "Пароль должен состоять из латинских букв и цифр!";
            else
                return "The password must consist of Latin letters and numbers!";
        }
    }

    public static string PasswordDoNotMatch
    {
        get
        {
            if (Language == 0)
                return "Пароли не совпадают!";
            else
                return "The passwords don't match!";
        }
    }

    public static string SuchPlayerAlreadyExists 
    {
        get
        {
            if (Language == 0)
                return "Такой игрок уже существует!";
            else
                return "Such a player already exists!";
        }
    }

    public static string PasswordError
    {
        get
        {
            if (Language == 0)
                return "Ошибка пароля!";
            else
                return "Password error!";
        }
    }

    public static string ThereIsNoSuchPlayer
    {
        get
        {
            if (Language == 0)
                return "Такого игрока нет!";
            else
                return "There is no such player!";
        }
    }

    public static string EnterPassword
    {
        get
        {
            if (Language == 0)
                return "Введите пароль";
            else
                return "Enter the password";
        }
    }

    public static string EnterLogin
    {
        get
        {
            if (Language == 0)
                return "Введите логин";
            else
                return "Enter the login";
        }
    }

    public static string ConnectionLost
    {
        get
        {
            if (Language == 0)
                return "Потеряно соединение с сервером!";
            else
                return "The connection to the server is lost!";
        }
    }

    public static string ConfirmPassword
    {
        get
        {
            if (Language == 0)
                return "Подтвердите пароль";
            else
                return "Confirm the password";
        }
    }

    public static string KeepMeSign
    {
        get
        {
            if (Language == 0)
                return "Запомнить меня";
            else
                return "Keep me sign";
        }
    }

    public static string AreYouSureToGetOut
    {
        get
        {
            if (Language == 0)
                return "Вы точно хотите выйти?";
            else
                return "Are you sure you want to get out?";
        }
    }

    public static string No
    {
        get
        {
            if (Language == 0)
                return "НЕТ";
            else
                return "NO";
        }
    }

    public static string Yes
    {
        get
        {
            if (Language == 0)
                return "ДА";
            else
                return "YES";
        }
    }

    public static string ProgressWillBeReset
    {
        get
        {
            if (Language == 0)
                return "Прогресс будет сброшен, вы уверены?";
            else
                return "The progress will be reset, are you sure?";
        }
    }
}

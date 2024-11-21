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
                return "�����";
            else
                return "Round";
        }
    }

    public static string Start
    {
        get
        {
            if (Language == 0)
                return "�����";
            else
                return "Start";
        }
    }

    public static string Level
    {
        get
        {
            if (Language == 0)
                return "�������";
            else
                return "Level";
        }
    }

    public static string Multiplayer
    {
        get
        {
            if (Language == 0)
                return "�����������";
            else
                return "Multiplayer";
        }
    }

    public static string Records
    {
        get
        {
            if (Language == 0)
                return "�������";
            else
                return "Records";
        }
    }

    public static string Rules
    {
        get
        {
            if (Language == 0)
                return "�������";
            else
                return "Rules";
        }
    }

    public static string Settings
    {
        get
        {
            if (Language == 0)
                return "���������";
            else
                return "Settings";
        }
    }

    public static string Points
    {
        get
        {
            if (Language == 0)
                return "����";
            else
                return "Points";
        }
    }

    public static string Maximum
    {
        get
        {
            if (Language == 0)
                return "������������";
            else
                return "Maximum";
        }
    }

    public static string Easy
    {
        get
        {
            if (Language == 0)
                return "�����";
            else
                return "Easy";
        }
    }

    public static string Normal
    {
        get
        {
            if (Language == 0)
                return "���������";
            else
                return "Normal";
        }
    }

    public static string Hard
    {
        get
        {
            if (Language == 0)
                return "������";
            else
                return "Hard";
        }
    }

    public static string Hardcore
    {
        get
        {
            if (Language == 0)
                return "����������";
            else
                return "Hardcore";
        }
    }

    public static string WinsToOpen
    {
        get
        {
            if (Language == 0)
                return "�������� ����� �� ������";
            else
                return "There are still victories on the level";
        }
    }

    public static string LogIn
    {
        get
        {
            if (Language == 0)
                return "�����";
            else
                return "Log In";
        }
    }

    public static string SignIn
    {
        get
        {
            if (Language == 0)
                return "������������������";
            else
                return "Sign In";
        }
    }

    public static string Guest
    {
        get
        {
            if (Language == 0)
                return "�����";
            else
                return "Guest";
        }
    }

    public static string LoginMustConsistLatinAndNumb
    {
        get
        {
            if (Language == 0)
                return "����� ������ �������� �� ��������� ���� � ����!";
            else
                return "The login must consist of Latin letters and numbers!";
        }
    }

    public static string PasswordMustConsistLatinAndNumb
    {
        get
        {
            if (Language == 0)
                return "������ ������ �������� �� ��������� ���� � ����!";
            else
                return "The password must consist of Latin letters and numbers!";
        }
    }

    public static string PasswordDoNotMatch
    {
        get
        {
            if (Language == 0)
                return "������ �� ���������!";
            else
                return "The passwords don't match!";
        }
    }

    public static string SuchPlayerAlreadyExists 
    {
        get
        {
            if (Language == 0)
                return "����� ����� ��� ����������!";
            else
                return "Such a player already exists!";
        }
    }

    public static string PasswordError
    {
        get
        {
            if (Language == 0)
                return "������ ������!";
            else
                return "Password error!";
        }
    }

    public static string ThereIsNoSuchPlayer
    {
        get
        {
            if (Language == 0)
                return "������ ������ ���!";
            else
                return "There is no such player!";
        }
    }

    public static string EnterPassword
    {
        get
        {
            if (Language == 0)
                return "������� ������";
            else
                return "Enter the password";
        }
    }

    public static string EnterLogin
    {
        get
        {
            if (Language == 0)
                return "������� �����";
            else
                return "Enter the login";
        }
    }

    public static string ConnectionLost
    {
        get
        {
            if (Language == 0)
                return "�������� ���������� � ��������!";
            else
                return "The connection to the server is lost!";
        }
    }

    public static string ConfirmPassword
    {
        get
        {
            if (Language == 0)
                return "����������� ������";
            else
                return "Confirm the password";
        }
    }

    public static string KeepMeSign
    {
        get
        {
            if (Language == 0)
                return "��������� ����";
            else
                return "Keep me sign";
        }
    }

    public static string AreYouSureToGetOut
    {
        get
        {
            if (Language == 0)
                return "�� ����� ������ �����?";
            else
                return "Are you sure you want to get out?";
        }
    }

    public static string No
    {
        get
        {
            if (Language == 0)
                return "���";
            else
                return "NO";
        }
    }

    public static string Yes
    {
        get
        {
            if (Language == 0)
                return "��";
            else
                return "YES";
        }
    }

    public static string ProgressWillBeReset
    {
        get
        {
            if (Language == 0)
                return "�������� ����� �������, �� �������?";
            else
                return "The progress will be reset, are you sure?";
        }
    }
}

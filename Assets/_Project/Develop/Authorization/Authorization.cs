using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text.RegularExpressions;


public class Authorization : MonoBehaviour
{
    private const string DOMEN = "http://45.90.34.57:9102/";

    private const string AUTH = "auth";
    private const string REGISTER = "register";
    private const string GET_RECORDS = "get_records";
    private const string EXIT = "exit";

    public PlayersData PlayersData;

    [SerializeField] private Button _logInBtn;
    [SerializeField] private Button _signInBtn;
    [SerializeField] private Button _guestBtn;

    [SerializeField] private TMP_InputField _loginUsernameText;
    [SerializeField] private TMP_InputField _loginPasswordText;
    
    [SerializeField] private TMP_InputField _registerUsernameText;
    [SerializeField] private TMP_InputField _registerPasswordText;
    [SerializeField] private TMP_InputField _confirmPasswordText;

    private bool _hasPlayersData;

    [SerializeField] private CanvasGroup _menuPanel;
    [SerializeField] private CanvasGroup _loginPanel;
    [SerializeField] private CanvasGroup _registerPanel;

    [SerializeField] private TMP_Text _messageText;

    [SerializeField] private Button _keepMeSignBtn;

    [SerializeField] private GameObject _mark;

    [SerializeField] private TMP_Text _keepMeSignText;

    [SerializeField] private Button _applicationExitBtn;

    private void Awake()
    {
        UserData.IsGuest = 0;

        UpdateCheckMark();

        if(UserData.IsAuthorizated == 1)
        {
            SetActiveAuthorizationPanel(false);
            SetActiveLoginPanel(false);
            SetActiveSignInPanel(false);

            UpdateUserData();

            return;
        }
        if (UserData.IsAuthorizated == 0 && UserData.KeepMeSignIn == 1 && UserData.Name != "user")
        {
            SetActiveAuthorizationPanel(false);
            SetActiveLoginPanel(false);
            SetActiveSignInPanel(false);

            UpdateUserData();

            return;
        }

        _guestBtn.onClick.AddListener(Guest);

        _logInBtn.onClick.AddListener(LogIn);

        _signInBtn.onClick.AddListener(SignIn);

        _keepMeSignBtn.onClick.AddListener(KeepMeSign);

        _applicationExitBtn.onClick.AddListener(Quit);

        UpdateUserData();

        _menuPanel.alpha = 1f;
        _menuPanel.interactable = false;
        _menuPanel.blocksRaycasts = false;
    }

    private void UpdateCheckMark()
    {
        if(UserData.KeepMeSignIn == 0)
        {
            _mark.SetActive(false);
        }
        else
        {
            _mark.SetActive(true);
        }
    }

    private void Quit()
    {
        SaveUserData();
    }

    private void Guest()
    {
        UserData.Name = "Guest";
        UserData.IsGuest = 1;
    }

    public void ShowMessage(string message)
    {
        _messageText.text = message;
    }

    IEnumerator ClearMessage()
    {
        yield return new WaitForSeconds(2.5f);

        ShowMessage("");
    }

    private void LogIn()
    {
        StartCoroutine(ClearMessage());

        if (string.IsNullOrEmpty(_loginPasswordText.text))
        {
            ShowMessage(Localization.EnterPassword);
            return;
        }
        if (string.IsNullOrEmpty(_loginUsernameText.text))
        {
            ShowMessage(Localization.EnterLogin);
            return;
        }
        if(UsernameContains(_loginUsernameText.text) == false)
        {
            ShowMessage(Localization.SuchPlayerAlreadyExists);
            return;
        }
        
        string data = JsonUtility.ToJson(new LoginData(_loginUsernameText.text, _loginPasswordText.text));

        StartCoroutine(AuthPostRequest(data));
    }

    private void SignIn()
    {
        StartCoroutine(ClearMessage());

        if (string.IsNullOrEmpty(_confirmPasswordText.text))
        {
            ShowMessage(Localization.ConfirmPassword);
            return;
        }
        if (string.IsNullOrEmpty(_registerPasswordText.text))
        {
            ShowMessage(Localization.EnterPassword);
            return;
        }
        if (string.IsNullOrEmpty(_registerUsernameText.text))
        {
            ShowMessage(Localization.EnterLogin);
            return;
        }
        if (_confirmPasswordText.text != _registerPasswordText.text)
        {
            ShowMessage(Localization.PasswordDoNotMatch);
            return;
        }
        if (UsernameContains(_registerUsernameText.text))
        {
            ShowMessage(Localization.SuchPlayerAlreadyExists);
            return;
        }
        if (ContainsNonLatin(_registerUsernameText.tag) == true || ContainsNumbersChar(_registerUsernameText.text) == false)
        {
            ShowMessage(Localization.LoginMustConsistLatinAndNumb);
            return;
        }
        if (ContainsNonLatin(_registerPasswordText.tag) == true || ContainsNumbersChar(_registerPasswordText.text) == false)
        {
            ShowMessage(Localization.PasswordMustConsistLatinAndNumb);
            return;
        }

        string data = JsonUtility.ToJson(new LoginData(_registerUsernameText.text, _registerPasswordText.text));

        StartCoroutine(RegisterPostRequest(data));
    }

    private bool ContainsNonLatin(string text)
    {
        string pattern = @"[^a-zA-Z\s]";
        Regex regex = new Regex(pattern);

        return regex.IsMatch(text);
    }

    private bool ContainsNumbersChar(string text)
    {
        foreach (char c in text)
        {
            if (char.IsDigit(c))
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateUserData()
    {
        StartCoroutine(RecordsGetRequest());
    }

    private bool UsernameContains(string name)
    {
        Debug.Log(PlayersData.PlayerDatas.Count);

        foreach (var playerData in PlayersData.PlayerDatas)
        {
            if (Equals(playerData.username, name))
            {
                return true;
            }
        }

        return false;
    }

    public void CheckContainsUsernameInLogin()
    {
        if (UsernameContains(_loginUsernameText.text))
        {
            
        }
        else
        {
            ShowMessage("Игрока с таким именем ещё нет");
        }
        StartCoroutine(ClearMessage());
    }

    public void CheckContainsUsernameInSignIn()
    {
        if (UsernameContains(_registerUsernameText.text))
        {
            ShowMessage("Игрок с таким именем уже существует");
        }
        else
        {
        }
        StartCoroutine(ClearMessage());
    }

    public void SaveUserData()
    {
        int maxDifficulty = 0;
        int level = UserData.EasyLevel;

        if (UserData.EasyWins > 2)
        {
            maxDifficulty = 1;
            level = UserData.NormalLevel;
        }
        if (UserData.NormalWins > 2) 
        {
            maxDifficulty = 2; 
        }
        if(UserData.HardWins > 2) 
        {
            maxDifficulty = 3;
        }  

        string data = JsonUtility.ToJson(new PlayerData(UserData.Name, UserData.Wins * 10 + UserData.Kills, maxDifficulty, level));
        StartCoroutine(ExitPostRequest(data));
    }

    public void SetActiveAuthorizationPanel(bool value)
    {
        _menuPanel.alpha = value ? 1 : 0;
        _menuPanel.interactable = value;
        _menuPanel.blocksRaycasts = value;
    }

    public void SetActiveLoginPanel(bool value)
    {
        _loginPanel.alpha = value ? 1f : 0f;
        _loginPanel.interactable = value;
        _loginPanel.blocksRaycasts = value;
    }

    public void SetActiveSignInPanel(bool value)
    {
        _registerPanel.alpha = value ? 1 : 0;
        _registerPanel.interactable = value;
        _registerPanel.blocksRaycasts = value;
    }

    private void KeepMeSign()
    {
        if (UserData.KeepMeSignIn == 0)
        {
            UserData.KeepMeSignIn = 1;
        }
        else
        {
            UserData.KeepMeSignIn = 0;
        }

        Debug.Log("Wow");

        UpdateCheckMark();
    }

    private void SetUserData(PlayerData data)
    {
        UserData.Wins = data.points / 10;
        UserData.Kills = data.points % 10;

        if (data.points > 3)
            UserData.EasyLevel = 3;
        if(data.points > 6)
            UserData.NormalLevel = 3;
        if (data.points > 9)
            UserData.HardLevel = 3;
        if (data.points > 12)
            UserData.HardcoreLevel = 3;
    }

    IEnumerator AuthPostRequest(string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(DOMEN + AUTH, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            ShowMessage("Потеряно соединение с сервером!");
            StartCoroutine(ClearMessage());
        }
        else if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            ShowMessage("Неверный логин или пароль");
            StartCoroutine(ClearMessage());
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            if (UsernameContains(_loginUsernameText.text))
                Debug.Log("");


            UserData.Name = _loginUsernameText.text;
            UserData.Password = _loginPasswordText.text;

            foreach (var data in PlayersData.PlayerDatas)
            {
                if (data.username == UserData.Name)
                    SetUserData(data);
            }

            SetActiveAuthorizationPanel(false);
            SetActiveLoginPanel(false);
            SetActiveSignInPanel(false);

            UserData.IsAuthorizated = 1;
        }
    }

    IEnumerator RegisterPostRequest(string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(DOMEN + REGISTER, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            ShowMessage("Потеряно соединение с сервером!");
        }
        else if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            ShowMessage("Такой пользователь уже зарегистрирован!");
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            ShowMessage("Вы успешно прошли регистрацию");

            UserData.Name = _registerUsernameText.text;
            UserData.Password = _registerPasswordText.text;

            StartCoroutine(ClearMessage());

            SetActiveSignInPanel(false);
            SetActiveAuthorizationPanel(false);
            SetActiveLoginPanel(false);

            UserData.IsAuthorizated = 1;
        }
    }

    IEnumerator RecordsGetRequest()
    {
        UnityWebRequest request = new UnityWebRequest(DOMEN + GET_RECORDS, "GET");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            List<PlayerData> data = JsonConvert.DeserializeObject<List<PlayerData>>(request.downloadHandler.text);

            PlayersData = new PlayersData();
            PlayersData.PlayerDatas = data;

            if(UserData.KeepMeSignIn == 0)
                SetActiveAuthorizationPanel(true);
        }
    }

    IEnumerator ExitPostRequest(string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(DOMEN + EXIT, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            Application.Quit();
        }
    }
}
public class LoginData
{
    public string username;
    public string password;

    public LoginData(string u, string p)
    {
        username = u;
        password = p;
    }
}
public class PlayerData
{
    public string username;
    public int points;
    public int difficulty;
    public int level;

    public PlayerData(string u, int p, int d, int l)
    {
        username = u;
        points = p;
        difficulty = d;
        level = l;
    }
}
public class PlayersData
{
    public List<PlayerData> PlayerDatas = new List<PlayerData>();
}
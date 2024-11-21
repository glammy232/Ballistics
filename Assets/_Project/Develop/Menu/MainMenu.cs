using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField] private TMP_Text _pointsText;

    [SerializeField] private TMP_Text _startBtnText;
    [SerializeField] private TMP_Text _recordsBtnText;
    [SerializeField] private TMP_Text _multiplayerBtnText;
    [SerializeField] private TMP_Text _rulesBtnText;
    [SerializeField] private TMP_Text _settingsBtnText;

    [SerializeField] private TMP_Text _easyBtnText;
    [SerializeField] private TMP_Text _normalBtnText;
    [SerializeField] private TMP_Text _hardBtnText;
    [SerializeField] private TMP_Text _hardcoreBtnText;

    [SerializeField] private TMP_Text _maxLevelsText;

    [SerializeField] private TMP_Text _winsToOpenNormalText;
    [SerializeField] private TMP_Text _winsToOpenHardText;

    [SerializeField] private Button _easyBtn;
    [SerializeField] private Button _normalBtn;
    [SerializeField] private Button _hardBtn;
    [SerializeField] private Button _hardcoreBtn;

    [SerializeField] private Complexityes _complexityes;

    [SerializeField] private TMP_Text _logInBtnText;
    [SerializeField] private TMP_Text _signInBtnText;
    [SerializeField] private TMP_Text _guestBtnText;

    [SerializeField] private TMP_Text _enterUsernameBtnText;
    [SerializeField] private TMP_Text _enterPasswordBtnText;
    [SerializeField] private TMP_Text _confirmPasswordBtnText;

    [SerializeField] private TMP_Text _enterUsernameBtnText0;
    [SerializeField] private TMP_Text _enterPasswordBtnText0;

    [SerializeField] private TMP_Text _keepMeSignText;

    [SerializeField] private TMP_Text _sureToExitText;
    [SerializeField] private TMP_Text _sureToExitText0;

    [SerializeField] private TMP_Text _noText;
    [SerializeField] private TMP_Text _noText0;
    [SerializeField] private TMP_Text _yesText;
    [SerializeField] private TMP_Text _yesText0;

    [SerializeField] private Authorization _authorization;

    private void Awake()
    {
        Instance = this;

        UpdateUI();
    }

    public void UpdateUI()
    {
        _logInBtnText.text = Localization.LogIn;
        _signInBtnText.text = Localization.SignIn;
        _guestBtnText.text = Localization.Guest;
        _enterUsernameBtnText.text = Localization.EnterLogin;
        _enterUsernameBtnText0.text = Localization.EnterLogin;
        _enterPasswordBtnText.text = Localization.EnterPassword;
        _enterPasswordBtnText0.text = Localization.EnterPassword;
        _confirmPasswordBtnText.text = Localization.ConfirmPassword;

        _keepMeSignText.text = Localization.KeepMeSign;

        _startBtnText.text = $"{Localization.Start}";
        _multiplayerBtnText.text = Localization.Multiplayer;
        _rulesBtnText.text = Localization.Rules;
        _settingsBtnText.text = Localization.Settings;
        _recordsBtnText.text = Localization.Records;

        _easyBtnText.text = Localization.Easy;
        _normalBtnText.text = Localization.Normal;
        _hardBtnText.text = Localization.Hard;
        _hardcoreBtnText.text = Localization.Hardcore;

        _sureToExitText.text = Localization.AreYouSureToGetOut;

        _sureToExitText0.text = Localization.AreYouSureToGetOut;

        _noText.text = Localization.No;
        _noText0.text = Localization.No;

        _yesText.text = Localization.Yes;
        _yesText0.text = Localization.Yes;

        _easyBtn.onClick.AddListener(_complexityes.Easy);

        _winsToOpenNormalText.text = $"{Localization.WinsToOpen} {Localization.Easy}: {3 - UserData.EasyWins}";
        _winsToOpenHardText.text = $"{Localization.WinsToOpen} {Localization.Normal}: {3 - UserData.NormalWins}";

        if (UserData.EasyWins > 2)
        {
            _winsToOpenNormalText.gameObject.SetActive(false);

            _normalBtn.onClick.RemoveAllListeners();

            _normalBtn.onClick.AddListener(_complexityes.Normal);
        }
        if(UserData.NormalWins > 2)
        {
            _winsToOpenHardText.gameObject.SetActive(false);

            _hardBtn.onClick.RemoveAllListeners();

            _hardBtn.onClick.AddListener(_complexityes.Hard);
        }
        if(UserData.HardWins > 2)
        {
            _hardcoreBtn.gameObject.SetActive(true);

            _hardcoreBtn.onClick.AddListener(_complexityes.Hardcore);
        }

        int finalPoints = UserData.Wins * 10 + UserData.Kills - UserData.Defeats;

        _pointsText.text = $"{Localization.Points}: {finalPoints}";

        if (finalPoints < 0)
            _pointsText.text = $"{Localization.Points}: 0";

        _maxLevelsText.text = $"{Localization.Maximum} {Localization.Level}: {UserData.MaxRound}";
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void Quit()
    {
        _authorization.SaveUserData();
    }
}

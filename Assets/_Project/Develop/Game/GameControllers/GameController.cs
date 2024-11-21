using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public event Action OnRoundStarting;
    public event Action OnRoundStarted;
    public event Action<Character> OnCurrentCharacterSelected;
    public event Action OnRoundStop;

    public event Action OnPlayerKilled;

    public static GameController Instance;

    private GameConfig _gameConfig;

    public GameConfig GameConfig => _gameConfig;

    [Header("Characters Settings")]

    private CharactersController _charactersController;

    public CharactersController GetCharactersController => _charactersController;

    [Header("Rounds Settings")]

    private RoundsController _roundsController;

    [SerializeField] private TMP_Text _roundText;

    private int _currentRound;
    private int currentRound
    {
        get => _currentRound;
        set
        {
            _currentRound = value;
            _roundText.text = $"{Localization.Round} {value}";
        }
    }

    [Header("UI")]
    #region UI

    [SerializeField] private TMP_Text _nextLevelButtonText;

    [SerializeField] private Button _nextLevelButton;

    [SerializeField] private MessagePanel _messagePanel;

    [SerializeField] private Button _exitButton;

    [SerializeField] private GameObject _endCanvas;

    [SerializeField] private TMP_Text _resultValuesText;
    #endregion
    [Header("Timer")]
    #region Timer

    private TimerModel _timer;

    public int GetTime => _timer.GetTime;

    #endregion
    [Header("Fields")]
    #region Fields
    [SerializeField] private List<Field> _fields;
    #endregion

    [SerializeField] private GameObject _exitPanel;

    [SerializeField] private TMP_Text _noText;
    [SerializeField] private TMP_Text _yesText;
    [SerializeField] private TMP_Text _sureText;

    public int MainPlayerKills;
    public int MainPlayerPersantageOfHits;

    public void Initialize(GameConfig gameConfig, CharactersController charactersController, RoundsController roundsController, TimerModel timerModel)
    {
        Instance = this;

        _gameConfig = gameConfig;

        _charactersController = charactersController;

        charactersController.OnCharacterKilled += delegate { StartCoroutine(StopRound()); };

        charactersController.OnCharactersLost += delegate { Debug.Log("End"); StartCoroutine(EndGame()); };

        OnPlayerKilled += delegate { StartCoroutine(EndGame()); };

        _roundsController = roundsController;

        _timer = timerModel;

        OnRoundStarting += delegate
        {
            _roundsController.ResetMap(_charactersController.GetCharacters);

            new PlayerPlacemaker().MovePlayersLittle(_charactersController.GetCharacters, new BordersPlacement(2f, 2f, -2f, -2f));

            currentRound++;

            _messagePanel.AddMessage($"Раунд {currentRound}");
            _messagePanel.AddMessage("HATE!");
        };

        OnRoundStop += delegate
        {

        };

        InitializeUI();

        StartRound();
    }

    public void Kill(int id)
    {
        foreach (Character ch in _charactersController.GetCharacters)
        {
            if(id == ch.GetID)
            {
                _charactersController.KillCharacter(ch, null);
                return;
            }
        }
    }

    private void InitializeUI()
    {
        int level = GameConfig.Complexity switch
        {
            0 => UserData.EasyLevel,
            1 => UserData.NormalLevel,
            2 => UserData.HardLevel,
            _ => UserData.HardcoreLevel,
        };

        _nextLevelButton.onClick.AddListener(delegate { GameEntryPoint.GameConfig = new GameConfig(level, GameConfig.Complexity, GameConfig.NumOfPlayers); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });

        _exitButton.onClick.AddListener(delegate
        {
            OpenSureToExitPanel();
        });
    }

    private void StartRound()
    {
        OnRoundStarting?.Invoke();

        _charactersController.MakeMainCharacter(_charactersController.GetNextCharacter());

        _timer.StartTimer((int)SettingsModel.RoundTime, delegate
        {
            _charactersController.KillCharacter(null, null);
        });
    }

    public void OpenSureToExitPanel()
    {
        Time.timeScale = 0f;

        _exitPanel.SetActive(true);
    }

    public void CloseSureToExitPanel()
    {
        Time.timeScale = 1f;

        _exitPanel.SetActive(false);
    }

    public void ExitIntoTheGame()
    {
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator StopRound()
    {
        OnRoundStop?.Invoke();

        _timer.StopTimer();

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 1f;

        StartRound();
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);

        _endCanvas.gameObject.SetActive(true);

        GameObject.Find("Map").SetActive(false);

        int final = 0;

        if (_charactersController.GetCharacters.Count == 1 && _charactersController.GetCharacters[0].TryGetComponent(out Player player))
        {
            final = 5 * GameConfig.NumOfPlayers + _charactersController.GetCharacters[0].Kills;

            final *= (GameConfig.Complexity + 1);

            _resultValuesText.text = $"Победа: 10\nПремия за выбитых: {MainPlayerKills}\n Премия за меткость: {MainPlayerPersantageOfHits / 60 * 2}\n\nИтог: {final}";

            UserData.Kills += MainPlayerKills + MainPlayerPersantageOfHits / 25;

            UserData.Wins += 1;

            if (GameConfig.Complexity == 0)
            {
                UserData.EasyLevel++;

                UserData.EasyWins++;

                if (UserData.EasyLevel > UserData.MaxRound)
                    UserData.MaxRound = UserData.EasyLevel;

                _nextLevelButtonText.text = $"{Localization.Level} {UserData.EasyLevel}";
            }
            else if (GameConfig.Complexity == 1)
            {
                UserData.NormalLevel++;
                UserData.NormalWins++;

                if (UserData.NormalLevel > UserData.MaxRound)
                    UserData.MaxRound = UserData.NormalLevel;

                _nextLevelButtonText.text = $"{Localization.Level} {UserData.NormalLevel}";
            }
            else if (GameConfig.Complexity == 2)
            {
                UserData.HardLevel++;
                UserData.HardWins++;

                if (UserData.HardLevel > UserData.MaxRound)
                    UserData.MaxRound = UserData.HardLevel;

                _nextLevelButtonText.text = $"{Localization.Level} {UserData.HardLevel}";
            }
            else if (GameConfig.Complexity == 3)
            {
                UserData.HardcoreLevel++;

                if(UserData.HardcoreLevel > UserData.MaxRound)
                    UserData.MaxRound = UserData.HardcoreLevel;

                _nextLevelButtonText.text = $"{Localization.Level} {UserData.HardcoreLevel}";
            }
        }
        else
        {
            foreach (var character in _charactersController.GetCharacters)
            {
                final = MainPlayerKills - 1;

                final *= (GameConfig.Complexity + 1);

                if (final < 0)
                    final = 0;

                _resultValuesText.text = $"Поражение: -1\nПремия за выбитых: {MainPlayerKills}\n Премия за меткость: {MainPlayerPersantageOfHits / 60 * 2}\n\nИтог: {final}";

                UserData.Kills += MainPlayerKills + MainPlayerPersantageOfHits / 25;

                UserData.Defeats += 1;
            }

            if (GameConfig.Complexity == 0)
                UserData.EasyLevel = 1;
            else if (GameConfig.Complexity == 1)
                UserData.NormalLevel = 1;
            else if (GameConfig.Complexity == 2)
                UserData.HardLevel = 1;
            else if (GameConfig.Complexity == 3)
                UserData.HardcoreLevel = 1;

            _nextLevelButtonText.text = "Level 1";
        }
    }
}
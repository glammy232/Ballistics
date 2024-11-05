using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public event Action OnRoundStarting;
    public event Action OnRoundStarted;
    public event Action<Character> OnCurrentCharacterSelected;
    public event Action OnRoundStop;

    public static GameController Instance;

    private GameConfig _gameConfig;

    [Header("Characters Settings")]

    private CharactersController _charactersController;

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
            _roundText.text = $"Round {value}";
        }
    }

    [Header("UI")]
    #region UI

    [SerializeField] private MessagePanel _messagePanel;

    [SerializeField] private Button _exitButton;

    [SerializeField] private GameObject _endCanvas;

    [SerializeField] private TMP_Text _resultValuesText;
    #endregion
    [Header("Timer")]
    #region Timer

    [SerializeField] private TimerModel _timer;

    public int GetTime => _timer.GetTime;

    #endregion
    [Header("Fields")]
    #region Fields
    [SerializeField] private List<Field> _fields;
    #endregion

    public void Initialize(GameConfig gameConfig)
    {
        Instance = this;

        _gameConfig = gameConfig;

        OnRoundStarting += delegate 
        {
            if (currentRound > 0)
            {
                _charactersController.KillCharacter();
            }

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

    private void InitializeControllers()
    {
        BotConfig botConfig = new BotConfig(100); 
    }

    private void InitializeUI()
    {
        _exitButton.onClick.AddListener(delegate
        {
            SceneManager.LoadSceneAsync("Menu");
        });
    }

    private void StartRound()
    {
        //if (canStartRound() == false)
          //  return;

        OnRoundStarting?.Invoke();

        
        _charactersController.GetCurrentCharacter.CanFire = true;

        if (/*hasPlayer() == false || */_charactersController.GetCurrentCharacter == null)
        {
            StartCoroutine(EndGame());
            return;
        }

        _timer.StartTimer((int)SettingsModel.RoundTime, delegate
        {
            StartCoroutine(StopRound());
        });

        /*bool canStartRound()
        {
            if (_characterToKill == null && _timer.GetTime > 0)
                return false;
            else if (_characters.Count <= 1)
                return false;
            else
                return true;
        }*/

        /*bool hasPlayer()
        {
            foreach (Character character in _characters)
            {
                if (character.TryGetComponent(out Player player))
                    return true;
            }

            return false;
        }*/
    }

    /*public void AddPlayerToKillHim(Character player, bool stopRound)
    {
        _characterToKill = player;

        player.DeathAnimation();

        _messagePanel.AddMessage($"Игрок {_characterToKill.GetID} выбывает");

        if (stopRound)
            StartCoroutine(StopRound());
    }*/

    public Character GetRandomCharacter(Character exclude) => _charactersController.GetRandomCharacter(exclude);

    public Character GetCharacterWithMinHealth(Character exclude) => _charactersController.GetCharacterWithMinHealth(exclude);

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
            //final = 10 + PlayerKills;

            //_resultValuesText.text = $"Победа: 10\nВыбил: {PlayerKills}\n\nИтог: {final}";

            //UserData.Kills += PlayerKills;
            UserData.Wins += 1;
        }
        else
        {
            //final = PlayerKills - 1;

            if (final < 0)
                final = 0;

            //_resultValuesText.text = $"Поражение: -1\nВыбил: {PlayerKills}\n\nИтог: {final}";

            //UserData.Kills += PlayerKills;
            UserData.Defeats += 1;
        }
    }
}
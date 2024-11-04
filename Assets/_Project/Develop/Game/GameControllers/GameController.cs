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

    [SerializeField] private RoundsController _roundsController;

    private List<Character> _characters;

    private Dictionary<int, int> _charactersKills = new Dictionary<int, int>();

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

    private Character _currentCharacter;

    public int GetCurrentCharacterID => _currentCharacter.GetID;

    private int _killedCharacterID;

    private Character _characterToKill;

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
    #endregion
    [Header("Fields")]
    #region Fields
    [SerializeField] private List<Field> _fields;
    #endregion

    private void Awake() => Initialize(4);

    private void Initialize(int numOfPlayers)
    {
        Instance = this;

        OnRoundStarting += delegate 
        {
            if (currentRound > 0 && _characterToKill != null)
            {
                _killedCharacterID = _characterToKill.GetID;

                _charactersController.KillCharacter(ref _characters, ref _characterToKill, ref _fields);
            }

            _roundsController.ResetMap(_characters);

            new PlayerPlacemaker().MovePlayersLittle(_characters, new BordersPlacement(2f, 2f, -2f, -2f));

            currentRound++;

            _messagePanel.AddMessage($"Раунд {currentRound}");
            _messagePanel.AddMessage("HATE!");
        };

        OnRoundStop += delegate
        {
            if (_characterToKill == null)
                AddPlayerToKillHim(_currentCharacter, true);
        };

        InitializeUI();

        InitializeCharacters(numOfPlayers);

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

    private void InitializeCharacters(int numOfPlayers)
    {
        _characters = _charactersController.CreateCharacters(numOfPlayers, _characters, _fields);

        HideFieldHealthBars();

        _charactersController.ArrangeCharacters(_characters);

        _charactersController.GivePoopToRandomCharacter(_characters);

    }

    private void StartRound()
    {
        if (canStartRound() == false)
            return;

        OnRoundStarting?.Invoke();

        if (currentRound > 1)
            OnCurrentCharacterSelected?.Invoke(_currentCharacter);

        _currentCharacter = _charactersController.FindNextPlayer(_killedCharacterID, currentRound, _characters, _currentCharacter);

        _currentCharacter.CanFire = true;

        if (hasPlayer() == false || _currentCharacter == null)
        {
            StartCoroutine(EndGame());
            return;
        }

        _timer.StartTimer((int)SettingsModel.RoundTime, delegate
        {
            StartCoroutine(StopRound());
        });

        bool canStartRound()
        {
            if (_characterToKill == null && _timer.GetTime > 0)
                return false;
            else if (_characters.Count <= 1)
                return false;
            else
                return true;
        }

        bool hasPlayer()
        {
            foreach (Character character in _characters)
            {
                if (character.TryGetComponent(out Player player))
                    return true;
            }

            return false;
        }
    }

    public void AddPlayerToKillHim(Character player, bool stopRound)
    {
        _characterToKill = player;

        player.DeathAnimation();

        _messagePanel.AddMessage($"Игрок {_characterToKill.GetID} выбывает");

        if (stopRound)
            StartCoroutine(StopRound());
    }

    private void HideFieldHealthBars()
    {
        foreach (var field in _fields)
        {
            if(field.ParentCharacter == null)
                field.HideHealthBar();
        }
    }

    public Character GetRandomCharacter(Character exclude) => _charactersController.GetRandomCharacter(exclude, ref _characters);

    public Character GetCharacterWithMinHealth(Character exclude) => _charactersController.GetCharacterWithMinHealth(exclude, ref _characters, _characters);

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
        if (_characters.Count == 1 && _characters[0].TryGetComponent(out Player player))
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
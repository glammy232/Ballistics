using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static event Action OnPlayerKilled;

    public static GameController Instance;

    [SerializeField] private Player _playerTemplate;
    [SerializeField] private Bot _botTemplate;

    [SerializeField] private Transform _ground;

    private List<Character> _characters;

    private int[] _charactersKills = new int[6];

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

    private Character _characterToKill;

    [SerializeField] private MessagePanel _messagePanel;

    [SerializeField] private Button _exitButton;

    [SerializeField] private GameObject _endCanvas;

    [SerializeField] private TMP_Text _resultValuesText;

    #region Timer

    [SerializeField] private TimerModel _timer;

    private const int ROUND_PERIOD = 15;

    #endregion

    [SerializeField] private List<Field> _fields;

    private void Awake() => Initialize(3);
    
    private void Initialize(int numOfPlayers)
    {
        Instance = this;

        _charactersKills = new int[numOfPlayers];

        _exitButton.onClick.AddListener(delegate { SceneManager.LoadSceneAsync("Menu"); });
        
        CreatePlayers(numOfPlayers);

        ArrangePlayers();

        ResetMap();

        GivePoopToRandomCharacter();

        StartRound();
    }

    private void CreatePlayers(int numOfPlayers)
    {
        _characters = new List<Character>();

        List<int> usedIDs = new List<int>();

        for (int i = 0; i < _fields.Count; i++)
        {
            int j = i;
            usedIDs.Add(j);
        }

        for (int i = 0; i < numOfPlayers; i++)
        {
            Character newCharacter;

            if (i == 0)
                newCharacter = Instantiate(_playerTemplate);
            else
                newCharacter = Instantiate(_botTemplate);

            int newID = usedIDs[UnityEngine.Random.Range(0, usedIDs.Count)];

            Debug.Log(newID);

            InitializationValueObject valueObject = new InitializationValueObject(_fields[newID], i + 1, SettingsModel.FireCooldown, 0f, 100f, 100f, 100, SettingsModel.SpeedKoaf);

            usedIDs.Remove(newID);

            newCharacter.Initialize(valueObject);

            _characters.Add(newCharacter);
        }

        HideFieldHeathBars();
    }

    private void HideFieldHeathBars()
    {
        for (int i = 0; i < _fields.Count; i++)
        {
            int value = 0;
            for (int j = 0; j < _characters.Count; j++)
            {
                if (_characters[j].GetField.GetHashCode() == _fields[i].GetHashCode())
                    value = 1;
            }
            
            if(value == 0)
                _fields[i].HideHealthBar();
        }
    }

    private void ArrangePlayers()
    {
        Transform[] charactersTransforms = new Transform[_characters.Count];

        for (int i = _characters.Count - 1; i > 0 ; i--)
        {
            charactersTransforms[_characters.Count - i] = _characters[i].transform;
        }

        /*charactersTransforms[0] = _characters[5].transform;
        charactersTransforms[1] = _characters[0].transform;
        charactersTransforms[2] = _characters[4].transform;
        charactersTransforms[3] = _characters[1].transform;
        charactersTransforms[4] = _characters[3].transform;
        charactersTransforms[5] = _characters[2].transform;*/

        PlayerPlacemaker playerPlacemaker = new PlayerPlacemaker();

        //playerPlacemaker.ArrangePlayers(charactersTransforms, _ground.position, _ground.localScale, -2, 2, -2, 2);

        playerPlacemaker.ArrangePlayers(_characters);

        foreach (var character in _characters)
        {
            character.SetStartPosition(character.transform.position);
        }
    }

    private void StartRound()
    {
        ResetMap();

        if(_characters.Count == 2)
        {
            PreEndGame();
            return;
        }

        PlayerPlacemaker playerPlacemaker = new PlayerPlacemaker();

        BordersPlacement bordersPlacement = new BordersPlacement(2f, 2f, -2f, -2f);

        playerPlacemaker.MovePlayersLittle(_characters, bordersPlacement);

        currentRound++;

        _messagePanel.AddMessage($"Раунд {currentRound}");
        _messagePanel.AddMessage("HATE!");

        _currentCharacter = FindNextPlayer();

        if(_currentCharacter == _characterToKill)
        {
            if (_characterToKill != null)
                KillCharacter(_characterToKill);

            _currentCharacter = FindNextPlayer(true);

            if (_currentCharacter == null)
            {
                if (_characterToKill != null)
                    KillCharacter(_characterToKill);

                PreEndGame();

                return;
            }

            SetPlayerFireAbility(_currentCharacter, true);

            _timer.StartTimer((int)SettingsModel.RoundTime, delegate
            {
                ResetMap();

                if (_characterToKill == null)
                    AddPlayerToKillHim(_currentCharacter, false);

                StopRound();
            });

            return;
        }

        if (_currentCharacter == null)
        {
            if (_characterToKill != null)
                KillCharacter(_characterToKill);

            PreEndGame();

            return;
        }
        
        SetPlayerFireAbility(_currentCharacter, true);

        _timer.StartTimer((int)SettingsModel.RoundTime, delegate
        {
            if(_characterToKill == null)
                AddPlayerToKillHim(_currentCharacter, false);
            
            StopRound();
        });

        if (_characterToKill != null)
            KillCharacter(_characterToKill);
    }

    private Character FindNextPlayer()
    {
        if(_characters.Count - 1 <= 1)
            return null;

        if (_currentCharacter == null && currentRound == 1)
            return _characters[currentRound - 1];

        if (_characters.IndexOf(_currentCharacter) != _characters.Count - 1)
        {
            return _characters[_characters.IndexOf(_currentCharacter) + 1];
        }
        else return _characters[0];
    }

    private Character FindNextPlayer(bool isNextAfterKilledPlayer)
    {
        return _characters[_characters.IndexOf(_currentCharacter) + 2];
    }

    private void ResetMap()// Подготавливает начальное состояние перед раундом
    {
        foreach (var character in _characters)
        {
            if(character != _characterToKill)
                character.GetComponent<Renderer>().material.color = Color.white;

            character.CanFire = false;
        }

        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();

        if (bullets.Length > 0)
        {
            for(int i = 0; i < bullets.Length; i++)
            {
                Bullet bullet = bullets[i];
                DestroyBullet(bullet);
            }
        }
    }

    private void SetPlayerFireAbility(Character player, bool value)
    {
        player.CanFire = value;

        if(value)
            player.GetComponent<Renderer>().material.color = Color.green;
    }

    public void KillCharacter(Character player)
    {
        Field field = _fields[_characters.IndexOf(player)];

        field.HideHealthBar();

        _fields.Remove(field);

        //Destroy(field.gameObject);

        player.GetComponent<Renderer>().material.color = Color.black;

        _charactersKills[player.ID-1] = player.Kills;

        _characters.Remove(player);

        Destroy(player);
    }

    private void StopRound()
    {
        StartCoroutine(CooldownBeetwenRounds(delegate
        {
            _timer.StopTimer();

            Time.timeScale = 0f;
        },
        delegate
        {
            Time.timeScale = 1f;
            
            StartRound();
        }));
    }

    public void AddPlayerToKillHim(Character player, bool stopRound)
    {
        _characterToKill = player;

        player.StartCoroutine(player.DeathAnimation());

        _messagePanel.AddMessage($"Игрок {_characterToKill.ID} выбывает");

        if (stopRound) 
            StopRound();
    }

    public void DestroyBullet(Bullet bullet)
    {
        if(bullet.gameObject != null)
            Destroy(bullet.gameObject);
    }

    private void PreEndGame()
    {
        StartCoroutine(EndGame(delegate
        {
            List<int> isDead = new List<int>();
            for(int i = 0; i < _characters.Count; i++)
            {
                if (_characters[i].Health <= 0)
                    isDead.Add(_characters[i].ID);
            }
            _endCanvas.gameObject.SetActive(true);

            int final = 0;
            if (_characters[0].ID == 1 && isDead.Contains(1) == false)
            {
                final = 10 + _charactersKills[0];
                _resultValuesText.text = $"Победа: 10\nВыбил: {_charactersKills[0]}\n\nИтог: {final}";
                UserData.Kills += _charactersKills[0];
                UserData.Wins += 1;
            }
            else
            {
                final = _charactersKills[0] - 1;

                if (final < 0)
                    final = 0;

                _resultValuesText.text = $"Поражение: -1\nВыбил: {_charactersKills[0]}\n\nИтог: {final}";
                UserData.Kills += _charactersKills[0];
                UserData.Defeats += 1;
            }

        }));
    }
    
    private void GivePoopToRandomCharacter() => _characters[UnityEngine.Random.Range(0, _characters.Count)].SetHasPoop(true);

    public Character GetRandomCharacterTransform() => _characters[UnityEngine.Random.Range(0, _characters.Count)];

    public Character GetCharacterWithMinHealth(Character execlude) => _characters.Find(x => x.Health == GetMinHealthOfCharacters(execlude) && execlude);

    private int GetMinHealthOfCharacters(Character execlude)
    {
        int minHealth = 100;

        foreach (var character in _characters)
        {
            if(minHealth > character.Health && character.ID != execlude.ID)
                minHealth = character.Health;
        }

        return minHealth;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    private IEnumerator EndGame(Action action)
    {
        yield return new WaitForSeconds(2f);

        action?.Invoke();
    }

    private IEnumerator CooldownBeetwenRounds(Action preCooldownAction, Action afterCooldownAction)
    {
        preCooldownAction?.Invoke();

        yield return new WaitForSecondsRealtime(SettingsModel.CooldownBetweenRounds);

        afterCooldownAction?.Invoke();
    }
}

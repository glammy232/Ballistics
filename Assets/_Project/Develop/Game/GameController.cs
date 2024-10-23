using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private int _killedCharacterID;

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

    private bool isRoundRunning = false;

    private void Awake() => Initialize(SettingsModel.NumOfPlayers);
    
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

    private void StartRound()
    {
        if (_characterToKill == null && _timer.GetTime > 0)
            return;

        if(_characterToKill != null)
            _killedCharacterID = _characterToKill.GetID;

        if (currentRound > 0)
            KillCharacter(_characterToKill);

        ResetMap();

        if (_characters.Count == 1)
        {
            Debug.Log("End2");
            PreEndGame();
            return;
        }

        PlayerPlacemaker playerPlacemaker = new PlayerPlacemaker();

        BordersPlacement bordersPlacement = new BordersPlacement(2f, 2f, -2f, -2f);

        playerPlacemaker.MovePlayersLittle(_characters, bordersPlacement);

        currentRound++;

        _messagePanel.AddMessage($"Раунд {currentRound}");
        _messagePanel.AddMessage("HATE!");

        if (currentRound > 1)
            _currentCharacter = FindNextPlayer();

        if (_currentCharacter != null)
        {
            SetPlayerFireAbility(_currentCharacter, true);

            if (_currentCharacter.CanFire == false)
                _currentCharacter.CanFire = true;

            _timer.StartTimer((int)SettingsModel.RoundTime, delegate
            {
                ResetMap();

                if (_characterToKill == null)
                    AddPlayerToKillHim(_currentCharacter, true);
            });

            return;
        }
        else
        {
            Debug.Log("End");
            PreEndGame();

            return;
        }
    }

    private Character FindNextPlayer()
    {
        int nextCharacterID = 0;

        if (_currentCharacter is null)
            Debug.Log("CURRENT CHARACTER IS NULL");
        else if(_currentCharacter.GetID == GetSortedCharacterIDs().Max())
            return _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min());

        if (_currentCharacter == null)
        {
            if(GetSortedCharacterIDs().Max() < _killedCharacterID)
                return _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min());
            else
            {
                for (int i = 0; nextCharacterID <= _killedCharacterID; i++)
                {
                    if (nextCharacterID > _killedCharacterID)
                        break;

                    nextCharacterID = GetSortedCharacterIDs()[i];
                    Debug.Log(nextCharacterID);

                    return _characters.Find((x) => x.GetID == nextCharacterID);
                }
            }
        }

        if (GetSortedCharacterIDs().Max() == _currentCharacter.GetID)
        {
            nextCharacterID = GetSortedCharacterIDs().Min();
        }
        else
        {
            for (int i = 0; nextCharacterID <= _currentCharacter.GetID; i++)
            {
                if (nextCharacterID > _currentCharacter.GetID)
                    break;

                nextCharacterID = GetSortedCharacterIDs()[i];
                Debug.Log(nextCharacterID);
            }
        }

        return _characters.Find((x) => x.GetID == nextCharacterID);
    }

    private void ResetMap()// Подготавливает начальное состояние перед раундом
    {
        foreach (Character character in _characters)
        {
            if(character != _characterToKill)
                character.GetComponent<Renderer>().material.color = Color.white;

            if(character.TryGetComponent(out Bot bot))
                bot.SetTargetCharacterToNull();

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
        Debug.Log("Player with ID: " + player.GetID + " Is Killed");

        player.GetField.HideHealthBar();
        
        _fields.Remove(player.GetField);

        player.GetComponent<Renderer>().material.color = Color.black;

        //_charactersKills[_characters.IndexOf(player)] = player.Kills;

        _characters.Remove(player);

        Destroy(player);

    }

    private void StopRound()
    {
        _timer.StopTimer();

        Time.timeScale = 0f;

        StartCoroutine(CooldownBeetwenRounds(delegate
        {
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

        //player.DeathAnimation();

        _messagePanel.AddMessage($"Игрок {_characterToKill.GetID} выбывает");

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
                    isDead.Add(_characters[i].GetID);
            }
            _endCanvas.gameObject.SetActive(true);

            int final = 0;
            if (_characters[0].GetID == 1 && isDead.Contains(1) == false)
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

    public Character GetRandomCharacter(Character execlude)
    {
        List<Character> characters = _characters;

        _characters.Remove(execlude);

        return characters[UnityEngine.Random.Range(0, characters.Count)];
    }

    public Character GetCharacterWithMinHealth(Character execlude)
    {
        Debug.Log("Character With Min Health Is: " + _characters.Find(x => x.Health == GetMinHealthOfCharacters(execlude)).name);

        if(GetMinHealthOfCharacters(execlude) == 100)
            return GetRandomCharacter(execlude);

        return _characters.Find(x => x.Health == GetMinHealthOfCharacters(execlude));
    }

    private int GetMinHealthOfCharacters(Character execlude)
    {
        int minHealth = 100;

        foreach (var character in _characters)
        {
            if(minHealth > character.Health && character.GetID != execlude.GetID && character.Health > 0)
                minHealth = character.Health;
        }

        return minHealth;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
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

            InitializationValueObject valueObject = new InitializationValueObject(_fields[newID], newID + 1, SettingsModel.FireCooldown, 0f, 100f, 100f, 100, SettingsModel.SpeedKoaf);

            _fields[newID].ParentCharacter = newCharacter;

            newCharacter.Initialize(valueObject);

            _characters.Add(newCharacter);

            usedIDs.Remove(newID);
        }

        usedIDs = new List<int>();

        for (int i = 0; i < _characters.Count; i++)
        {
            usedIDs.Add(_characters[i].GetID);
        }

        _currentCharacter = _characters.Find((x) => x.GetID == usedIDs.Min());

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

            if (value == 0)
                _fields[i].HideHealthBar();
        }
    }

    private void ArrangePlayers()
    {
        Transform[] charactersTransforms = new Transform[_characters.Count];

        for (int i = _characters.Count - 1; i > 0; i--)
        {
            charactersTransforms[_characters.Count - i] = _characters[i].transform;
        }

        PlayerPlacemaker playerPlacemaker = new PlayerPlacemaker();

        playerPlacemaker.ArrangePlayers(_characters);

        foreach (var character in _characters)
        {
            character.SetStartPosition(character.transform.position);
        }
    }

    private List<int> GetSortedCharacterIDs()
    {
        List<int> sorted = new List<int>();

        List<int> charactersIDs = new List<int>();

        foreach (var character in _characters)
        {
            charactersIDs.Add(character.GetID);
            Debug.Log("Characters ID " + character.GetID);
        }

        for (int i = 0; i < charactersIDs.Count; i++)
        {
            sorted.Add(charactersIDs.Min());
            charactersIDs.Remove(charactersIDs.Min());
        }

        sorted.Add(charactersIDs.Max());

        foreach (var sort in sorted)
        {
            Debug.Log("Sorted " + sort);
        }

        return sorted;
    }

    private IEnumerator EndGame(Action action)
    {
        yield return new WaitForSeconds(2f);

        action?.Invoke();
    }

    private IEnumerator CooldownBeetwenRounds(Action preCooldownAction, Action afterCooldownAction)
    {
        preCooldownAction?.Invoke();
        
        yield return new WaitForSecondsRealtime(1.5f);
        
        afterCooldownAction?.Invoke();
    }
}

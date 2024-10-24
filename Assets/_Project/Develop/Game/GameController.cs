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

    [SerializeField] private TMP_Text _persentOfHits;

    public int Hits;
    public int FiredBullets;

    [SerializeField] private Player _playerTemplate;
    [SerializeField] private Bot _botTemplate;

    [SerializeField] private Transform _ground;

    private List<Character> _characters;

    public int PlayerKills;

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

    private int _killedCharacterID;

    private Character _characterToKill;

    [SerializeField] private MessagePanel _messagePanel;

    [SerializeField] private Button _exitButton;

    [SerializeField] private GameObject _endCanvas;

    [SerializeField] private TMP_Text _resultValuesText;

    private int _mainPlayerID;

    #region Timer

    [SerializeField] private TimerModel _timer;

    private const int ROUND_PERIOD = 15;

    #endregion

    [SerializeField] private List<Field> _fields;

    private void Awake() => Initialize(SettingsModel.NumOfPlayers);

    private void LateUpdate()
    {
        _persentOfHits.text = ((float)Hits / (float)FiredBullets * 100).ToString();
    }

    private void Initialize(int numOfPlayers)
    {
        Instance = this;

        _exitButton.onClick.AddListener(delegate { SceneManager.LoadSceneAsync("Menu"); });
        
        CreatePlayers(numOfPlayers);

        ArrangePlayers();

        GivePoopToRandomCharacter();

        StartRound();
    }

    private void StartRound()
    {
        if (_characterToKill == null && _timer.GetTime > 0)
            return;

        if (currentRound > 0 && _characterToKill != null)
        {
            _killedCharacterID = _characterToKill.GetID;

            KillCharacter(_characterToKill);
        }

        ResetMap();

        if (_characters.Count == 1)
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

        if (currentRound > 1)
            _currentCharacter = FindNextPlayer();

        bool hasPlayer = false;

        foreach (Character character in _characters)
        {
            if (character.TryGetComponent(out Player player))
                hasPlayer = true;
        }

        if(hasPlayer == false)
            PreEndGame();

        if (_currentCharacter != null)
        {
            SetPlayerFireAbility(_currentCharacter, true);

            if (_currentCharacter.CanFire == false)
            {
                if(_currentCharacter.TryGetComponent(out Bot bot))
                    bot.SetTargetCharacterToNull();

                _currentCharacter.CanFire = true;
            }

            _timer.StartTimer((int)SettingsModel.RoundTime, delegate
            {
                if (_characterToKill == null)
                {
                    AddPlayerToKillHim(_currentCharacter, true);
                }
            });

            return;
        }
        else
        {
            PreEndGame();

            return;
        }
    }

    private Character FindNextPlayer()
    {
        int nextCharacterID = 0;

        if (_killedCharacterID > GetSortedCharacterIDs().Max() && currentRound == _killedCharacterID - 1)
            return _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min());

        if (_currentCharacter.GetID == GetSortedCharacterIDs().Max())
        {
            Debug.Log("Next CharacterID is: " + _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min()).GetID);
            return _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min());
        }
        else if (_currentCharacter == null)
        {
            if (GetSortedCharacterIDs().Max() < _killedCharacterID) 
            {
                Debug.Log("Next CharacterID is: " + _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min()));
                return _characters.Find((x) => x.GetID == GetSortedCharacterIDs().Min());
            }
            else
            {
                for (int i = 0; nextCharacterID <= _killedCharacterID; i++)
                {
                    nextCharacterID = GetSortedCharacterIDs()[i];

                    if (nextCharacterID > _killedCharacterID)
                    {
                        return _characters.Find((x) => x.GetID == nextCharacterID);
                    }

                    Debug.Log("Next CharacterID is: " + _characters.Find((x) => x.GetID == nextCharacterID).GetID);
                }
            }
        }
        else
        {
            for (int i = 0; nextCharacterID <= _currentCharacter.GetID; i++)
            {
                if (nextCharacterID > _currentCharacter.GetID)
                {
                    Debug.Log("Next ID is: " + nextCharacterID);
                    return _characters.Find((x) => x.GetID == nextCharacterID);
                }

                nextCharacterID = GetSortedCharacterIDs()[i];
            }
        }

        Debug.Log("NextPlayer ID: " + _characters.Find((x) => x.GetID == nextCharacterID).GetID);
        return _characters.Find((x) => x.GetID == nextCharacterID);
    }

    private void ResetMap()
    {
        Debug.Log("ResetMap");
        foreach (var character in _characters)
        {
            Debug.Log("Char On Reset ID: " + character.GetID);
            character.GetComponent<Renderer>().material.color = Color.white;

            if (character.TryGetComponent(out Bot bot))
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

        player.GetComponent<Renderer>().material.color = Color.black;

        _fields.Remove(player.GetField);

        _charactersKills.Add(player.GetID, player.Kills);

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

        player.DeathAnimation();

        Debug.Log("Add to kill: " + player.GetID);

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
            _endCanvas.gameObject.SetActive(true);

            GameObject.Find("Map").SetActive(false);

            int final = 0;
            if (_characters.Count == 1 && _characters[0].TryGetComponent(out Player player))
            {
               //if (_charactersKills.TryGetValue(_mainPlayerID, out int value))
               //{
                   final = 10 + PlayerKills;

                   _resultValuesText.text = $"Победа: 10\nВыбил: {PlayerKills}\n\nИтог: {final}";

                   UserData.Kills += PlayerKills;
                   UserData.Wins += 1;
               //}
            }
            else
            {
                //if (_charactersKills.TryGetValue(_mainPlayerID, out int value))
                //{
                    final = PlayerKills - 1;

                    if (final < 0)
                        final = 0;

                    _resultValuesText.text = $"Поражение: -1\nВыбил: {PlayerKills}\n\nИтог: {final}";

                    UserData.Kills += PlayerKills;
                    UserData.Defeats += 1;
                //}
            }

        }));
    }
    
    private void GivePoopToRandomCharacter() => _characters[UnityEngine.Random.Range(0, _characters.Count)].SetHasPoop(true);

    public Character GetRandomCharacter(Character execlude)
    {
        List<Character> characters = new List<Character>();

        for (int i = 0; i < _characters.Count; i++)
        {
            characters.Add(_characters[i]);
        }

        characters.Remove(execlude);

        return characters[UnityEngine.Random.Range(0, characters.Count)];
    }

    public Character GetCharacterWithMinHealth(Character execlude)
    {
        if(GetMinHealthOfCharacters(execlude) == 100)
            return GetRandomCharacter(execlude);

        return _characters.Find(x => x.GetHealth == GetMinHealthOfCharacters(execlude));
    }

    private int GetMinHealthOfCharacters(Character execlude)
    {
        int minHealth = 100;

        foreach (var character in _characters)
        {
            if(minHealth > character.GetHealth && character.GetID != execlude.GetID && character.GetHealth > 0)
                minHealth = character.GetHealth;
        }

        return minHealth;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    private void CreatePlayers(int numOfPlayers)//DELETE
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

            newCharacter.transform.parent = GameObject.Find("Map").transform;

            _characters.Add(newCharacter);

            if (newCharacter.TryGetComponent(out Player player))//DELETE
            {
                _mainPlayerID = player.GetID;
            }

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
        List<int> charactersIDs = new List<int>();

        foreach (var character in _characters)
        {
            charactersIDs.Add(character.GetID);
        }

        charactersIDs.Sort();

        return charactersIDs;
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

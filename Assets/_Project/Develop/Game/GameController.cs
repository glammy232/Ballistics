using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static event Action OnPlayerKilled;

    public static GameController Instance;

    [SerializeField] private Player _playerTemplate;

    [SerializeField] private Transform _ground;

    public List<Bullet> Bullets;

    private List<Player> _players;

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

    private Player _currentPlayer;

    private Player _playerToKill;

    [SerializeField] private MessagePanel _messagePanel;

    #region Timer

    [SerializeField] private TimerModel _timer;

    private const int ROUND_PERIOD = 15;

    #endregion

    [SerializeField] private List<Field> _fields;

    private void Awake() => Initialize(6);
    
    private void Initialize(int numOfPlayers)
    {
        Instance = this;

        CreatePlayers(numOfPlayers);

        ArrangePlayers();

        ResetMap();

        StartRound();
    }

    private void CreatePlayers(int numOfPlayers)
    {
        _players = new List<Player>();

        for (int i = 0; i < numOfPlayers; i++)
        {
            Player newPlayer = Instantiate(_playerTemplate);

            newPlayer.Initialize(_fields[i], i + 1);

            _players.Add(newPlayer);
        }
    }

    private void ArrangePlayers()
    {
        Transform[] playersTransforms = new Transform[_players.Count];

        for (int i = 0; i < playersTransforms.Length; i++)
        {
            playersTransforms[i] = _players[i].transform;
        }

        PlayerPlacemaker playerPlacemaker = new PlayerPlacemaker();

        playerPlacemaker.ArrangePlayers(playersTransforms, _ground.position, _ground.localScale, -2, 2, -2, 2);
    }

    private void StartRound()
    {
        ResetMap();

        currentRound++;

        _messagePanel.AddMessage($"Раунд {currentRound}");
        _messagePanel.AddMessage("HATE!");

        _currentPlayer = FindNextPlayer();

        if (_currentPlayer == null)
        {
            if (_playerToKill != null)
                KillPlayer(_playerToKill);

            EndGame();

            return;
        }
        
        SetPlayerFireAbility(_currentPlayer, true);

        _timer.StartTimer((int)SettingsModel.RoundTime, delegate
        {
            ResetMap();

            if(_playerToKill == null)
                AddPlayerToKillHim(_currentPlayer, false);
            
            StopRound();
        });

        if (_playerToKill != null)
            KillPlayer(_playerToKill);
    }

    private Player FindNextPlayer()
    {
        if(_players.Count - 1 <= 1)
            return null;

        if (_currentPlayer == null)
            return _players[currentRound];

        if (_players.IndexOf(_currentPlayer) != _players.Count - 1)
        {
            return _players[_players.IndexOf(_currentPlayer) + 1];
        }
        else return _players[0];
    }

    private void ResetMap()// Подготавливает начальное состояние перед раундом
    {
        if(Bullets.Count > 0)
        {
            foreach (var bullet in Bullets)
            {
                DestroyBullet(bullet);
            }
        }

        foreach (var player in _players)
        {
            player.GetComponent<Renderer>().material.color = Color.white;

            player.CanFire = false;
        }
    }

    private void SetPlayerFireAbility(Player player, bool value)
    {
        player.CanFire = value;

        if(value)
            player.GetComponent<Renderer>().material.color = Color.green;
    }

    public void KillPlayer(Player player)
    {
        Field field = _fields[_players.IndexOf(player)];

        _fields.Remove(field);  

        Destroy(field.gameObject);

        _players.Remove(player);

        Destroy(player);
    }

    private void StopRound()
    {
        StartCoroutine(CooldownBeetwenRounds(delegate
        {
            _timer.StopTimer();

            ResetMap();
        },
        delegate
        {
            StartRound();
        }));
    }

    public void AddPlayerToKillHim(Player player, bool stopRound)
    {
        _playerToKill = player;

        _messagePanel.AddMessage($"Игрок {_playerToKill.ID} выбывает");

        if (stopRound) 
            StopRound();
    }

    public void DestroyBullet(Bullet bullet)
    {
        if (Bullets.Contains(bullet))
            Bullets.Remove(bullet);

        Destroy(bullet.gameObject);
    }

    private void EndGame()
    {
        Debug.Log("End");
        //SceneManager.LoadScene("Menu");
    }

    private IEnumerator CooldownBeetwenRounds(Action preCooldownAction, Action afterCooldownAction)
    {
        preCooldownAction?.Invoke();

        yield return new WaitForSeconds(SettingsModel.CooldownBetweenRounds);

        afterCooldownAction?.Invoke();
    }
}

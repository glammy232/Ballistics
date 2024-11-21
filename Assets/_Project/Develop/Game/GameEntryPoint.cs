using System.Collections.Generic;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    [SerializeField] private RoundsController _roundsController;

    [SerializeField] private Player _playerTemplate;

    [SerializeField] private Bot _botTemplate;

    [SerializeField] private List<Field> _fields;

    [SerializeField] private TimerModel _timerModel;

    public static GameConfig GameConfig;

    public static BotConfig BotConfig;

    private void Awake()
    {
        CharactersController charactersController = new CharactersController(GameConfig, BotConfig, _playerTemplate, _botTemplate, _fields);

        _gameController.Initialize(GameConfig, charactersController, _roundsController, _timerModel);
    }
}

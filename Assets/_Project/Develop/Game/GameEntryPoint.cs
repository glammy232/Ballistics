using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    [SerializeField] private CharactersController _charactersController;

    [SerializeField] private RoundsController _roundsController;

    private void Awake()
    {

        GameConfig gameConfig = new GameConfig(1, 1, 6);

        _gameController.Initialize(gameConfig);
    }
}

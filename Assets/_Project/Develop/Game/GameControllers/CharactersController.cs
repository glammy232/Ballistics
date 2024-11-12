using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersController
{
    private BotConfig _botConfig;

    private Player _playerTemplate;

    private Bot _botTemplate;

    private List<Character> _characters;
    public List<Character> GetCharacters => _characters;

    private Character _currentCharacter;
    public Character GetCurrentCharacter => _currentCharacter;

    private int _lastCharacterId;

    private int _killedCharacterID;

    private List<Field> _fields;

    public CharactersController(GameConfig gameConfig, BotConfig botConfig, Player playerTemplate, Bot botTemplate, List<Field> fields)
    {
        _botConfig = botConfig;

        _playerTemplate = playerTemplate;

        _botTemplate = botTemplate;

        _fields = fields;

        _characters = CreateCharacters(gameConfig);

        _currentCharacter = SelectCurrentCharacter(0);

        HideFieldHealthBars();

        ArrangeCharacters(_characters);

        GivePoopToRandomCharacter(_characters);
    }

    public List<Character> CreateCharacters(GameConfig gameConfig)
    {
        List<Character> characters = new List<Character>();

        List<int> usedIDs = new List<int>();

        for (int i = 0; i < _fields.Count; i++)
        {
            int j = i;
            usedIDs.Add(j);
        }

        for (int i = 0; i < gameConfig.NumOfPlayers; i++)
        {
            Character newCharacter;

            if (i == 0)
                newCharacter = GameObject.Instantiate(_playerTemplate);
            else
            {
                newCharacter = GameObject.Instantiate(_botTemplate);

                newCharacter.GetComponent<Bot>().Initialize(_botConfig);
            }

            int newID = usedIDs[UnityEngine.Random.Range(0, usedIDs.Count)];

            InitializationValueObject valueObject = new InitializationValueObject(_fields[newID], newID + 1, SettingsModel.FireCooldown, 0f, 100f, 100f, 100, SettingsModel.SpeedKoaf);

            _fields[newID].ParentCharacter = newCharacter;

            newCharacter.Initialize(valueObject);

            newCharacter.transform.parent = GameObject.Find("Map").transform;

            characters.Add(newCharacter);

            usedIDs.Remove(newID);
        }

        usedIDs = new List<int>();

        for (int i = 0; i < characters.Count; i++)
        {
            usedIDs.Add(characters[i].GetID);
        }

        return characters;
    }

    public void ArrangeCharacters(List<Character> characters)
    {
        Transform[] charactersTransforms = new Transform[characters.Count];

        for (int i = characters.Count - 1; i > 0; i--)
        {
            charactersTransforms[characters.Count - i] = characters[i].transform;
        }

        PlayerPlacemaker playerPlacemaker = new PlayerPlacemaker();

        playerPlacemaker.ArrangePlayers(characters);

        foreach (var character in characters)
        {
            character.SetStartPosition(character.transform.position);
        }
    }

    public void GivePoopToRandomCharacter(List<Character> characters) => characters[UnityEngine.Random.Range(0, characters.Count)].SetHasPoop(true);

    public Character SelectCurrentCharacter(int currentRound)
    {
        if (currentRound == 0)
            return _characters.Find((ch) => ch.GetID == GetSortedCharacterIDs().Min());
        else if (GetCurrentCharacter == null && _characters.Count > _lastCharacterId + 1)
            return _characters.Find((ch) => ch.GetID == _lastCharacterId + 1);
        else if (_lastCharacterId + 1 > _characters.Count)
            return _characters[0];
        else
            return _characters[_lastCharacterId + 1];
    }

    public Character GetRandomCharacter(Character execlude)
    {
        List<Character> newCharacters = new List<Character>();

        for (int i = 0; i < _characters.Count; i++)
        {
            newCharacters.Add(_characters[i]);
        }

        newCharacters.Remove(execlude);

        return newCharacters[UnityEngine.Random.Range(0, newCharacters.Count)];
    }

    public Character GetCharacterWithMinHealth(Character execlude)
    {
        if (GetMinHealthOfCharacters(execlude) == 100)
            return GetRandomCharacter(execlude);
        
        return _characters.Find(x => x.GetHealth == GetMinHealthOfCharacters(execlude));
    }

    public void KillCharacter()
    {
        Character character = GetCharacterToKill();

        character.GetField.HideHealthBar();

        character.GetComponent<Renderer>().material.color = Color.black;

        _fields.Remove(character.GetField);

        _characters.Remove(character);

        GameObject.Destroy(character);
    }

    private Character GetCharacterToKill()
    {
        Character character = null;

        if (GameController.Instance.GetTime <= 0)
            character = _currentCharacter;
        else
            character = _characters.Find((x) => x.GetHealth == 0);

        if (character == null)
            throw new NullReferenceException("PROBLEM IN GetCharacterToKill Method");

        return character;
    }

    public List<int> GetSortedCharacterIDs()
    {
        List<int> charactersIDs = new List<int>();

        foreach (var character in _characters)
        {
            charactersIDs.Add(character.GetID);
        }

        charactersIDs.Sort();

        return charactersIDs;
    }

    public int GetMinHealthOfCharacters(Character exclude)
    {
        int minHealth = 100;

        foreach (var character in _characters)
        {
            if (minHealth > character.GetHealth && character.GetID != exclude.GetID && character.GetHealth > 0)
                minHealth = character.GetHealth;
        }

        return minHealth;
    }

    private void HideFieldHealthBars()
    {
        foreach (var field in _fields)
        {
            if (field.ParentCharacter == null)
                field.HideHealthBar();
        }
    }
}

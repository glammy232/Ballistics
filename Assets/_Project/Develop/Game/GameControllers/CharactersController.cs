using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersController
{
    public event Action OnCharacterKilled;
    public event Action OnCharactersLost;

    private BotConfig _botConfig;

    private GameConfig _gameConfig;

    private Player _playerTemplate;

    private Bot _botTemplate;

    private List<Character> _characters;
    public List<Character> GetCharacters => _characters;

    private Character _currentCharacter;
   
    private List<Field> _fields;

    private int _currentCharacterId = -1;

    public CharactersController(GameConfig gameConfig, BotConfig botConfig, Player playerTemplate, Bot botTemplate, List<Field> fields)
    {
        _gameConfig = gameConfig;

        _botConfig = botConfig;

        _playerTemplate = playerTemplate;

        _botTemplate = botTemplate;

        _fields = fields;

        _characters = CreateCharacters(gameConfig);

        HideFieldHealthBars();

        ArrangeCharacters(_characters);

        GivePoopToRandomCharacter(_characters);

        GiveHeartToRandomCharacter(_characters);
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

    private void ArrangeCharacters(List<Character> characters)
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

    private void GivePoopToRandomCharacter(List<Character> characters) => characters[UnityEngine.Random.Range(0, characters.Count)].SetHasPoop(true);

    private void GiveHeartToRandomCharacter(List<Character> characters)
    {
        int rand = UnityEngine.Random.Range(0, characters.Count);

        if (characters[rand].GetPoop)
        {
            GiveHeartToRandomCharacter(characters);
            return;
        }

        characters[rand].SetHasHeart(true);
    }

    private Character GetRandomCharacter(Character execlude)
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

    public void KillCharacter(Character killed, Character killer)
    {
        if (killed == null)
            killed = _currentCharacter;

        killed.GetField.HideHealthBar();

        killed.GetComponentInChildren<Renderer>().material.color = Color.black;

        _fields.Remove(killed.GetField);

        _characters.Remove(killed);

        GameObject.Destroy(killed);

        if (_characters.Count == 1)
        {
            OnCharactersLost?.Invoke();
        }

        if (killer != null)
            killer.Kills++;

        OnCharacterKilled?.Invoke();
    }

    public Character GetNextCharacter()
    {
        if (_currentCharacterId == GetCharacterIds().Count || _currentCharacterId == -1)
        {
            _currentCharacterId = GetCharacterIds().Min();

            Debug.Log("Min ID");
        }
        else if (GetSortedCharacterIds().IndexOf(_currentCharacterId) < GetCharacterIds().Count)
        {
            for (int i = 0; i < GetSortedCharacterIds().Count; i++)
            {
                Debug.Log("CurrentId " + _currentCharacterId);
          
                if (_currentCharacterId < GetSortedCharacterIds()[i])
                {
                    Debug.Log("Sorted Id " + GetSortedCharacterIds()[i]);
                    _currentCharacterId = GetSortedCharacterIds()[i];
                    Debug.Log("Next " +_currentCharacterId);
                    break;
                }
            }
        }
        else if (GetCharacterIds().Count <= 1)
        {
            Debug.LogError("Метод - GetNextCharacter. Кол-во игроков <= 1 ");
        }
        else
        {
            Debug.LogError("Метод - GetNextCharacter. Неизвестная ошибка в else");
        }

        Character character = null;

        foreach (var field in _fields)
        {
            if (field.ParentCharacter.GetID == _currentCharacterId)
            {
                character = field.ParentCharacter;
                _currentCharacterId = character.GetID; 
                Debug.Log("Выбран игрок с ID: " + field.ParentCharacter.GetID);
            }
        }

        if (character == null)
            Debug.LogError("Метод - GetNextCharacter. Возвращает null");

        return character;
    }

    private List<int> GetCharacterIds()
    {
        List<int> Ids = new List<int>();

        for (int i = 0; i < _fields.Count; i++)
        {
            int j = i;

            Ids.Add(_fields[j].ParentCharacter.GetID);
        }

        return Ids;
    }

    private List<int> GetSortedCharacterIds()
    {
        List<int> Ids = GetCharacterIds();

        Ids.Sort();

        return Ids;
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

    public void MakeMainCharacter(Character character)
    {
        character.GetComponentInChildren<Renderer>().material.color = Color.red;

        if (character.TryGetComponent(out Bot bot))
        {
            FindTargetCharacter(bot);
        }

        character.CanFire = true;

        _currentCharacter = character;
    }

    public void MakeShootingCharacter(Character character)
    {
        character.CanFire = true;

        if(character.TryGetComponent(out Bot bot))
        {
            FindTargetCharacter(bot);
        }
    }

    public void MakeKilledCharacter(Character character)
    {
        character.GetComponent<Renderer>().material.color = Color.black;

        character.CanFire = false;
    }

    public void MakeDefaultCharacter(Character character)
    {
        character.GetComponent<Renderer>().material.color = Color.white;

        character.CanFire = false;
    }

    public void FindTargetCharacter(Bot bot)
    {
        switch (_gameConfig.Complexity)
        {
            case 0:
                bot.SetTargetCharacter(GetNearestCharacter(bot));
                break;
            case 1:
                bot.SetTargetCharacter(GetRandomCharacter(bot));
                break;
            case 2:
                bot.SetTargetCharacter(GetCharacterWithMinHealth(bot));
                break;
            case 3:
                bot.SetTargetCharacter(GetCharacterWithMinHealth(bot));
                break;
        }
    }

    private Character GetNearestCharacter(Bot bot)
    {
        if (_characters.Count <= 1)
        {
            Debug.LogWarning("Метод GetNearestCharacter return null");
            return null;
        }

        Character ch = null;

        List<float> distances = new List<float>();

        foreach(var character in _characters)
        {
            if (character.GetID == bot.GetID)
                continue;

            distances.Add(Vector3.Distance(bot.transform.position, character.transform.position));
        }

        foreach(var character in _characters)
        {
            if (Vector3.Distance(bot.transform.position, character.transform.position) == distances.Min())
                ch = character;
        }

        if (ch == null)
            Debug.LogError("Метод GetNearestCharacter, ch is null");

        return ch;
    }
}

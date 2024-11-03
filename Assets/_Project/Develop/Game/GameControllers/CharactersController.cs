using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersController : MonoBehaviour
{
    [SerializeField] private Player _playerTemplate;

    [SerializeField] private Bot _botTemplate;

    public List<Character> CreateCharacters(int numOfPlayers, List<Character> characters, List<Field> fields)
    {
        characters = new List<Character>();

        List<int> usedIDs = new List<int>();

        for (int i = 0; i < fields.Count; i++)
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

            InitializationValueObject valueObject = new InitializationValueObject(fields[newID], newID + 1, SettingsModel.FireCooldown, 0f, 100f, 100f, 100, SettingsModel.SpeedKoaf);

            fields[newID].ParentCharacter = newCharacter;

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

        //currentCharacter = characters.Find((x) => x.GetID == usedIDs.Min());

        //currentCharacter.GetComponent<Renderer>().material.color = Color.red;

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

    public void GivePoopToRandomCharacter(List<Character> characters) => characters[Random.Range(0, characters.Count)].SetHasPoop(true);

    public Character FindCharacterToKill(List<Character> characters, Character currentCharacter)
    {
        Character character = characters.Find((x) => x.GetHealth == 0);

        if (character == null)
            character = currentCharacter;

        return character;
    }

    public Character FindNextPlayer(int killedCharacterID, int currentRound, List<Character> characters, Character currentCharacter)
    {
        int nextCharacterID = 0;

        List<int> characterIDs = GetSortedCharacterIDs(characters);

        if (killedCharacterID > characterIDs.Max() || currentCharacter == null || currentCharacter.GetID == characterIDs.Max() || currentCharacter == null && characterIDs.Max() < killedCharacterID)
            return characters.Find((x) => x.GetID == characterIDs.Min());
        else if (currentCharacter == null && characterIDs.Max() >= killedCharacterID)
        {
            for (int i = 0; nextCharacterID <= killedCharacterID; i++)
            {
                nextCharacterID = GetSortedCharacterIDs(characters)[i];

                if (nextCharacterID > killedCharacterID)
                    return characters.Find((x) => x.GetID == nextCharacterID);
            }
        }
        else
        {
            for (int i = 0; nextCharacterID <= currentCharacter.GetID; i++)
            {
                if (nextCharacterID > currentCharacter.GetID)
                    return characters.Find((x) => x.GetID == nextCharacterID);

                nextCharacterID = GetSortedCharacterIDs(characters)[i];
            }
        }

        return characters.Find((x) => x.GetID == nextCharacterID);
    }

    public Character GetRandomCharacter(Character execlude, ref List<Character> characters)
    {
        List<Character> newCharacters = new List<Character>();

        for (int i = 0; i < characters.Count; i++)
        {
            newCharacters.Add(characters[i]);
        }

        newCharacters.Remove(execlude);

        return newCharacters[Random.Range(0, newCharacters.Count)];
    }

    public Character GetCharacterWithMinHealth(Character execlude, ref List<Character> refCharacters, List<Character> characters)
    {
        if (GetMinHealthOfCharacters(execlude, refCharacters) == 100)
            return GetRandomCharacter(execlude, ref refCharacters);
        
        return characters.Find(x => x.GetHealth == GetMinHealthOfCharacters(execlude, characters));
    }

    public void KillCharacter(ref List<Character> characters, ref Character character, ref List<Field> fields)
    {
        character.GetField.HideHealthBar();

        character.GetComponent<Renderer>().material.color = Color.black;

        fields.Remove(character.GetField);

        characters.Remove(character);

        Destroy(character);
    }

    public List<int> GetSortedCharacterIDs(List<Character> characters)
    {
        List<int> charactersIDs = new List<int>();

        foreach (var character in characters)
        {
            charactersIDs.Add(character.GetID);
        }

        charactersIDs.Sort();

        return charactersIDs;
    }

    public int GetMinHealthOfCharacters(Character execlude, List<Character> characters)
    {
        int minHealth = 100;

        foreach (var character in characters)
        {
            if (minHealth > character.GetHealth && character.GetID != execlude.GetID && character.GetHealth > 0)
                minHealth = character.GetHealth;
        }

        return minHealth;
    }
}

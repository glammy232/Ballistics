using UnityEngine;

public class GameConfig
{
    public int Level;

    public int Complexity;

    public int NumOfPlayers;

    public GameConfig(int level, int complexity, int numOfPlayers)
    {
        Level = level;

        Complexity = complexity;

        NumOfPlayers = numOfPlayers;
    }
}

public class BotConfig
{
    [Range(0, 100)]
    public int PersentageOfHits;

    public BotConfig(int persentageOfHits)
    {
        PersentageOfHits = persentageOfHits;
    }
}
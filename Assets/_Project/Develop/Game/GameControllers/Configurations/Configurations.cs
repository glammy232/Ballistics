using UnityEngine;

public class GameConfig
{
    public int Level;

    public int Complexity;

    public int NumOfPlayers;
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
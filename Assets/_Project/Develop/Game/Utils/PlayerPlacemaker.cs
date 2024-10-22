using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacemaker
{
    public void ArrangePlayers(Transform[] players, Vector3 groundPosition, Vector3 groundScale, float minWidthOffset, float maxWidthOffset, float minHeightOffset, float maxHeightOffset)
    {
        float fieldHeight = groundScale.z / 6f;
        float fieldWidth = groundScale.x / 2f;

        Vector3 startPos = new Vector3(-groundScale.x / 2f + groundPosition.x + fieldWidth / 2f, groundPosition.y + 1f, groundScale.z / 2f + groundPosition.z - fieldHeight);

        int numOfProcessedPlayers = 0;

        while(numOfProcessedPlayers != players.Length)
        {
            Vector3 offset = new Vector3(Random.Range(minWidthOffset, maxWidthOffset), 0, Random.Range(minHeightOffset, maxHeightOffset));

            players[numOfProcessedPlayers].position = (numOfProcessedPlayers % 2) switch
            {
                0 => startPos + new Vector3(0, 0, -numOfProcessedPlayers * fieldHeight) + offset,
                _ => startPos + new Vector3(fieldWidth, 0, -(numOfProcessedPlayers - 1) * fieldHeight) + offset
            };

            numOfProcessedPlayers++;
        }
    }

    public void ArrangePlayers(List<Character> players)
    {
        List<int> usedIDs = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            int j = i;
            usedIDs.Add(j);
        }

        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = players[i].GetField.transform.GetChild(0).position;

            

            usedIDs.Remove(1);
        }
    }

    public void MovePlayersLittle(List<Character> players, BordersPlacement bordersPlacement)
    {
        foreach(var player in players)
        {
            Vector3 offset = new Vector3(Random.Range(bordersPlacement.MinX, bordersPlacement.MaxX), 0, Random.Range(bordersPlacement.MinY, bordersPlacement.MaxY));

            player.transform.position = player.StartPosition + offset;
        }
    }
}
public class BordersPlacement
{
    public float MaxX;
    public float MaxY; 
    public float MinX; 
    public float MinY;

    public BordersPlacement(float maxX, float maxY, float minX, float minY)
    {
        MaxX = maxX; 
        MaxY = maxY; 
        MinX = minX; 
        MinY = minY;
    }
}
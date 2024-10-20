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
}

using System.Collections.Generic;
using UnityEngine;

public class RoundsController : MonoBehaviour
{
    public void ResetMap(List<Character> characters)
    {
        foreach (var character in characters)
        {
            character.GetComponentInChildren<Renderer>().material.color = Color.green;

            if (character.TryGetComponent(out Bot bot))
                bot.SetTargetCharacter(null);

            character.CanFire = false;


        }

        Bullet[] bullets = FindObjectsOfType<Bullet>();

        if (bullets.Length == 0)
            return;

        for (int i = 0; i < bullets.Length; i++)
        {
            Bullet bullet = bullets[i];

            Destroy(bullet.gameObject);
        }
    }
}

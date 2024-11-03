using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundsController : MonoBehaviour
{
    public void ResetMap(List<Character> characters)
    {
        foreach (var character in characters)
        {
            character.GetComponent<Renderer>().material.color = Color.white;

            if (character.TryGetComponent(out Bot bot))
                bot.SetTargetCharacterToNull();

            character.CanFire = false;
        }

        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();

        if (bullets.Length == 0)
            return;

        for (int i = 0; i < bullets.Length; i++)
        {
            Bullet bullet = bullets[i];

            Destroy(bullet.gameObject);
        }
    }
}

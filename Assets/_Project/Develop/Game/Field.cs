using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    [SerializeField] private Image _healthBar;

    public void UpdateHealthView(Player player)
    {
         _healthBar.fillAmount = (float)player.Health / (float)Player.MAX_HEALTH;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Button _activatePoopButton;

    public Character ParentCharacter;

    private void Awake()
    {
        _activatePoopButton.onClick.AddListener(delegate {

            if (ParentCharacter.TryGetComponent(out Player player) && player.CanFire)
                HidePoopButton();
        });
    }

    public void UpdateHealthView(Character player)
    {
         _healthBar.fillAmount = (float)player.GetHealth / player.GetMaxHealth;
    }

    public void HideHealthBar() => _healthBar.transform.parent.gameObject.SetActive(false);

    public void HidePoopButton() => _activatePoopButton.gameObject.SetActive(false);

    public void ActivatePoopButtonObject() => _activatePoopButton.gameObject.SetActive(true);

    public void SetPoopButtonListener(Action action) => _activatePoopButton.onClick.AddListener(delegate { action?.Invoke(); });
}

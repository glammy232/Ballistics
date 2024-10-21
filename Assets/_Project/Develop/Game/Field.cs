using System;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Button _activatePoopButton;

    private void Awake()
    {
        _activatePoopButton.onClick.AddListener(delegate { _activatePoopButton.gameObject.SetActive(false); });
    }

    public void UpdateHealthView(Character player)
    {
         _healthBar.fillAmount = (float)player.Health / player.GetMaxHealth;
    }

    public void HideHealthBar() => Destroy(_healthBar.transform.parent.gameObject);

    public void ActivatePoopButtonObject() => _activatePoopButton.gameObject.SetActive(true);

    public void SetPoopButtonListener(Action action) => _activatePoopButton.onClick.AddListener(delegate { action?.Invoke(); });
}

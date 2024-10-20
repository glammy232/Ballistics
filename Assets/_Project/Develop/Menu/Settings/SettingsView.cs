using System;
using TMPro;
using UnityEngine;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private TMP_Text _maxheightText;

    [SerializeField] private TMP_Text _speedKoafText;

    [SerializeField] private TMP_Text _cooldownBetweenRoundsText;

    [SerializeField] private TMP_Text _fireCooldownText;

    [SerializeField] private TMP_Text _roundTimeText;

    private void Awake()
    {
        UpdateView();
    }

    public void UpdateView()
    {
        _maxheightText.text = Math.Round(SettingsModel.MaxHeight, 2).ToString();

        _speedKoafText.text = Math.Round(SettingsModel.SpeedKoaf, 2).ToString();

        _cooldownBetweenRoundsText.text = Math.Round(SettingsModel.CooldownBetweenRounds, 2).ToString();

        _fireCooldownText.text = Math.Round(SettingsModel.FireCooldown, 2).ToString();

        _roundTimeText.text = Math.Round(SettingsModel.RoundTime, 2).ToString();
    }
}

using System;
using TMPro;
using UnityEngine;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private TMP_Text _numOfPlayersText;

    private void Awake()
    {
        UpdateView();
    }

    public void UpdateView()
    {
        _numOfPlayersText.text = SettingsModel.NumOfPlayers.ToString();
    }
}

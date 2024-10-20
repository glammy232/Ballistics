using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsViewModel : MonoBehaviour
{
    [SerializeField] private SettingsModel _model;
    [SerializeField] private SettingsView _view;

    private void Awake() => Initialize();
    public void Initialize()
    {
        _model.PropertyChanged += delegate
        {
            _view.UpdateView();
        };
    }
}

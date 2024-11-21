using UnityEngine;

public class TimerViewModel : MonoBehaviour
{
    [SerializeField] private TimerModel _model;
    [SerializeField] private TimerView _view;

    private void Awake() => Initialize();

    public void Initialize()
    {
        _model.PropertyChanged += delegate { _view.UpdateView(_model); };
    }
}

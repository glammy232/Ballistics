using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimerModel : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private event Action onTimerCompleted;

    private int _time;
    private int time
    {
        get => _time;
        set
        {
            _time = value;

            NotifyPropertyChanged(nameof(_time));

            if (value <= 0)
            {
                onTimerCompleted?.Invoke();

                if (_countTime != null)
                    StopCoroutine(_countTime);
            }
        }
    }

    private bool _isWork = false;

    public bool GetIsWork => _isWork;

    public int GetTime => _time;

    private Coroutine _countTime;

    public void StartTimer(int time, Action action)
    {
        if (_countTime != null)
            StopCoroutine(_countTime);

        this.time = time;

        onTimerCompleted = action + delegate { _isWork = false; };

        _countTime = StartCoroutine(CountTime());

        _isWork = true;
    }

    public void StopTimer() 
    {
        if(_countTime != null)
            StopCoroutine(_countTime);

        onTimerCompleted = null;
    }

    private IEnumerator CountTime()
    {
        yield return new WaitForSeconds(1f);

        time--;

        if (_time > 0)
            _countTime = StartCoroutine(CountTime());
    }

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        if (PropertyChanged != null)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnDestroy()
    {
        onTimerCompleted -= onTimerCompleted;
    }
}

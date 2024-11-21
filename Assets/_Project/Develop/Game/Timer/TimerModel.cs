using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimerModel : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private event Action onTimerCompleted;

    [SerializeField] private AudioClip _audio;

    public AudioClip GetAudio => _audio;

    private int _time;
    private int time
    {
        get => _time;
        set
        {
            _time = value;

            if (value <= 0)
            {
                onTimerCompleted?.Invoke();

                if (_countTime != null)
                    StopCoroutine(_countTime);
            }

            if (value <= 3)
                _textColor = Color.red;

            NotifyPropertyChanged(nameof(_time));
        }
    }

    private Color _textColor;
    public Color GetColor => _textColor;

    [SerializeField] private AudioSource _audioSource;
    public AudioSource GetAudioSource => _audioSource;

    public int GetTime => _time;

    private Coroutine _countTime;

    public void StartTimer(int time, Action action)
    {
        if (_countTime != null)
            StopCoroutine(_countTime);

        this.time = time;

        onTimerCompleted = action;

        _textColor = Color.white;

        _countTime = StartCoroutine(CountTime());

        NotifyPropertyChanged(nameof(_countTime));
    }

    public void StopTimer()
    {
        _textColor = Color.white;

        if (_countTime != null)
            StopCoroutine(_countTime);

        onTimerCompleted = null;

        NotifyPropertyChanged();
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

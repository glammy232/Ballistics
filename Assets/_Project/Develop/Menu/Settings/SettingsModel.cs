using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SettingsModel : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public static float MaxHeight { get => PlayerPrefs.GetFloat(nameof(MaxHeight), 2.5f); set => PlayerPrefs.SetFloat(nameof(MaxHeight), value); }
    public static float SpeedKoaf { get => PlayerPrefs.GetFloat(nameof(SpeedKoaf), 4f); set => PlayerPrefs.SetFloat(nameof(SpeedKoaf), value); }
    public static float CooldownBetweenRounds { get => PlayerPrefs.GetFloat(nameof(CooldownBetweenRounds), 2f); set => PlayerPrefs.SetFloat(nameof(CooldownBetweenRounds), value); }
    public static float FireCooldown { get => PlayerPrefs.GetFloat(nameof(FireCooldown), 0.5f); set => PlayerPrefs.SetFloat(nameof(FireCooldown), value); }
    public static float RoundTime { get => PlayerPrefs.GetFloat(nameof(RoundTime), 30f); set => PlayerPrefs.SetFloat(nameof(RoundTime), value); }

    public void SetMaxHeight(float value)
    {
        MaxHeight += value;
        Notify();
    }
    public void SetSpeedKoaf(float value)
    {
        SpeedKoaf += value;
        Notify();
    }
    public void SetCooldownBetweenRounds(float value)
    {
        CooldownBetweenRounds += value;
        Notify();
    }
    public void SetFireCooldown(float value)
    {
        FireCooldown += value;
        Notify();
    }
    public void SetRoundTime(float value)
    {
        RoundTime += value;
        Notify();
    }

    private void Notify([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged != null)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

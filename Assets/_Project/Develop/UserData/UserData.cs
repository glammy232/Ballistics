using UnityEngine;

public class UserData : MonoBehaviour
{
    public static string Name { get => PlayerPrefs.GetString(nameof(Name), "user"); set => PlayerPrefs.SetString(nameof(Name), value); }

    public static int Wins { get => PlayerPrefs.GetInt(nameof(Wins), 0); set => PlayerPrefs.SetInt(nameof(Wins), value); }

    public static int Defeats { get => PlayerPrefs.GetInt(nameof(Defeats), 0); set => PlayerPrefs.SetInt(nameof(Defeats), value); }

    public static int Kills { get => PlayerPrefs.GetInt(nameof(Kills), 0); set => PlayerPrefs.SetInt(nameof(Kills), value); }
}

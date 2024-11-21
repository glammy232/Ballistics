using UnityEngine;

public class UserData : MonoBehaviour
{
    public static string Name { get => PlayerPrefs.GetString(nameof(Name), "user"); set => PlayerPrefs.SetString(nameof(Name), value); }

    public static string Password { get => PlayerPrefs.GetString(nameof(Password), ""); set => PlayerPrefs.SetString(nameof(Password), value); }

    public static int KeepMeSignIn { get => PlayerPrefs.GetInt(nameof(KeepMeSignIn), 0); set => PlayerPrefs.SetInt(nameof(KeepMeSignIn), value); }

    public static int Wins { get => PlayerPrefs.GetInt(nameof(Wins), 0); set => PlayerPrefs.SetInt(nameof(Wins), value); }

    public static int Defeats { get => PlayerPrefs.GetInt(nameof(Defeats), 0); set => PlayerPrefs.SetInt(nameof(Defeats), value); }

    public static int Kills { get => PlayerPrefs.GetInt(nameof(Kills), 0); set => PlayerPrefs.SetInt(nameof(Kills), value); }

    public static int MaxRound { get => PlayerPrefs.GetInt(nameof(MaxRound), 0); set => PlayerPrefs.SetInt(nameof(MaxRound), value); }

    public static int EasyWins { get => PlayerPrefs.GetInt(nameof(EasyWins), 0); set => PlayerPrefs.SetInt(nameof(EasyWins), value); }

    public static int NormalWins { get => PlayerPrefs.GetInt(nameof(NormalWins), 0); set => PlayerPrefs.SetInt(nameof(NormalWins), value); }

    public static int HardWins { get => PlayerPrefs.GetInt(nameof(HardWins), 0); set => PlayerPrefs.SetInt(nameof(HardWins), value); }

    public static int HardcoreWins { get => PlayerPrefs.GetInt(nameof(HardcoreWins), 0); set => PlayerPrefs.SetInt(nameof(HardcoreWins), value); }

    public static int EasyLevel { get => PlayerPrefs.GetInt(nameof(EasyLevel), 0); set => PlayerPrefs.SetInt(nameof(EasyLevel), value); }

    public static int NormalLevel { get => PlayerPrefs.GetInt(nameof(NormalLevel), 0); set => PlayerPrefs.SetInt(nameof(NormalLevel), value); }

    public static int HardLevel { get => PlayerPrefs.GetInt(nameof(HardLevel), 0); set => PlayerPrefs.SetInt(nameof(HardLevel), value); }

    public static int HardcoreLevel { get => PlayerPrefs.GetInt(nameof(HardcoreLevel), 0); set => PlayerPrefs.SetInt(nameof(HardcoreLevel), value); }

    public static int IsAuthorizated { get => PlayerPrefs.GetInt(nameof(IsAuthorizated), 0); set => PlayerPrefs.SetInt(nameof(IsAuthorizated), value); }

    public static int IsGuest { get => PlayerPrefs.GetInt(nameof(IsGuest), 0); set => PlayerPrefs.SetInt(nameof(IsGuest), value); }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Complexityes : MonoBehaviour
{
    private int _easyLevel => UserData.EasyLevel;
    private int _normalLevel => UserData.NormalLevel;
    private int _hardLevel => UserData.HardLevel;
    private int _hardcoreLevel => UserData.HardcoreLevel;
    public static int NumberOfPlayers { get => PlayerPrefs.GetInt(nameof(NumberOfPlayers), 6); set => PlayerPrefs.SetInt(nameof(NumberOfPlayers), value); }

    public void Easy()
    {
        GameEntryPoint.GameConfig = new GameConfig(_easyLevel, 0, NumberOfPlayers);

        SceneManager.LoadSceneAsync("Game");
    }

    public void Normal()
    {
        GameEntryPoint.GameConfig = new GameConfig(_normalLevel, 1, NumberOfPlayers);

        SceneManager.LoadSceneAsync("Game");
    }

    public void Hard()
    {
        GameEntryPoint.GameConfig = new GameConfig(_hardLevel, 2, NumberOfPlayers);

        SceneManager.LoadSceneAsync("Game");
    }

    public void Hardcore()
    {
        GameEntryPoint.GameConfig = new GameConfig(_hardcoreLevel, 3, NumberOfPlayers);

        SceneManager.LoadSceneAsync("Game");
    }
}

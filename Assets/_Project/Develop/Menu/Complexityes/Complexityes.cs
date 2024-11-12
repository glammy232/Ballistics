using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Complexityes : MonoBehaviour
{
    private int _easyLevel { get => PlayerPrefs.GetInt(nameof(_easyLevel), 0); set => PlayerPrefs.SetInt(nameof(_easyLevel), value); }
    private int _normalLevel { get => PlayerPrefs.GetInt(nameof(_normalLevel), 0); set => PlayerPrefs.SetInt(nameof(_normalLevel), value); }
    private int _hardLevel { get => PlayerPrefs.GetInt(nameof(_hardLevel), 0); set => PlayerPrefs.SetInt(nameof(_hardLevel), value); }
    private int _hardcoreLevel { get => PlayerPrefs.GetInt(nameof(_hardcoreLevel), 0); set => PlayerPrefs.SetInt(nameof(_hardcoreLevel), value); }

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

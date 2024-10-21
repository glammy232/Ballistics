using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _pointsText;

    private void Awake()
    {
        int finalPoints = UserData.Wins * 10 + UserData.Kills - UserData.Defeats;

        _pointsText.text = $"Очки: {finalPoints}";

        if(finalPoints < 0)
            _pointsText.text = "Очки: 0";
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;

    public void UpdateView(int time)
    {
        _timeText.text = string.Format("00:{0:d2}", time);
    }
}

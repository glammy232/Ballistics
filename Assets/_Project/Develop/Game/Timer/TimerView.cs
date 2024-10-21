using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;

    public void UpdateView(TimerModel model)
    {
        _timeText.text = string.Format("00:{0:d2}", model.GetTime);

        _timeText.color = model.GetColor;

        if (model.GetTime <= 3)
            model.GetAudioSource.PlayOneShot(model.GetAudio);
    }
}

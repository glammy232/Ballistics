using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _messageText;

    private const float MESSAGE_LIFETIME = 1.5f;

    private Queue<string> _messageQueue = new Queue<string>();

    private Coroutine _messageCycle;

    private void ShowMessage(string message)
    {
        _messageText.text = message;

        _messageCycle = StartCoroutine(HideMessage(MESSAGE_LIFETIME));
    }

    public void AddMessage(string message)
    {
        _messageQueue.Enqueue(message);

        if (_messageCycle == null)
            ShowMessage(_messageQueue.Dequeue());
    }

    private IEnumerator HideMessage(float time)
    {
        yield return new WaitForSeconds(time);

        _messageText.text = "";

        if (_messageQueue.Count > 0)
            ShowMessage(_messageQueue.Dequeue());
        else
            _messageCycle = null;
    }
}

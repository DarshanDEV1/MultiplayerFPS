using UnityEngine;
using TMPro;

public class ChatSendRcv : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text messageText;

    public void SetChatMessage(string playerName, string messaage)
    {
        nameText.text = playerName;
        messageText.text = messaage;
    }
}

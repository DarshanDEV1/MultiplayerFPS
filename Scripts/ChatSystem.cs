using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ChatSystem : MonoBehaviourPun
{
    [SerializeField] TMP_InputField _messageInput;
    [SerializeField] Button _sendButton;
    [SerializeField] GameObject _messagePrefab;
    [SerializeField] Transform _messagePanel;
    [SerializeField] Transform instantiatePosition;

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            _sendButton.onClick.AddListener(() =>
            {
                photonView.RPC("UpdateMessage", RpcTarget.AllBuffered);
            });
        }
    }

    [PunRPC]
    private void UpdateMessage()
    {
        GameObject messageInstance = PhotonNetwork.Instantiate(_messagePrefab.name, instantiatePosition.position, Quaternion.identity); //Instantiate(scorePrefab, instantiatePosition);
        messageInstance.transform.SetParent(instantiatePosition);
        messageInstance.GetComponent<ChatSendRcv>().SetChatMessage(PhotonNetwork.LocalPlayer.NickName, _messageInput.text);
    }
}

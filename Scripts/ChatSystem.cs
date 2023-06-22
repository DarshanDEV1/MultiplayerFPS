using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ChatSystem : MonoBehaviourPun
{
    public TMP_InputField _messageInput;
    public Button _sendButton;
    public GameObject _messagePrefab;
    public Transform instantiatePosition;
    public Transform _messagePanel;

    // Start is called before the first frame update
    void Start()
    {
        /*if(photonView.IsMine)
        {
            _sendButton.onClick.AddListener(() =>
            {
                photonView.RPC("UpdateMessage", RpcTarget.AllBuffered);
            });
        }*/
    }
    public void Call(string name, string message)
    {
        photonView.RPC("UpdateMessage", RpcTarget.AllBuffered, name, message);
    }
    [PunRPC]
    private void UpdateMessage(string name, string message)
    {
        GameObject messageInstance = PhotonNetwork.Instantiate(_messagePrefab.name, instantiatePosition.position, Quaternion.identity); //Instantiate(scorePrefab, instantiatePosition);
        messageInstance.transform.SetParent(instantiatePosition);
        messageInstance.GetComponent<ChatSendRcv>().SetChatMessage(name, message);
    }
}

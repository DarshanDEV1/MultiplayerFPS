using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MobileFpsGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform[] spawnPoints;
    public Button exitBTN;
    
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[randomPoint].position, Quaternion.identity);
            ////
        }
        exitBTN.onClick.AddListener(OnClickExitRoom);
    }

    public void OnClickExitRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        //PhotonNetwork.LoadLevel("LobbyScene");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ReconnectAndRejoin();
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}

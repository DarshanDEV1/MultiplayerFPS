using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Connection Status")]
    public Text connectionStatustext;

    [Header("Login Ui Panel")]
    public InputField playerNameInput;
    public GameObject loginUiPanel;
    

    [Header("Game options UI Panel")]
    public GameObject gameoptionsUiPanel;

    [Header("Create Room UI Panel")]
    public GameObject createRoomUiPanel;
    public InputField roomNameInputField;
    public InputField maxPlayers;

    [Header("Inside Room UI Panel")]
    public GameObject insideRoomUiPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerlistcontent;
    public GameObject startGameButton;

    [Header("Room List UI Panel")]
    public GameObject roomListUiPanel;
    public GameObject roomListPrefab;
    public GameObject roomListParent;

    [Header("Join Random room UI Panel")]
    public GameObject joinRandomRoomUiPanel;

    private Dictionary<string, RoomInfo> cachedRoomList;

    private Dictionary<string, GameObject> roomListGameObjects;

    private Dictionary<int, GameObject> playerlistGameobjects;
    


    #region unity methods

    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(loginUiPanel.name);
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatustext.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI callbacks

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if(!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if(string.IsNullOrEmpty(roomName))
        {
            roomName = "Room" + Random.Range(1000,10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayers.text);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameoptionsUiPanel.name);
    }

    public void OnShowRoomListButtonClicked()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(roomListUiPanel.name);

    }

    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(gameoptionsUiPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJonRandomRoomButtonClicked()
    {
        ActivatePanel(joinRandomRoomUiPanel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        
    }

    #endregion

    #region pun callbacks

    public override void OnConnectedToMaster()
    {
        ActivatePanel(gameoptionsUiPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is Created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomUiPanel.name); 
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerlistGameobject = Instantiate(playerListPrefab);
            playerlistGameobject.transform.SetParent(playerlistcontent.transform);
            playerlistGameobject.transform.localScale = Vector3.one;

            if(playerlistGameobjects == null)
            {
                playerlistGameobjects = new Dictionary<int, GameObject>();
            }

            playerlistGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            if(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerlistGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }
            else 
            {
                playerlistGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }

            playerlistGameobjects.Add(player.ActorNumber, playerlistGameobject);
        }

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();
        foreach(RoomInfo room in roomList)
        {
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList[room.Name] = room;
                }
                else
                {
                    cachedRoomList.Add(room.Name, room);
                }
            }

            
        }

        foreach(RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(roomListPrefab);
            roomListEntryGameObject.transform.SetParent(roomListParent.transform);
            roomListEntryGameObject.transform.localScale = Vector3.one;

            roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(()=>OnJoinRoomButtonClicked(room.Name));

            roomListGameObjects.Add(room.Name, roomListEntryGameObject);

        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerlistGameobject = Instantiate(playerListPrefab);
            playerlistGameobject.transform.SetParent(playerlistcontent.transform);
            playerlistGameobject.transform.localScale = Vector3.one;

            if(playerlistGameobjects == null)
            {
                playerlistGameobjects = new Dictionary<int, GameObject>();
            }

            playerlistGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
            if(newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerlistGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }
            else 
            {
                playerlistGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }

            playerlistGameobjects.Add(newPlayer.ActorNumber, playerlistGameobject);

            roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerlistGameobjects[otherPlayer.ActorNumber].gameObject);
        playerlistGameobjects.Remove(otherPlayer.ActorNumber);

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(gameoptionsUiPanel.name);

        foreach(GameObject player in playerlistGameobjects.Values)
        {
            Destroy(player);
        }

        playerlistGameobjects.Clear();
        playerlistGameobjects = null;


    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room" + Random.Range(1000,10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 16;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion

    #region public methods
    public void ActivatePanel(string panelToBeActivated)
    {
        loginUiPanel.SetActive(panelToBeActivated.Equals(loginUiPanel.name));
        gameoptionsUiPanel.SetActive(panelToBeActivated.Equals(gameoptionsUiPanel.name));
        createRoomUiPanel.SetActive(panelToBeActivated.Equals(createRoomUiPanel.name));
        insideRoomUiPanel.SetActive(panelToBeActivated.Equals(insideRoomUiPanel.name));
        joinRandomRoomUiPanel.SetActive(panelToBeActivated.Equals(joinRandomRoomUiPanel.name));
        roomListUiPanel.SetActive(panelToBeActivated.Equals(roomListUiPanel.name));
    }
    #endregion

    #region Private Methods

    void OnJoinRoomButtonClicked(string _roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(_roomName);
    }

    void ClearRoomList()
    {
        foreach(var roomlistobject in roomListGameObjects.Values)
        {
            Destroy(roomlistobject);
        }

        roomListGameObjects.Clear();
    }
    #endregion
}

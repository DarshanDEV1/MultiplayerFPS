using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class MobileFpsGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;
    public TMP_Text killsText;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] Button exitBTN;
    private Dictionary<int, int> playerKills = new Dictionary<int, int>();
    private int highestKills = 0;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject scorePrefab;
    [SerializeField] Transform instantiatePosition;
    private List<GameObject> scorePrefabs = new List<GameObject>();
    public bool gameOver = false;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[randomPoint].position, Quaternion.identity);
        }
        exitBTN.onClick.AddListener(OnClickExitRoom);
        GameOverPanel.SetActive(false);
        gameOver = false;
    }

    [PunRPC]
    public void PlayerKilled(string killerName, string victimName)
    {
        Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
        Player killer = otherPlayers.FirstOrDefault(p => p.NickName == killerName);

        if (killer != null)
        {
            int killerID = killer.ActorNumber;

            if (!playerKills.ContainsKey(killerID))
            {
                playerKills.Add(killerID, 0);
            }

            playerKills[killerID] += 1;
            UpdateScorePanel();

            // Check if the player has reached 3 kills
            if (playerKills[killerID] >= 3)
            {
                //GameOver(killer.NickName);
                //PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("GameOver", RpcTarget.AllBuffered, killer.NickName);
                gameOver = true;
            }

            // Check if the player has the highest kills
            if (playerKills[killerID] > highestKills)
            {
                highestKills = playerKills[killerID];
                UpdateKillsText(killerID);
            }
        }
    }

    private void UpdateKillsText(int playerID)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
        string playerName = player.NickName;
/*        killsText.text = "Highest Kills: " + playerName + " - " + highestKills.ToString();*/
    }

    [PunRPC]
    private void GameOver(string playerName)
    {
        //killsText.text = "Game Over! " + playerName + " wins!";
        GameOverPanel.SetActive(true);
        GameOverPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = "Winner is : " + playerName;
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ReconnectAndRejoin();
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void OnClickExitRoom()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ReconnectAndRejoin();
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    private IEnumerator ChangeScene(string SceneName)
    {
        Time.timeScale = 0;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1;
        PhotonNetwork.LoadLevel(SceneName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int playerID = otherPlayer.ActorNumber;
        if (playerKills.ContainsKey(playerID))
        {
            playerKills.Remove(playerID);
            UpdateHighestKills();
            UpdateScorePanel();
        }
    }

    private void UpdateHighestKills()
    {
        highestKills = 0;

        foreach (KeyValuePair<int, int> entry in playerKills)
        {
            if (entry.Value > highestKills)
            {
                highestKills = entry.Value;
                UpdateKillsText(entry.Key);
            }
        }
    }

    public void OnClickToggleScoreWindow()
    {
        scorePanel.SetActive(!scorePanel.activeInHierarchy);
    }

    /*    private void UpdateScorePanel()
        {
            ClearScorePanel();

            List<KeyValuePair<int, int>> sortedScores = playerKills.ToList();
            sortedScores.Sort((x, y) => y.Value.CompareTo(x.Value));

            foreach (KeyValuePair<int, int> scoreEntry in sortedScores)
            {
                Player player = PhotonNetwork.CurrentRoom.GetPlayer(scoreEntry.Key);
                string playerName = player.NickName;
                int playerScore = scoreEntry.Value;

                // Instantiate the score prefab across all clients
                GameObject scoreInstance = PhotonNetwork.Instantiate(scorePrefab.name, instantiatePosition.position, Quaternion.identity);
                scoreInstance.transform.SetParent(instantiatePosition);
                scoreInstance.GetComponent<PlayerScore>().SetPlayerScore(playerName, playerScore);
                scorePrefabs.Add(scoreInstance);
            }
        }*/

    private void UpdateScorePanel()
    {
        //ClearScorePanel();
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ClearScorePanel", RpcTarget.AllBuffered);

        List<KeyValuePair<int, int>> sortedScores = playerKills.ToList();
        sortedScores.Sort((x, y) => y.Value.CompareTo(x.Value));

        int instantiateIndex = 0; // Index to keep track of the instantiate position

        foreach (KeyValuePair<int, int> scoreEntry in sortedScores)
        {
            Player player = PhotonNetwork.CurrentRoom.GetPlayer(scoreEntry.Key);
            string playerName = player.NickName;
            int playerScore = scoreEntry.Value;

            // Instantiate the score prefab across all clients
            //PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("InstantiateScorePrefab", RpcTarget.AllBuffered, playerName, playerScore, instantiateIndex);

            instantiateIndex++; // Increment the instantiate index
        }
    }
    [PunRPC]
    public void InstantiateScorePrefab(string playerName, int playerScore, int instantiateIndex)
    {
        // Instantiate the score prefab
        GameObject scoreInstance = PhotonNetwork.Instantiate(scorePrefab.name, instantiatePosition.position, Quaternion.identity); //Instantiate(scorePrefab, instantiatePosition);
        scoreInstance.transform.SetParent(instantiatePosition);
        scoreInstance.GetComponent<PlayerScore>().SetPlayerScore(playerName, playerScore);
    }


    [PunRPC]
    private void ClearScorePanel()
    {
        /*foreach (GameObject scoreInstance in scorePrefabs)
        {
            // Destroy the score prefab across all clients
            PhotonNetwork.Destroy(scoreInstance);
        }

        scorePrefabs.Clear();*/
        GameObject[] scores = GameObject.FindGameObjectsWithTag("Score");
        foreach(GameObject obj in scores)
        {
            PhotonNetwork.Destroy(obj.gameObject);
        }
    }
}

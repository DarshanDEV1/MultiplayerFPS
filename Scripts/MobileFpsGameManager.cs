using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobileFpsGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform[] spawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[randomPoint].position, Quaternion.identity);
            ////
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class Networkmanager : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 10;
    public static Networkmanager instance;
    void Awake()
    {
        // if an instance already exists and it's not this one - destroy us
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;
        PhotonNetwork.CreateRoom(roomName);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        PhotonNetwork.JoinLobby();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }
    // instance
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("menu");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //GameManager.instance.CheckWinCondition();
        }
    }
}

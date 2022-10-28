using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
public class GameManager : MonoBehaviourPunCallbacks
{
    public float postGameTime;

    [Header("Players")]
    public string playerPrefabLocation; // path in Resources folder to the Player prefab
    public Transform[] spawnPoints; // array of all available spawn points
    public PlayerController[] players; // array of all the players
    public float respawnTime;
    private int playersInGame; // number of players in the game
    // instance
    public static GameManager instance;
    void Awake()
    {
        // instance
        instance = this;
    }
    private void Update()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }
    public void CheckWinCondition(int id)
    {
        if(players[id-1].gold >= 1000)
        {
            photonView.RPC("WinGame", RpcTarget.All,players, id);
        }
    }
    [PunRPC]
    void WinGame(int playerId)
    {
        // set the UI to show who's won
        GameUI.instance.SetWinText(GetPlayer(playerId).photonPlayer.NickName);
        Invoke("GoBackToMenu", postGameTime);
    }
    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom(); 
        Networkmanager.instance.ChangeScene("Menu");
    }
    [PunRPC]
    void SpawnPlayer()
    {
        // instantiate the player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        // get the player script
        PlayerController player = playerObj.GetComponent<PlayerController>();
        playerObj.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        reskin(player);
    }
    void reskin(PlayerController player)
    {
        for (int x = 0; x < players.Length-1; x++)
        { 
            Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);
            player.sr.color = newColor;
        }
    }
    public PlayerController GetPlayer(int playerId)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }
        return null;
    }
    public PlayerController GetPlayer(GameObject playerObject)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObject)
                return player;
        }
        return null;
    }
}

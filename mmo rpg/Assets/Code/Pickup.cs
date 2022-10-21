using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public enum PickupType
{
    Health,
    gold
}
public class Pickup : MonoBehaviourPun
{
    public PickupType type;
    public int value;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if(type == PickupType.Health)
            {
                player.photonView.RPC("Heal", player.photonPlayer, value);
            }
            if(type == PickupType.gold)
            {
                player.photonView.RPC("GiveGold", player.photonPlayer, value);
            }
            photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void DestroyPickup()
    {
        Destroy(gameObject);
    }
}

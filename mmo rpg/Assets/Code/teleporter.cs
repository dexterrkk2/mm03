using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporter : MonoBehaviour
{
    public Transform teleportLocation;
    public bool playerIsOverlapping;
    public Collider2D playercheck;
    public float delay = 2f;
    public float playerCheckTIme;
    private void Update()
    {
        if (playerIsOverlapping)
        {
            foreach (PlayerController player in GameManager.instance.players)
            {
                if (player.transform.position == playercheck.transform.position)
                {
                    player.transform.position = teleportLocation.position;
                    playerIsOverlapping = false;
                }
            }
        }
    }
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if(other.tag == "Player")
        {
            playerIsOverlapping = true;
            playercheck = other;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        playerIsOverlapping = false;
        playerCheckTIme = Time.time;
    }
}

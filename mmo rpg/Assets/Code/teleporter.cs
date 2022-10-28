using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporter : MonoBehaviour
{
    public Transform teleportLocation;
    public bool playerIsOverlapping;
    public Collider playercheck;
    private void Update()
    {
        if (playerIsOverlapping)
        {
            foreach (PlayerController player in GameManager.instance.players)
            {
                if (player.transform.position == playercheck.transform.position)
                {
                    Vector2 portalToPlayer = player.transform.position - transform.position;
                    float dotProduct = Vector3.Dot(transform.up, portalToPlayer);
                    if (dotProduct < 0f)
                    {
                        float rotationDiff = -Quaternion.Angle(transform.rotation, teleportLocation.rotation);
                        rotationDiff += 180;
                        player.transform.Rotate(Vector2.up, rotationDiff);
                        Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                        player.transform.position = teleportLocation.position + positionOffset;
                        playerIsOverlapping = false;
                    }
                }
            }
        }
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.tag == "Player")
        {
            playerIsOverlapping = true;
            playercheck = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerIsOverlapping = false;
    }
}

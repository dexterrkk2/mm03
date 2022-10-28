using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPun
{
    [Header("Info")]
    public string enemyName;
    public float moveSpeed;

    public int curhp;
    public float maxhp;

    public float chaseRange;
    public float attackRange;

    private PlayerController targetPlayer;

    public float playerDetectRate =.2f;
    private float lastPlayerDetectTime;

    public string objectToSpawnOnDeath;

    [Header("Attack")]
    public int damage;
    public float attackRate;
    public float lastAttackTime;

    [Header("Components")]
    public HeaderInfo healthbar;
    public SpriteRenderer sr;
    public Rigidbody2D rig;
    public int xpToGive;
    private void Start()
    {
        if (xpToGive == 0)
        {
            xpToGive = ((int)maxhp);
        }
        healthbar.Initialize(enemyName, ((int)maxhp));
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if(targetPlayer!= null)
        {
            float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if(dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                Attack();
            }
            else if (dist> attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rig.velocity = dir.normalized * moveSpeed;
            }
            else
            {
                rig.velocity = Vector2.zero;
            }
        }
        DetectPlayer();
    }
    void Attack()
    {
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);
    }
    void DetectPlayer()
    {
        if (Time.time -lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;
            foreach (PlayerController player in GameManager.instance.players)
            {
                
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                    {
                        targetPlayer = null;
                        rig.velocity = Vector2.zero;
                    }
                }
                else if (dist < chaseRange)
                {
                    if (targetPlayer == null)
                    {
                        targetPlayer = player;
                    }
                }
                
            }
            
        }
    }
    [PunRPC]
    public void TakeDamage(int damage,int playerid)
    {
        curhp -= damage;
        healthbar.photonView.RPC("UpdateHealthBar", RpcTarget.All, curhp, maxhp);
        // flash the player red
        // update the health bar UI
        // die if no health left
        if (curhp <= 0)
        { 
            Die(playerid);  
        }
        else
        {
            photonView.RPC("DamageFlash", RpcTarget.All);
        }
    }
    [PunRPC]
    void DamageFlash()
    {
        StartCoroutine(DamageFlashCoRoutine());
        IEnumerator DamageFlashCoRoutine()
        {

            Color defaultColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = defaultColor;
        }
    }
    void Die(int playerId)
    {
        GameManager.instance.players[playerId -1].photonView.RPC("GiveXp", GameManager.instance.players[playerId - 1].photonPlayer, xpToGive);
        if (objectToSpawnOnDeath != string.Empty)
        {
            PhotonNetwork.Instantiate(objectToSpawnOnDeath, transform.position, Quaternion.identity);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}

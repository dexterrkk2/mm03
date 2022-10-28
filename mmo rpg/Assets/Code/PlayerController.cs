using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Photon")]
    public int id;
    public Player photonPlayer;

    [Header("Stats")]
    public float moveSpeed;
    public int gold;
    public int curHp;
    public float maxHp;

    [Header("Components")]
    public Rigidbody2D rig;
    public int kills;
    public bool dead;
    public SpriteRenderer sr;
    public static PlayerController me;
    public Animator weaponAnim;
    public HeaderInfo headerInfo;
    [Header("Attack")]
    public float damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;
    public int xp;
    public float maxXp;
    public int level;
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(level);
        }
        else if (stream.IsReading)
        {
            level = (int)stream.ReceiveNext();
        }
    }
    void Attack()
    {
        lastAttackTime = Time.time;

        Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("enemy"))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, ((int)damage), id);
        }
        weaponAnim.SetTrigger("Attack");
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        Move();

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRange)
        {
            Attack();
        }

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;

        if (mouseX < 0)
        {
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
        }
    }
    [PunRPC]
    public void Initialize(Player player)
    {
        level = 1;
        maxXp = maxHp;
        id = player.ActorNumber;
        photonPlayer = player;
        //photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        headerInfo.Initialize(player.NickName, ((int)maxHp));
        // if this isn't our local player, disable physics as that's
        // controlled by the user and synced to all other clients
        if (player.IsLocal)
        {
            me = this;
        }
        else
        {
            rig.isKinematic = true;
        }
    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (dead)
            return;
        curHp -= damage;
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp, maxHp);
        // flash the player red
        // update the health bar UI
        // die if no health left
        if (curHp <= 0)
        {
            Die();
        }
        else
        {
            photonView.RPC("FlashDamage", RpcTarget.All);
        }
    }
    [PunRPC]
    void FlashDamage()
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
    void Die()
    {
        dead = true;
        gold = 0;
        GameUI.instance.UpdateGoldText(gold);
        if (PhotonNetwork.IsMasterClient)
        {
            //GameManager.instance.CheckWinCondition();
        }
        rig.isKinematic = true;
        transform.position = new Vector3(0, 99, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }
    [PunRPC]
    void GiveGold(int goldToGive)
    {
        gold += goldToGive;
        GameUI.instance.UpdateGoldText(gold);
    }
    [PunRPC]
    public void AddKill()
    {
        kills++;
    }
    [PunRPC]
    public void Heal(int amountToheal)
    {
        curHp = Mathf.Clamp(curHp + amountToheal, 0, ((int)maxHp));
        //GameUI.instance.UpdateHealthBar();
    }
    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        curHp = ((int)maxHp);
        rig.isKinematic = false;
    }
    [PunRPC]
    public void GiveXp(int xpToGive)
    {
        xp += xpToGive;
        if (xp > maxXp)
        {
            LevelUP();
        }
        headerInfo.photonView.RPC("UpdateXpBar", RpcTarget.All, xp, maxXp, id);
    }
    public void LevelUP()
    {
        xp -= ((int)maxXp);
        maxXp *= 1.2f;
        maxHp *= 1.2f;
        damage *= 1.2f;
        level++;
        curHp = ((int)maxHp);
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp, maxHp);
    }
}

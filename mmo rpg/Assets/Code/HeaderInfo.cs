using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfo : MonoBehaviourPun
{
    public TextMeshProUGUI nameText;
    public Image bar;
    private float maxValue;
    public Image xpBar;
    public bool isPlayer;
    public TextMeshProUGUI Level;
    public int level;
    public static HeaderInfo instance;
    public void Initialize(string text, int maxVal)
    {
        instance = this;
        nameText.text = text;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
        if (isPlayer)
        {
            xpBar.fillAmount = 0f;
        }
        else
        {
            xpBar.gameObject.SetActive(false);
        }
        Level.text = "LV:" + level;
    }

    [PunRPC]
    void UpdateHealthBar(int value, float maxHp)
    {
        bar.fillAmount = (float)value / maxHp;
    }
    [PunRPC]
    void UpdateXpBar(int value, float maxXp, int id)
    {
        xpBar.fillAmount = (float)value / maxXp;
        Level.text = "LV:" + GameManager.instance.players[id - 1].level; 
    }
}

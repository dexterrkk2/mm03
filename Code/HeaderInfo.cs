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
    public void Initialize(string text, int maxVal)
    {
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
    }

    [PunRPC]
    void UpdateHealthBar(int value)
    {
        bar.fillAmount = (float)value / maxValue;
    }
    [PunRPC]
    void UpdateXpBar(int value)
    {
        xpBar.fillAmount = (float)value / maxValue;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI goldTxt;

    public static GameUI instance;
    public TextMeshProUGUI winText;
    public Image winBackground;
    private void Awake()
    {
        instance = this;
    }
    public void SetWinText(string winnername)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnername + "wins";
    }
    public void UpdateGoldText(int gold)
    {
        goldTxt.text = "<b>Gold</b>:" + gold;
    }

}

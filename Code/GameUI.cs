using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI goldTxt;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateGoldText(int gold)
    {
        goldTxt.text = "<b>Gold</b>:" + gold;
    }
}

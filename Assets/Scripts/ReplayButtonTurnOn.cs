using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayButtonTurnOn : MonoBehaviour
{
    [SerializeField] private GameObject ReplayButton;
    Color orange = new Color32(255, 121, 0, 255); //FF7900
    Color pink = new Color32(255, 0, 255, 255); //FF00FF
    public void turnOnButton()
    {
        ReplayButton.SetActive(true);
        if (StaticWinner.rightPlayerLost)
        {
            ReplayButton.GetComponent<Image>().color = orange;
        }
        else
        {
            ReplayButton.GetComponent<Image>().color = pink;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnButtons : MonoBehaviour
{
    [SerializeField] private GameObject playButton;

    public void turnOnPlayButton()
    {
        Debug.Log("supposed to turn on button");
        playButton.SetActive(true);
    }
}

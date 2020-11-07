using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpauseScriptLinker : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void Unpause()
    {
        gameManager.GetComponent<GameManager>().Unpause();
        this.GetComponent<Animator>().SetBool("endPause", false);
    }
}

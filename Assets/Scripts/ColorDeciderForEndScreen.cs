using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorDeciderForEndScreen : MonoBehaviour
{
    [SerializeField] private Sprite orangeSprite;
    [SerializeField] private Sprite purpleSprite;
    [SerializeField] private GameObject purpleController;
    [SerializeField] private GameObject orangeController;
    [SerializeField] private GameObject ReplayButton;


    // Start is called before the first frame update
    void Start()
    {
        ReplayButton.SetActive(false);
        purpleController.SetActive(false);
        orangeController.SetActive(false);
        
        if (StaticWinner.rightPlayerLost)
        {
            orangeController.SetActive(true);
            orangeController.GetComponent<Animator>().SetTrigger("orange");
        }
        else
        {
            purpleController.SetActive(true);
            purpleController.GetComponent<Animator>().SetTrigger("purple");

        }
    }

    public void SetBackground(int rightPlayerLost)
    {
        if (rightPlayerLost == 1)
        {
            GetComponent<SpriteRenderer>().sprite = orangeSprite;
        }
        else if (rightPlayerLost == 2)
        {
            GetComponent<SpriteRenderer>().sprite = purpleSprite;
        }
    }
}

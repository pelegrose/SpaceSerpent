using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    private int paddingX;
    private int paddingY;
    private float timeToLeave, currentTime;

    public float selfDistructMinTime = 0, selfDistructMaxTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
        timeToLeave = Random.Range(selfDistructMinTime, selfDistructMaxTime) * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= timeToLeave)
        {
            SelfRemover();
        }
    }

    public void Init()
    {
        paddingY = 1;
        paddingX = 2;
        transform.position = new Vector2(
            Random.Range(GameManager.BottomLeft.x + paddingX, GameManager.TopRight.x - paddingX),
            Random.Range(GameManager.BottomLeft.y + paddingY, GameManager.TopRight.y - paddingY));
    }

    void SelfRemover()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
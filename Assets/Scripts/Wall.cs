using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(bool isTopWall)
    {
        Vector2 pos = Vector2.zero;
        if (isTopWall)
        {
            pos = new Vector2(0,GameManager.TopRight.y) + (Vector2.down * transform.localScale.y / 2);
        }
        else
        {
            pos = new Vector2(0,GameManager.BottomLeft.y) + (Vector2.up * transform.localScale.y / 2);
        }
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

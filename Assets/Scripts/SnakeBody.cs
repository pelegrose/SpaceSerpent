using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    public static bool isTriggered = false;
    public int placeInBody;
    // Start is called before the first frame update
    public void Init(int partNum)
    {
        placeInBody = partNum;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HeadTip") && placeInBody > 7 && !isTriggered)
        {
            Snake parent = other.GetComponentInParent<Snake>();
            isTriggered = true;
            Debug.Log("Snake collided with itself!");
            SoundsManager.Instance.PlaySnakeHitSelfSound();
            parent.ResetAfterCollision(placeInBody);
        }
    }
}
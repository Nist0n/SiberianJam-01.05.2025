using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public static bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;

    private void Update()
    {
        tap = swipeDown = swipeUp = swipeLeft = swipeRight = false;
        
        if (Input.GetKeyDown(KeyCode.A))
                    swipeLeft = true;
        
        if (Input.GetKeyDown(KeyCode.D))
                    swipeRight = true;
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
                    swipeDown = true;
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
//Variables
public Transform target;
public Vector3 followPosition;
public Vector3 offsetPosition;

private float xPos;
private float yPos;

    /*
    Camera needs to follow player character
    Z Axis needs to be positive number, example 10

    */

    void Start()
    {
        offsetPosition = new Vector3 (0,2,-15);
        
    }

    void Update()
    {
        xPos = target.position.x;
        yPos = target.position.y;
        
        followPosition = new Vector3 (xPos,yPos,0);
        followPosition =  followPosition+offsetPosition;

        



        transform.position = followPosition;
    }






}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowFish : BaseFish
{
    void Start()
    {
        swimSpeed = 1f;  
        turnSpeed = 1f;
        changeDirectionTime = 5f; 
    }
}


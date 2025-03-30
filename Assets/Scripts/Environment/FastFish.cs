using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastFish : BaseFish
{
    void Start()
    {
        swimSpeed = 5f;
        turnSpeed = 3f;
        changeDirectionTime = 2f;
    }
}

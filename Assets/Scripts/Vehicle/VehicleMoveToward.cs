using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMoveToward : Obstacle
{
    private void Awake()
    {
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
    }

    public void KillPlayer()
    {
        PlayerControllerSmooth.Instance.Die();
    }
}

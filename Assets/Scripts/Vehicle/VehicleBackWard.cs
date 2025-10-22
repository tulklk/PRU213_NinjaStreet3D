using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleBackWard : ObstacleMove
{
    private PlayerControllerSmooth playerController;

    private void Awake()
    {
        playerController = GameObject.FindObjectOfType<PlayerControllerSmooth>();
        canMove = false;
    }

    public void StartMoving()
    {
        canMove = true;
    }

    public void KillPlayer()
    {
        playerController.Die();
    }
}

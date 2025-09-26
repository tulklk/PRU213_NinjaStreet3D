using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppeeFood : ObstacleMove
{
    private PlayerControllerSmooth playerController;
    private void Awake()
    {
        playerController = GameObject.FindObjectOfType<PlayerControllerSmooth>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.Die();
        }
    }
}


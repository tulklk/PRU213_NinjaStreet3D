using UnityEngine;

public class Bus : Obstacle
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

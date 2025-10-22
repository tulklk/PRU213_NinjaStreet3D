using UnityEngine;

public class Grab : ObstacleMove
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform menuCameraPosition;  
    public Transform playerCameraPosition; 
    public float transitionSpeed = 2.0f; 
    private bool isMovingToPlayer = false; 
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        SetMenuCamera(); 
    }

    void Update()
    {
       
        if (Input.GetMouseButtonDown(0) && !isMovingToPlayer)
        {
            MoveToPlayerView();
        }

        
        if (isMovingToPlayer)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, playerCameraPosition.position, Time.deltaTime * transitionSpeed);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, playerCameraPosition.rotation, Time.deltaTime * transitionSpeed);

            
            if (Vector3.Distance(mainCamera.transform.position, playerCameraPosition.position) < 0.1f)
            {
                mainCamera.transform.position = playerCameraPosition.position;
                mainCamera.transform.rotation = playerCameraPosition.rotation;
                isMovingToPlayer = false;
            }
        }
    }

    public void SetMenuCamera()
    {
        mainCamera.transform.position = menuCameraPosition.position;
        mainCamera.transform.rotation = menuCameraPosition.rotation;
        isMovingToPlayer = false;
    }

    public void MoveToPlayerView()
    {
        isMovingToPlayer = true;
    }
}

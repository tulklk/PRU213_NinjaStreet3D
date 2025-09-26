using Cinemachine;
using UnityEngine;

public class LockCameraFOV : MonoBehaviour
{
    public float fixedFOV = 60f;

    void LateUpdate()
    {
        var cam = GetComponent<CinemachineVirtualCamera>();
        if (cam != null)
            cam.m_Lens.FieldOfView = fixedFOV;
    }
}

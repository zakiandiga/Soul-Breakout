using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using ECM.Controllers;

public class GhostBody : MonoBehaviour
{
    private FirstPersonCinemachine control;
    private CinemachineVirtualCamera virtualCam;

    public static event Action<GhostBody> OnGhostBodyReady; 

    private void Start()
    {
        control = GetComponent<FirstPersonCinemachine>();
        virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();

        OnGhostBodyReady?.Invoke(this);
        virtualCam.Priority = 10;
    }

    
}

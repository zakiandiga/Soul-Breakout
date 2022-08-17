using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using ECM.Controllers;

public class GhostBody : MonoBehaviour
{
    private FirstPersonCinemachine control;
    private CinemachineVirtualCamera virtualCam;

    public static event Action<GhostBody> OnGhostBodyReady; 

    private IEnumerator Start()
    {
        control = GetComponent<FirstPersonCinemachine>();
        virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
        
        yield return new WaitForSeconds(1.2f);

        OnGhostBodyReady?.Invoke(this);
        virtualCam.Priority = 10;

        
    }

    
}

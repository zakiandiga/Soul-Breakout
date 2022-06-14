using System;
using UnityEngine;
using Cinemachine;

//put this on the main camera with the CinemachineBrain
[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineBlendMonitor : MonoBehaviour
{
    public static event Action<CinemachineBlendMonitor> OnCameraBlendStarted;
    public static event Action<CinemachineBlendMonitor> OnCameraBlendFinished;

    private CinemachineBrain cinemachineBrain;

    private bool blendingOnProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cinemachineBrain.IsBlending)
        {
            if(!blendingOnProgress)
            {
                OnCameraBlendStarted?.Invoke(this);
            }

            blendingOnProgress = true;
        }

        else
        {
            if(blendingOnProgress)
            {
                OnCameraBlendFinished?.Invoke(this);
            }
            blendingOnProgress = false;
        }
    }
}

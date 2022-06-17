using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;

//put this on the main camera with the CinemachineBrain
[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineBlendMonitor : MonoBehaviour
{
    public static event Action<CinemachineBlendMonitor> OnCameraBlendStarted;
    public static event Action<CinemachineBlendMonitor> OnCameraBlendFinished;

    private CinemachineBrain cinemachineBrain;
    [SerializeField] private PostProcessVolume postPro;
    private ColorGrading colorGrading;

    private bool blendingOnProgress = false;

    private float smoothToTime = 0.3f;
    private float smoothFromTime = 0.12f;
    private float smoothRef;
    private float currentSmoothValue;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
        postPro.profile.TryGetSettings(out colorGrading);

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

        if(blendingOnProgress)
        {
            if(colorGrading.saturation.value != -80f)
            {
                colorGrading.saturation.value = Mathf.SmoothDamp(colorGrading.saturation.value, -80, ref smoothRef, smoothToTime);
            }
        }
        else if(!blendingOnProgress)
        {
            if(colorGrading.saturation.value != 0)
            {
                colorGrading.saturation.value = Mathf.SmoothDamp(colorGrading.saturation.value, 0, ref smoothRef, smoothFromTime);
                if (Mathf.Abs(colorGrading.saturation.value) < 0.1f)
                    colorGrading.saturation.value = 0;
            }
        }
    }
}

using System;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

//put this on the main camera with the CinemachineBrain
[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineBlendMonitor : MonoBehaviour
{
    public static event Action<CinemachineBlendMonitor> OnCameraBlendStarted;
    public static event Action<CinemachineBlendMonitor> OnCameraBlendFinished;



    private CinemachineBrain cinemachineBrain;
    //[SerializeField] private PostProcessVolume postPro;
    //private ColorGrading colorGrading;
    [SerializeField] private Volume uPostPro;
    private VolumeProfile volumeProfile;
    private ColorAdjustments colorAdjustment;
    private ChromaticAberration chroma;
    private LensDistortion lensDistortion;

    private bool blendingOnProgress = false;

    private float smoothToTime = 0.3f;
    private float smoothFromTime = 0.12f;
    private float smoothRef;
    private float currentSaturationSmoothValue;

    private float distortionSmoothTime = 0.8f;
    private float currentLensDistortionValue;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();
        //postPro.profile.TryGetSettings(out colorGrading);
        //uPostPro.TryGetComponent(out colorAdjustment);
        volumeProfile = uPostPro.profile;
        volumeProfile.TryGet(out colorAdjustment);
        volumeProfile.TryGet(out chroma);
        volumeProfile.TryGet(out lensDistortion);


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
            currentSaturationSmoothValue = colorAdjustment.saturation.value;
            chroma.intensity.value = 1;

            if(currentSaturationSmoothValue != -80f)
            {
                currentSaturationSmoothValue = Mathf.SmoothDamp(colorAdjustment.saturation.value, -80, ref smoothRef, smoothToTime);
                colorAdjustment.saturation.value = currentSaturationSmoothValue;
            }

            if(currentLensDistortionValue != -0.40f)
            {
                currentLensDistortionValue = Mathf.SmoothDamp(lensDistortion.intensity.value, -0.40f, ref smoothRef, distortionSmoothTime);
                lensDistortion.intensity.value = currentLensDistortionValue;
            }
        }
        else if(!blendingOnProgress)
        {
            currentSaturationSmoothValue = colorAdjustment.saturation.value;
            chroma.intensity.value = 0;

            if (currentSaturationSmoothValue != 0)
            {
                currentSaturationSmoothValue = Mathf.SmoothDamp(colorAdjustment.saturation.value, 0, ref smoothRef, smoothFromTime);
                if (Mathf.Abs(currentSaturationSmoothValue) < 0.1f)
                    currentSaturationSmoothValue = 0;

                colorAdjustment.saturation.value = currentSaturationSmoothValue;
                //colorAdjustment.saturation.Override()
            }

            if(currentLensDistortionValue != 0)
            {
                currentLensDistortionValue = Mathf.SmoothDamp(lensDistortion.intensity.value, 0, ref smoothRef, distortionSmoothTime);
                if (Mathf.Abs(currentLensDistortionValue) < 0.01f)
                    currentLensDistortionValue = 0;

                lensDistortion.intensity.value = currentLensDistortionValue;
            }
        }
    }
}

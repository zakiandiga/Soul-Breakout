using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class PostProcessCameraBlend : MonoBehaviour
{
    public static event Action<PostProcessCameraBlend> OnCameraBlendStarted;
    public static event Action<PostProcessCameraBlend> OnCameraBlendFinished;    

    private CinemachineBrain cinemachineBrain;
    [SerializeField] private Volume uPostPro;
    private VolumeProfile volumeProfile;
    private ColorAdjustments colorAdjustment;
    private ChromaticAberration chroma;
    private LensDistortion lensDistortion;

    private bool blendingOnProgress = false;

    private float smoothToDuration = 0.3f;
    private float smoothFromDuration = 0.12f;
    private float smoothRef;

    private float defaultSaturationValue = 0f;
    private float defaultLensDistortionValue = 0f;
    private float shiftedSaturationValue = -80f;
    private float shiftedLensDistortionValue = -0.40f;

    private float SaturationTargetValue => blendingOnProgress ? shiftedSaturationValue : defaultSaturationValue;
    private float lensDistortionTargetValue => blendingOnProgress ? shiftedLensDistortionValue : defaultLensDistortionValue;
    private float SmoothDuration => blendingOnProgress ? smoothToDuration : smoothFromDuration;


    void Start()
    {        
        cinemachineBrain = GetComponent<CinemachineBrain>();

        volumeProfile = uPostPro.profile;
        volumeProfile.TryGet(out colorAdjustment);
        volumeProfile.TryGet(out chroma);
        volumeProfile.TryGet(out lensDistortion);
    }

    private void ToggleCameraBlend()
    {
        if(!cinemachineBrain.IsBlending)
        {
            if (blendingOnProgress)
            {
                OnCameraBlendFinished?.Invoke(this);
            }
            blendingOnProgress = false;
            return;
        }

        if (!blendingOnProgress)
        {
            OnCameraBlendStarted?.Invoke(this);
        }

        blendingOnProgress = true;

    }

    private void ShiftPostProcessValue()
    {            
        this.chroma.intensity.value = blendingOnProgress ? 1 : 0;

        colorAdjustment.saturation.value = SmoothShift(colorAdjustment.saturation.value, SaturationTargetValue);
        lensDistortion.intensity.value = SmoothShift(lensDistortion.intensity.value, lensDistortionTargetValue);
    }

    void Update()
    {
        ToggleCameraBlend();

        if (!blendingOnProgress && CheckGhostBody())
            return;

        ShiftPostProcessValue();        
    }

    private float SmoothShift(float currentValue, float targetValue)
    {
        if (currentValue == targetValue)
            return targetValue;

        float currentSmoothValue;
        currentSmoothValue = Mathf.SmoothDamp(currentValue, targetValue, ref smoothRef, SmoothDuration);
        return currentSmoothValue;
    }

    private bool CheckGhostBody()
    {
        return cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform.root.GetComponent<GhostBody>() == null ? false : true;
    }
}

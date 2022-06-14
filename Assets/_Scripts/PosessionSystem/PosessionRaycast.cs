using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using ECM.Controllers;

[RequireComponent(typeof(FirstPersonCinemachine))]
public class PosessionRaycast : MonoBehaviour
{
    /*
    [SerializeField] private Material highlightMaterial; 
    [SerializeField] private string selectableTag = "Player";
    [SerializeField] private Material defaultMaterial;
    private Transform _selection;
    */

    private Camera cam; //need camera reference for ScreenPointToRay
    private FirstPersonCinemachine control;

    private CinemachineVirtualCamera currentVirtualCamera;

    private Transform targetPossess;
    private FirstPersonCinemachine targetControl;
    private CinemachineVirtualCamera targetVirtualCamera;
    private PosessionRaycast targetPossessionRaycast;

    private bool isPossessing = false;


    private void Start()
    {
        cam = Camera.main;
        currentVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        targetPossess = null;
        targetControl = null;
        targetVirtualCamera = null;
        targetPossessionRaycast = null;

        control ??= GetComponent<FirstPersonCinemachine>();
        control.OnPossessPressed += TryPossess;
    }

    private void OnDisable()
    {
        control.OnPossessPressed -= TryPossess;

    }

    public void TryPossess(bool possessPressed)
    {
        if (!possessPressed)
            return;

        var ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 500, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if(!isPossessing)
            {
                targetPossess = hit.transform;
                isPossessing = true;
                if (targetPossess.TryGetComponent(out FirstPersonCinemachine targetPossessControl))
                {
                    targetControl = targetPossessControl;

                    Debug.Log(transform.root.name + ": Can possess " + targetControl.transform.root + " the target, now possessing!");
                    Possess();
                }

                else
                {
                    Debug.Log("Target cannot be possessed!");
                    isPossessing = false;
                }
            }

            /*
                if (_selection != null)
                {
                    var selectionRenderer = _selection.GetComponent<Renderer>();
                    selectionRenderer.material = defaultMaterial;
                    _selection = null;
                }
            
            if (selection.CompareTag(selectableTag))
            {
                var selectionRenderer = selection.GetComponentInChildren<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial;
                }

                //temp direct possess
                var vcam = selection.GetComponentInChildren<CinemachineVirtualCamera>();
                if(vcam != null)
                {
                    vcam.m_Priority = 11;
                }

                var controller = selection.GetComponent<FirstPersonCinemachine>();
                if(controller != null)
                {
                    controller.enabled = true;
                }

                _selection = selection;
            }
            */

        }
    }

    private void Possess()
    {
        CinemachineBlendMonitor.OnCameraBlendFinished += FinalizePossess;
        control.enabled = false;
        targetVirtualCamera = targetPossess.GetComponentInChildren<CinemachineVirtualCamera>();
        targetPossessionRaycast = targetPossess.GetComponent<PosessionRaycast>();
        
        Debug.Log(currentVirtualCamera.Priority);

        if (targetVirtualCamera != null)
        {
            currentVirtualCamera.Priority = 1;
            targetVirtualCamera.Priority = 10;
        }
        
    }

    private void FinalizePossess(CinemachineBlendMonitor c)
    {
        CinemachineBlendMonitor.OnCameraBlendFinished -= FinalizePossess;

        Debug.Log("Finish possessing");

        targetControl.enabled = true;
        targetPossessionRaycast.enabled = true;

        isPossessing = false;
        this.enabled = false;
    }

}

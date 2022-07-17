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
    private FirstPersonCinemachine controlPlayer;
    private EnemyAI controlAI;

    private CinemachineVirtualCamera currentVirtualCamera;

    private Transform targetPossess;
    private FirstPersonCinemachine targetControl;
    private CinemachineVirtualCamera targetVirtualCamera;
    private PosessionRaycast targetPossessionRaycast;

    private bool isPossessing = false;

    public ParticleSystem Laser;
    //public ParticleSystem LaserPrefab;
    //public Transform LaserPivot;

    [SerializeField] private float possessRange = 500f;
    [SerializeField] private LayerMask possessingRaycastLayer;


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
        //targetPossessionRaycast = null;

        controlPlayer ??= GetComponent<FirstPersonCinemachine>();
        controlAI ??= GetComponent<EnemyAI>();
        controlPlayer.OnPossessPressed += TryPossess;
        controlAI.OnAITryPosses += TryPossess;
    }

    private void OnDisable()
    {
        controlPlayer.OnPossessPressed -= TryPossess;
        controlAI.OnAITryPosses -= TryPossess;

    }

    public void TryPossess(bool possessingObserved)
    {
        if (!possessingObserved)
            return;

        var ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));  //crosshair
        Debug.DrawRay(ray.origin, ray.direction * 500, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, possessRange, possessingRaycastLayer))
        {
            
            if(!isPossessing)
            {
                targetPossess = hit.transform;
                isPossessing = true;
                if (targetPossess.TryGetComponent(out FirstPersonCinemachine targetPossessControl))
                {

                    targetControl = targetPossessControl;

                    PosessParticleFXStart();
                    Invoke("Possess",1.0f);
                }

                else
                {
                    Debug.Log("Target cannot be possessed!");
                    isPossessing = false;
                }
            }       

        }
    }

    void PosessParticleFXStart()//CinemachineBlendMonitor c)
    {
        //var newLaser = Instantiate(LaserPrefab, LaserPivot);
        //LaserPivot.transform.LookAt(targetPossess);
        //newLaser.Play();

        Laser.transform.LookAt(targetPossess);
        Laser.Play();
        //CinemachineBlendMonitor.OnCameraBlendStarted -= PosessParticleFXStart;
    }


    private void Possess() //Invoked from TryPossess()
    {
        CinemachineBlendMonitor.OnCameraBlendFinished += FinalizePossess;

        controlPlayer.enabled = false;
        targetVirtualCamera = targetPossess.GetComponentInChildren<CinemachineVirtualCamera>();
        //targetPossessionRaycast = targetPossess.GetComponent<PosessionRaycast>();
        
        
        if (targetVirtualCamera != null)
        {
            targetVirtualCamera.transform.rotation = currentVirtualCamera.transform.rotation;
            currentVirtualCamera.Priority = 1;
            targetVirtualCamera.Priority = 10;
        }
        
    }

    private void FinalizePossess(CinemachineBlendMonitor c)
    {
        CinemachineBlendMonitor.OnCameraBlendFinished -= FinalizePossess;
        //CinemachineBlendMonitor.OnCameraBlendFinished += PosessParticleFXStop;

        targetVirtualCamera.transform.rotation = targetVirtualCamera.transform.parent.rotation;

        targetControl.enabled = true;
        //targetPossessionRaycast.enabled = true;

        targetPossess = null;
        isPossessing = false;
        this.enabled = false;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using ECM.Controllers;

[RequireComponent(typeof(ModifiedECMController))]
public class PosessionRaycast : MonoBehaviour
{
    public bool CanPossess => canPossess;

    private Camera cam;
    private ModifiedECMController controlPlayer;
    private EnemyAI enemyAI;

    private CinemachineVirtualCamera currentVirtualCamera;

    private Transform targetPossess;
    private ModifiedECMController targetControl;
    private CinemachineVirtualCamera targetVirtualCamera;
    private EnemyAI targetEnemyAI;
    private PosessionRaycast targetPossessionRaycast;

    private bool isPossessingOnProgress = false;
    private bool canPossess = true;
    private bool isPlayerPossessing;

    public ParticleSystem Laser;

    [SerializeField] private float possessRange = 500f;
    [SerializeField] private LayerMask possessingRaycastLayer;

    private float possessionCooldownTime = 5f;

    private void Start()
    {
        cam = Camera.main;
        currentVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        controlPlayer ??= GetComponent<ModifiedECMController>();
        enemyAI ??= GetComponent<EnemyAI>();

        controlPlayer.OnPossessPressed += TryPossess;
        
        if(enemyAI != null)
            enemyAI.OnAITryPosses += TryPossess;
    }

    private void OnDisable()
    {
        controlPlayer.OnPossessPressed -= TryPossess;

        if (enemyAI != null)
            enemyAI.OnAITryPosses -= TryPossess;

    }

    private void AI_ProcessingPossession()
    {
        if (isPossessingOnProgress)
            return;

        targetPossess = enemyAI.CurrentPlayer;

        if (targetPossess == null)// || targetPossess.TryGetComponent(out FirstPersonCinemachine targetPossessControl))
        {
            isPossessingOnProgress = false;
            return;
        }
 
        targetControl = targetPossess.GetComponent<ModifiedECMController>();
        targetEnemyAI = targetControl.GetComponent<EnemyAI>();

        PossessParticleFXStart();
        AIPossessing();
    }

    private void PlayerProcessingPossession()
    {
        Ray ray;
        RaycastHit hit;
        ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));  //crosshair

        if (!Physics.Raycast(ray, out hit, possessRange, possessingRaycastLayer))
            return;

        if (isPossessingOnProgress)
            return;
        
        isPossessingOnProgress = true;
        targetPossess = hit.transform;
        targetControl = targetPossess.GetComponent<ModifiedECMController>();
        targetEnemyAI = targetControl.GetComponent<EnemyAI>();
        
        if (targetEnemyAI.CanSeePlayer)
            return;

        if (targetControl == null)
        {
            isPossessingOnProgress = false;
            return;
        }

        PossessParticleFXStart();
        Invoke("PlayerPossessing", 1.0f);   
        
    }

    public void TryPossess(bool playerPossessing) //TRUE = player | FALSE = AI
    {
        isPlayerPossessing = playerPossessing;

        if (!isPlayerPossessing) 
        {
            AI_ProcessingPossession();
            return;
        }

        PlayerProcessingPossession();
    }

    void PossessParticleFXStart()
    {
        Laser.transform.LookAt(targetPossess);
        Laser.Play();
    }

    private void PossessingCooldown()
    {
        canPossess = true;
    }

    private void AIPossessing()
    {
        canPossess = false;

        targetControl.OutOfBody();
        PostProcessCameraBlend.OnCameraBlendFinished += FinalizePossess;

        Invoke("PossessingCooldown", possessionCooldownTime);
    }

    private void PlayerPossessing() //Invoked from TryPossess()
    {
        PostProcessCameraBlend.OnCameraBlendFinished += FinalizePossess;

        controlPlayer.enabled = false;
        targetVirtualCamera = targetPossess.GetComponentInChildren<CinemachineVirtualCamera>();

        if (targetVirtualCamera == null)
            return;
        
        targetVirtualCamera.transform.rotation = currentVirtualCamera.transform.rotation;
        currentVirtualCamera.Priority = 1;
        targetVirtualCamera.Priority = 10;            
    }

    private void FinalizePossess(PostProcessCameraBlend c)
    {
        PostProcessCameraBlend.OnCameraBlendFinished -= FinalizePossess;

        var GhostBody = GetComponent<GhostBody>();

        if (isPlayerPossessing) 
        {
            targetVirtualCamera.transform.rotation = targetVirtualCamera.transform.parent.rotation;
            targetEnemyAI.enabled = false;
            targetControl.enabled = true;

            if (this.enemyAI != null)
            {
                this.enemyAI.enabled = true;
            }
        }

        else 
        {
            targetEnemyAI.enabled = true;
            targetControl.enabled = false;
        }

        CleaningTargetPossessComponents();
        if (GhostBody != null)
        {
            Destroy(this.gameObject);
        }
    }

    private void CleaningTargetPossessComponents()
    {
        isPossessingOnProgress = false;
        targetPossess = null;
        targetControl = null;
        targetVirtualCamera = null;
        targetEnemyAI = null;
    }
}

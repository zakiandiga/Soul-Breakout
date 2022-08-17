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
    //PROPERTIES
    public bool CanPossess => canPossess;


    private Camera cam; //need camera reference for ScreenPointToRay
    private FirstPersonCinemachine controlPlayer;
    private EnemyAI enemyAI;

    private CinemachineVirtualCamera currentVirtualCamera;

    private Transform targetPossess;
    private FirstPersonCinemachine targetControl;
    private CinemachineVirtualCamera targetVirtualCamera;
    private EnemyAI targetEnemyAI;
    private PosessionRaycast targetPossessionRaycast;

    private bool isPossessing = false;
    private bool canPossess = true;
    private bool possessMode;

    public ParticleSystem Laser;
    //public ParticleSystem LaserPrefab;
    //public Transform LaserPivot;

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

        //targetPossessionRaycast = null;

        controlPlayer ??= GetComponent<FirstPersonCinemachine>();
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

    public void TryPossess(bool playerPossessing) //TRUE = player | FALSE = AI
    {
        possessMode = playerPossessing;


        if (!possessMode) //AI is trying to possess
        {
            if(!isPossessing)
            {
                targetPossess = enemyAI.CurrentPlayer;
                if(targetPossess != null && targetPossess.TryGetComponent(out FirstPersonCinemachine targetPossessControl))
                {
                    targetControl = targetPossessControl;
                    targetEnemyAI = targetControl.GetComponent<EnemyAI>();

                    PossessParticleFXStart();
                    AIPossessing();
                }

                else
                {
                    Debug.Log("Target Cannot be possessed");
                    isPossessing = false;
                }
            }            
        }

        else //Player is trying to possess
        {
            Ray ray;
            RaycastHit hit;
            ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));  //crosshair
            Debug.DrawRay(ray.origin, ray.direction * 500, Color.red);

            if (Physics.Raycast(ray, out hit, possessRange, possessingRaycastLayer))
            {

                if (!isPossessing)
                {
                    targetPossess = hit.transform;
                    isPossessing = true;
                    if (targetPossess.TryGetComponent(out FirstPersonCinemachine targetPossessControl))
                    {
                        targetControl = targetPossessControl;
                        targetEnemyAI = targetControl.GetComponent<EnemyAI>();

                        if(targetEnemyAI.CanSeePlayer)
                        {
                            Debug.Log("CANNOT POSSESS, ENEMY CAN SEE PLAYER");
                        }
                        else
                        {
                            Debug.Log("PLAYER POSSESS!");
                            PossessParticleFXStart();
                            Invoke("PlayerPossessing", 1.0f);

                        }
                    }

                    else
                    {
                        Debug.Log("Target cannot be possessed!");
                        isPossessing = false;
                    }
                }
            }
        }
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
        CinemachineBlendMonitor.OnCameraBlendFinished += FinalizePossess;

        Invoke("PossessingCooldown", possessionCooldownTime);
    }

    private void PlayerPossessing() //Invoked from TryPossess()
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

        var GhostBody = GetComponent<GhostBody>();

        if (possessMode) //player possessing
        {
            targetVirtualCamera.transform.rotation = targetVirtualCamera.transform.parent.rotation;
            targetEnemyAI.enabled = false;
            targetControl.enabled = true;

            if (this.enemyAI != null)
            {
                this.enemyAI.enabled = true;
            }
        }

        else //AI possessing
        {
            targetEnemyAI.enabled = true;
            targetControl.enabled = false;
        }


        isPossessing = false;

        targetPossess = null;
        targetControl = null;
        targetVirtualCamera = null;
        targetEnemyAI = null;

        if (GhostBody != null)
        {
            Destroy(this.gameObject);
        }
        //this.enabled = false;
    }


}

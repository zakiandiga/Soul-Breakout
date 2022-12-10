using System;
using System.Collections;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;


public class EnemyAI : MonoBehaviour
{
    public float CurrentMoveSpeed => navMeshAgent.enabled ? navMeshAgent.velocity.magnitude :  pathfindingAI.desiredVelocity.magnitude;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public float NavMeshSpeed => navMeshAgent.enabled ? navMeshAgent.velocity.magnitude : 0;
    public bool CanSeePlayer => fieldOfViewAI.CanSeePlayer;
    public Transform CurrentPlayer => fieldOfViewAI.CurrentPlayer;

    private Rigidbody rigidBody;
    private FieldOfViewAI fieldOfViewAI; 
    private NavMeshAgent navMeshAgent;
    private AIPath pathfindingAI;
    private Animator animator; 
    public GameObject alert;

    [SerializeField] private float possessingDistance = 5f;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float readyPossessionTimer = 5f;
    [SerializeField] private float possessingCooldown = 5f;

    private bool isIdling = false;
    private bool readyToPossess = false;
    private bool decidingPossess = false; 
    private bool possessOnCooldown = false;

    public event Action<bool> OnAITryPosses;

    private AI_STATE AI_State = AI_STATE.IDLE;

    private IEnumerator idleTimer;
    private IEnumerator readyingPossess;

    public enum AI_STATE
    {
        INITIALIZE,
        IDLE,
        PATROL,
        CHASING,
        POSSESS,
    }
    
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        pathfindingAI = GetComponent<AIPath>();
       
        fieldOfViewAI = GetComponent<FieldOfViewAI>();

        animator = GetComponentInChildren<Animator>();

        
        navMeshAgent.autoBraking = false;
    }

    private void OnEnable()
    {
        if(fieldOfViewAI != null)
        {
            if (!fieldOfViewAI.enabled)
                fieldOfViewAI.enabled = true;
        }

        AI_State = AI_STATE.IDLE;
    }

    private void OnDisable()
    {
        DisablingAIComponent();
    }

    private void Update()
    {

        Animate();
        StateMachine();


        if (CanSeePlayer)
        {
            alert.SetActive(true);
        }
        else
        {
            alert.SetActive(false);
        }
    }

    private void DisablingAIComponent()
    {
        if (navMeshAgent.enabled)
            navMeshAgent.enabled = false;

        if (pathfindingAI.enabled)
            pathfindingAI.enabled = false;

        if (fieldOfViewAI.enabled)
            fieldOfViewAI.enabled = false;
    }

    private void StateMachine()
    {
        switch (AI_State)
        {
            case AI_STATE.IDLE:
                Idling();
                break;

            case AI_STATE.PATROL:
                Patroling();
                break;

            case AI_STATE.CHASING:                
                ChasePlayer();
                break;

            case AI_STATE.POSSESS:
                PossessDecision();
                break;
        }
    }


    private void Idling()
    {
        if (NavMeshAgent.enabled)
            NavMeshAgent.enabled = false;

        if (!pathfindingAI.enabled)
            pathfindingAI.enabled = true;

        if (!rigidBody.useGravity)
            rigidBody.useGravity = true;

        if (pathfindingAI.canSearch)
            pathfindingAI.canSearch = false;


        //EXIT to CHASING
        if (CurrentPlayer != null && CanSeePlayer)
        {
            //if the idleTime is on going, stop the coroutine
            if (idleTimer != null)
                StopCoroutine(idleTimer);

            //start the ready possession timer
            if (!readyToPossess)
            {
                readyingPossess = ReadyingPossession();
                StartCoroutine(readyingPossess);
            }

            //switch from A*Pathfinding to Unity NavMeshAgent
            pathfindingAI.enabled = false;
            rigidBody.useGravity = false;
            navMeshAgent.enabled = true;

            //Change the state
            AI_State = AI_STATE.CHASING;
        }

        //Exit to patrol /w A*
        if(!isIdling)
        {
            Debug.Log("exit to patrol");

            pathfindingAI.canSearch = true;

            AI_State = AI_STATE.PATROL;
        }
    }

    private IEnumerator IdleTimer()
    {        
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3f));

        Debug.Log("Idling time is out, now ready to move");
        isIdling = false;
        idleTimer = null;
    }

    private void Patroling()
    {
        //EXIT to IDLE
        if(pathfindingAI.reachedEndOfPath)
        {
            Debug.Log("AI reach the destination, now go to IDLE");
            pathfindingAI.canSearch = false;
            isIdling = true;
            idleTimer = IdleTimer();
            StartCoroutine(idleTimer);
            AI_State = AI_STATE.IDLE;
        }

        //EXIT to CHASING
        if (CurrentPlayer != null && CanSeePlayer)
        {
            //if the idleTime is on going, stop the coroutine
            if (idleTimer != null)
                StopCoroutine(idleTimer);

            //start the ready possession timer
            if (!readyToPossess)
            {
                readyingPossess = ReadyingPossession();
                StartCoroutine(readyingPossess);
            }

            //switch from A*Pathfinding to Unity NavMeshAgent
            pathfindingAI.enabled = false;
            rigidBody.useGravity = false;
            navMeshAgent.enabled = true;

            //Change the state
            AI_State = AI_STATE.CHASING;
        }
    }

    private void ChasePlayer()
    {
        if (navMeshAgent.speed != chaseSpeed)
            navMeshAgent.speed = chaseSpeed;

        if (CurrentPlayer != null)
            navMeshAgent.SetDestination(CurrentPlayer.position);


        //EXIT to IDLE
        if (CurrentPlayer == null || !CanSeePlayer)
        {
            if (readyingPossess != null)              
                StopCoroutine(readyingPossess);

            readyToPossess = false;

            navMeshAgent.ResetPath();
            isIdling = true;
            idleTimer = IdleTimer();

            navMeshAgent.enabled = false;
            pathfindingAI.enabled = true;
            rigidBody.useGravity = true;
            pathfindingAI.canSearch = false;

            StartCoroutine(idleTimer);
            AI_State = AI_STATE.IDLE;
        }

        //EXIT to POSSESS
        if (navMeshAgent.enabled && navMeshAgent.remainingDistance <= possessingDistance && readyToPossess)
        {
            if (readyingPossess != null)
                StopCoroutine(readyingPossess);

            readyToPossess = false;

            navMeshAgent.ResetPath();
            decidingPossess = true;
            AI_State = AI_STATE.POSSESS;
        }
    }

    private void PossessDecision()
    {
        if (decidingPossess)
        {
            bool tryPossessing = UnityEngine.Random.value >= 0.5f; //TRUE = will possess | FALSE = won't possess

            Debug.Log(tryPossessing);
            if (!tryPossessing || possessOnCooldown) //AI decide not to possess
            {
                Debug.Log("I don't want to possess");
                decidingPossess = false;
                ExitFromPossessing();
            }

            else //AI decide to possess
            {
                Debug.Log("I WILL possess the player!");
                decidingPossess = false;
                PossessExecution();
            }
        }
    }

    private void PossessExecution()
    {
        OnAITryPosses?.Invoke(false);
        possessOnCooldown = true;
        readyToPossess = false;
        StartCoroutine(PossessCooldownTimer());
    }

    private IEnumerator ReadyingPossession()
    {
        yield return new WaitForSeconds(readyPossessionTimer);
        Debug.Log("I'M READY TO POSSESS");
        readyToPossess = true;
        readyingPossess = null;
    }

    private IEnumerator PossessCooldownTimer()
    {
        yield return new WaitForSeconds(possessingCooldown);
        possessOnCooldown = false;
        ExitFromPossessing();
    }


    private void ExitFromPossessing()
    {
        if (CurrentPlayer != null && CanSeePlayer)
        {
            Debug.Log("Go back chasing again");

            if (!readyToPossess)
            {
                readyingPossess = ReadyingPossession();
                StartCoroutine(readyingPossess);
            }
            AI_State = AI_STATE.CHASING;
        }
        else
        {
            Debug.Log("Player is gone");
            possessOnCooldown = false;

            isIdling = true;
            idleTimer = IdleTimer();

            navMeshAgent.enabled = false;
            pathfindingAI.enabled = true;
            pathfindingAI.canSearch = false;

            StartCoroutine(idleTimer);
            AI_State = AI_STATE.IDLE;
        }
    }

    //Later we can use this if the obstacle and navmesh already set properly
    private void StopChasing()
    {
        navMeshAgent.SetDestination(fieldOfViewAI.LastPlayerPosition);
    }

    //Added this function for animation -Zak
    private void Animate()
    {        
        if (animator == null)
            return;

        
        if (CurrentMoveSpeed > 0.2f)
        {
            
            animator.SetFloat("MoveSpeed", CurrentMoveSpeed);
            return;
        }
       
        animator.SetFloat("MoveSpeed", 0);
        
    }
}

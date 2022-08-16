using System;
using System.Collections;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;


public class EnemyAI : MonoBehaviour
{
    //PROPERTIES
    //Added the property mainly for animation -Zak
    public float CurrentMoveSpeed => pathfindingAI.desiredVelocity.magnitude;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public float NavMeshSpeed => navMeshAgent.enabled ? navMeshAgent.velocity.magnitude : 0;
    public bool CanSeePlayer => fieldOfViewAI.CanSeePlayer;
    public Transform CurrentPlayer => fieldOfViewAI.CurrentPlayer;

    private Rigidbody rigidBody;
    //public AIManager aIManager;
    private FieldOfViewAI fieldOfViewAI; //Changed this to private
    private EnemyPatrol enemyPatrol;
    private NavMeshAgent navMeshAgent;
    private AIPath pathfindingAI;
    private Animator animator; //Added for animation -Zak
    public GameObject alert;

    [SerializeField] float chaseRange = 5f; //not used in new system (directly use NavMeshAgent StoppingDistance)

    [SerializeField] private float patrolDistanceTolerance = 2f; //previously minRemainingDistance
    [SerializeField] private float possessingDistance = 5f;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float readyPossessionTimer = 5f;
    [SerializeField] private float possessingCooldown = 5f;

    [HideInInspector] public Transform objectTransform;

    float distanceToTarget = Mathf.Infinity;
    private bool readyToPossess = false;
    private bool decidingPossess = false; //temp variable
    private bool possessOnCooldown = false;

    public event Action<bool> OnAITryPosses;

    private AI_STATE AI_State = AI_STATE.IDLE;

    public enum AI_STATE
    {
        IDLE,
        PATROL,
        CHASING,
        POSSESS,
    }

    #region MonoBehaviour Loops
    // Start is called before the first frame update
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        pathfindingAI = GetComponent<AIPath>();
        rigidBody = GetComponent<Rigidbody>();
        objectTransform = GetComponent<Transform>();
        fieldOfViewAI = GetComponent<FieldOfViewAI>();
        enemyPatrol = GetComponent<EnemyPatrol>();
        animator = GetComponentInChildren<Animator>();

        
        navMeshAgent.autoBraking = false;
    }

    private void OnEnable()
    {
        if (enemyPatrol != null && !enemyPatrol.enabled)
            enemyPatrol.enabled = true;

    }

    private void OnDisable()
    {
        enemyPatrol.enabled = false;
    }

    private void Update()
    {
        //Debug.Log(AI_State);
        //Added this line for animation -Zak
        if (navMeshAgent.enabled)
        {
            StateMachine();
        }

        Animate();


        if (CanSeePlayer)
        {
            alert.SetActive(true);
        }
        else
        {
            alert.SetActive(false);
        }
    }
    #endregion

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
                Debug.DrawRay(transform.position, transform.forward * possessingDistance, Color.cyan);
                ChasePlayer();
                break;

            case AI_STATE.POSSESS:
                PossessDecision();
                break;
        }
    }

    private void Idling()
    {
        //EXIT to Patrol
        if (enemyPatrol.TotalDestinationPoint > 0 && !enemyPatrol.OnDelay)
        {
            navMeshAgent.SetDestination(enemyPatrol.GetWayPoint());
            AI_State = AI_STATE.PATROL;
        }

        //EXIT to CHASING
        if (CurrentPlayer != null && CanSeePlayer)
        {
            if (!readyToPossess)
            {
                StartCoroutine(ReadyingPossession());
            }

            AI_State = AI_STATE.CHASING;
        }
    }

    private void Patroling()
    {
        if (navMeshAgent.speed != patrolSpeed)
            navMeshAgent.speed = patrolSpeed;

        //EXIT to IDLE
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            enemyPatrol.SetDelayBetweenPoints();
            AI_State = AI_STATE.IDLE;
        }

        //EXIT to CHASING
        if (CurrentPlayer != null && CanSeePlayer)
        {
            if(!readyToPossess)
            {
                StartCoroutine(ReadyingPossession());
            }

            AI_State = AI_STATE.CHASING;
        }
    }

    private void ChasePlayer()
    {
        if (navMeshAgent.speed != chaseSpeed)
            navMeshAgent.speed = chaseSpeed;

        if (CurrentPlayer != null)
            navMeshAgent.SetDestination(CurrentPlayer.position);

        //EXIT to PATROL
        if (CurrentPlayer == null || !CanSeePlayer)
        {
            navMeshAgent.ResetPath();
            AI_State = AI_STATE.PATROL;
        }

        //EXIT to POSSESS
        if (navMeshAgent.remainingDistance <= possessingDistance && readyToPossess)
        {
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
        Debug.Log("Readying possession!");
        yield return new WaitForSeconds(readyPossessionTimer);
        Debug.Log("I'M READY TO POSSESS");
        readyToPossess = true;
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
                StartCoroutine(ReadyingPossession());
            }
            AI_State = AI_STATE.CHASING;
        }
        else
        {
            Debug.Log("Player is gone");
            possessOnCooldown = false;
            AI_State = AI_STATE.IDLE;
        }
    }

    //Later we can use this if the obstacle and navmesh already set properly
    private void StopChasing()
    {
        navMeshAgent.SetDestination(fieldOfViewAI.LastPlayerPosition);
    }

    private void FollowTarget(Vector3 playerPosition)
    {
        distanceToTarget = Vector3.Distance(playerPosition, objectTransform.position);

        if (distanceToTarget <= chaseRange || fieldOfViewAI.CanSeePlayer == true)
        {
            navMeshAgent.SetDestination(playerPosition);

            navMeshAgent.speed = chaseSpeed;
        }
    }

    //Added this function for animation -Zak
    private void Animate()
    {        
        if (animator == null)
            return;

        
        if (CurrentMoveSpeed > 0.2f)
        {
            
            animator.SetFloat("MoveSpeed", CurrentMoveSpeed);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0);
        }
    }
}

using System;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    //PROPERTIES
    //Added the property mainly for animation -Zak
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public float NavMeshSpeed => navMeshAgent.enabled ? navMeshAgent.velocity.magnitude : 0;
    public bool CanSeePlayer => fieldOfViewAI.CanSeePlayer;

    //public AIManager aIManager;
    private FieldOfViewAI fieldOfViewAI; //Changed this to private
    private EnemyPatrol enemyPatrol;
    private NavMeshAgent navMeshAgent;
    private Animator animator; //Added for animation -Zak

    [SerializeField] float chaseRange = 5f;

    [SerializeField] private float patrolDistanceTolerance = 2f; //previously minRemainingDistance

    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [HideInInspector] public Transform objectTransform;
    
    float distanceToTarget = Mathf.Infinity;

    private bool isPossessing = false; //temp variable
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
        objectTransform = GetComponent<Transform>();
        fieldOfViewAI = GetComponent<FieldOfViewAI>();
        enemyPatrol = GetComponent<EnemyPatrol>();
        animator = GetComponentInChildren<Animator>();

        navMeshAgent.autoBraking = false;
    }

    private void OnEnable()
    {
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
            Animate();
            StateMachine();
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
                ChasePlayer();                
                break;

            case AI_STATE.POSSESS:

                break;
        }
    }

    private void Idling()
    {
        //EXIT to Patrol
        if (enemyPatrol.TotalDestinationPoint > 0 && !enemyPatrol.OnDelay)
        {
            navMeshAgent.SetDestination(enemyPatrol.GetWayPoint());
            //navMeshAgent.destination = enemyPatrol.GetWayPoint();
            AI_State = AI_STATE.PATROL;
        }

        //EXIT to CHASING
        if (fieldOfViewAI.CurrentPlayer != null && fieldOfViewAI.CanSeePlayer)
        {
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
        if (fieldOfViewAI.CurrentPlayer != null && fieldOfViewAI.CanSeePlayer)
        //Removed chaseRange from the condition
        {
            AI_State = AI_STATE.CHASING;
        }
    }

    private void ChasePlayer()
    {
        if (navMeshAgent.speed != chaseSpeed)
            navMeshAgent.speed = chaseSpeed;

        navMeshAgent.SetDestination(fieldOfViewAI.CurrentPlayer.position);
        //EXIT to PATROL
        if (fieldOfViewAI.CurrentPlayer == null || !fieldOfViewAI.CanSeePlayer)
        {
            navMeshAgent.ResetPath();
            AI_State = AI_STATE.PATROL;
        }

        //EXIT to POSSESS
        if (isPossessing)
        {
            AI_State = AI_STATE.POSSESS;
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

            //Debug.Log("navmeshAGENT DESTINATION " + navMeshAgent.destination);

            navMeshAgent.speed = chaseSpeed;
        }

    }


    //Added this function for animation -Zak
    private void Animate()
    {
        if (animator == null)
            return;

        if (NavMeshSpeed > 0.2f)
        {
            animator.SetFloat("MoveSpeed", NavMeshSpeed);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0);
        }
    }

    /*
    // Update is called once per frame
    void LateUpdate()
    {
        if(GetComponent<FirstPersonCinemachine>().enabled==true)     //if the chaaracter is the player
        {
            GetComponent<NavMeshAgent>().enabled=false;
            Vector3 dumpLoc = aIManager.WhereIsPlayer(objectTransform.position);            
        }
        else                                                        //if the chaaracter is the enemy
        {
            GetComponent<NavMeshAgent>().enabled=true;
            Vector3 playerPosition  = aIManager.WhereIsPlayer(new Vector3 (0f,0f,0f));
            FollowTarget(playerPosition); 
        }


        //Debug.Log("distance to target: " + distanceToTarget);           
    }
    */


}

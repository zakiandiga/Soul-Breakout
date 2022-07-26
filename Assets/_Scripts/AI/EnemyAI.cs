using System;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    //Added the property mainly for animation -Zak
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public float NavMeshSpeed => navMeshAgent.enabled ? navMeshAgent.velocity.magnitude : 0;

    public AIManager aIManager;
    private FieldOfViewAI fieldOfViewAI; //Changed this to private

    NavMeshAgent navMeshAgent;
    private Animator animator; //Added for animation -Zak

    [SerializeField] float chaseRange = 5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [HideInInspector] public Transform objectTransform;
    
    float distanceToTarget = Mathf.Infinity;

    private bool isChasing = false;

    public event Action<bool> OnAITryPosses;


    // Start is called before the first frame update
    void Start()
    {
  
        navMeshAgent = GetComponent<NavMeshAgent>();

        objectTransform = GetComponent<Transform>();

        fieldOfViewAI = GetComponent<FieldOfViewAI>();

        animator = GetComponentInChildren<Animator>();

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

    private void ChasePlayer()
    {
        navMeshAgent.speed = chaseSpeed;

        navMeshAgent.SetDestination(fieldOfViewAI.CurrentPlayer.position);
    }

    //Later we can use this if the obstacle and navmesh already set properly
    private void StopChasing()
    {
        navMeshAgent.SetDestination(fieldOfViewAI.LastPlayerPosition);
    }

    private void FollowTarget(Vector3 playerPosition)
    {       
        distanceToTarget = Vector3.Distance(playerPosition, objectTransform.position );

        if(distanceToTarget <=chaseRange || fieldOfViewAI.CanSeePlayer == true)
        {
            navMeshAgent.SetDestination(playerPosition);

            //Debug.Log("navmeshAGENT DESTINATION " + navMeshAgent.destination);

            navMeshAgent.speed = chaseSpeed;
        }
         
    }

    private void Update()
    {
        //Added this line for animation -Zak
        if(navMeshAgent.enabled)
        {
            Animate();

            //if there is player on sight
            if (fieldOfViewAI.CurrentPlayer != null && 
                (fieldOfViewAI.CanSeePlayer || fieldOfViewAI.CurrentDistance <= chaseRange))
            {
                if (!isChasing)
                    isChasing = true;

                ChasePlayer();
            }

            //else if player is out of sight
            else
            {
                if(isChasing) //check if isChasing is true (to make sure we only check this once on exit chasing)
                {
                    isChasing = false;
                    navMeshAgent.ResetPath();
                    //StopChasing();

                }
            }
        }
    }

    //Added this function for animation -Zak
    private void Animate()
    {
        if (animator == null)
            return;

        if(NavMeshSpeed > 0.2f)
        {
            if(!animator.GetBool("IsRunning"))
            {
                animator.SetBool("IsRunning", true);
            }
        }
        else
        {
            if (animator.GetBool("IsRunning"))
            {
                animator.SetBool("IsRunning", false);
            }
        }

    }
}

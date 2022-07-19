using System;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    //Added the property mainly for animation -Zak
    public float NavMeshSpeed => navMeshAgent.enabled ? navMeshAgent.velocity.magnitude : 0;

    public AIManager aIManager;
    public FieldOfViewAI fieldOfViewAI;

    NavMeshAgent navMeshAgent;
    private Animator animator; //Added for animation -Zak

    [SerializeField] float chaseRange = 5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [HideInInspector] public Transform objectTransform;
    
    float distanceToTarget = Mathf.Infinity;

    public event Action<bool> OnAITryPosses;

    // Start is called before the first frame update
    void Start()
    {
  
        navMeshAgent = GetComponent<NavMeshAgent>();

        objectTransform = GetComponent<Transform>();

        animator = GetComponentInChildren<Animator>();

    }

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

    public void FollowTarget(Vector3 playerPosition)
    {       
        distanceToTarget = Vector3.Distance(playerPosition, objectTransform.position );

        if(distanceToTarget <=chaseRange || fieldOfViewAI.canSeePlayer == true)
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
            Animate();
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

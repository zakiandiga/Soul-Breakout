using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public AIManager aIManager;
    public FieldOfViewAI fieldOfViewAI;

    NavMeshAgent navMeshAgent;

    [SerializeField] float chaseRange = 5f;
    [SerializeField] private float chaseSpeed = 3.5f;

    public Transform objectTransform;
    
    float distanceToTarget = Mathf.Infinity;
    

    // Start is called before the first frame update
    void Start()
    {
  
        navMeshAgent = GetComponent<NavMeshAgent>();

        objectTransform = GetComponent<Transform>();


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

        if(distanceToTarget<=chaseRange || fieldOfViewAI.canSeePlayer == true)
        {
            navMeshAgent.SetDestination(playerPosition);

            //Debug.Log("navmeshAGENT DESTINATION " + navMeshAgent.destination);

            navMeshAgent.speed = chaseSpeed;
        }
         
    }
}

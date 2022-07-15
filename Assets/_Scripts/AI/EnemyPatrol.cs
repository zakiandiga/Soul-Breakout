using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private List<Transform> points;
    [SerializeField] private float minRemainingDistance = 0.5f;
    [SerializeField] private float patrollingSpeed = 1.5f;
    private int destinationPoint = 0;
    private NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        GoToNextPoint();      
        
    }

    void GoToNextPoint()
    {

        if(points.Count==0)
        {
            Debug.LogError("need atleast one destination point");
            enabled = false;
            return;
        }

        agent.destination = points[destinationPoint].position;

       // Debug.Log("navmeshAGENT DESTINATION " + agent.destination);



        destinationPoint = (destinationPoint+1) % points.Count;  //looping through iterator

    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = patrollingSpeed;

        if(!agent.pathPending && agent.remainingDistance < minRemainingDistance)
        {
             Debug.Log("to next point called");
            GoToNextPoint();           
        }

       // Debug.Log("PAth status: " + agent.pathStatus);


       // Debug.Log("agent destinaation: " + agent.destination);

        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public bool OnDelay => onDelay;
    public int TotalDestinationPoint => totalDestinationPoint;

    [SerializeField] private List<Transform> wayPoints;
    [SerializeField] private float minRemainingDistance = 0.5f;
    [SerializeField] private float patrollingSpeed = 1.5f;
    private int totalDestinationPoint;
    private int currentDestinationPoint = 0;
    //private NavMeshAgent agent;

    private float idleTime;
    private bool onDelay = false;

    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        //agent.autoBraking = false;

        //GoToNextPoint();      

        totalDestinationPoint = wayPoints.Count;
    }

    void GoToNextPoint()
    {

        if(wayPoints.Count==0)
        {
            Debug.LogError("need atleast one destination point");
            enabled = false;
            return;
        }

        //agent.destination = wayPoints[totalDestinationPoint].position;

       // Debug.Log("navmeshAGENT DESTINATION " + agent.destination);



        totalDestinationPoint = (totalDestinationPoint+1) % wayPoints.Count;  //looping through iterator

    }

    public Vector3 GetWayPoint()
    {
        if(wayPoints.Count <= 0)
        {
            Debug.LogError("No way point set");
            return transform.position;
        }
        

        if (currentDestinationPoint == totalDestinationPoint)
            currentDestinationPoint = 0;

        Debug.Log("current destination point: " + currentDestinationPoint);

        return wayPoints[currentDestinationPoint].position;
    }

    public void SetDelayBetweenPoints()
    {
        if(!onDelay)
        {
            onDelay = true;
            currentDestinationPoint += 1;
            StartCoroutine(WaitBetweenPoints());
        }
    }    

    private IEnumerator WaitBetweenPoints()
    {
        idleTime = Random.Range(0.5f, 3f);

        yield return new WaitForSeconds(idleTime);
        onDelay = false;
    }

}

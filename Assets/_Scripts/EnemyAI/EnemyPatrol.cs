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

    private float idleTime;
    private bool onDelay = false;

    void Start()
    {
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

        totalDestinationPoint = (totalDestinationPoint+1) % wayPoints.Count; 
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
        idleTime = Random.Range(0.2f, 0.5f);

        yield return new WaitForSeconds(idleTime);
        onDelay = false;
    }

}

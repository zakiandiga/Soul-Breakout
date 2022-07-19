using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM.Controllers;

public class FieldOfViewAI : MonoBehaviour
{
    public float radius;
    [Range(0,360)] public float angle;

    private Transform target;

    [HideInInspector] public GameObject[] playerRefs;
    [HideInInspector] public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

    // Start is called before the first frame update
    void Start()
    {
       playerRefs = GameObject.FindGameObjectsWithTag("character");
       for(int i=0;i<playerRefs.Length;i++)
       {
       // Debug.Log("object name" + playerRefs[i].name);

        if(playerRefs[i].GetComponent<FirstPersonCinemachine>().enabled==true)
        {
            playerRef = playerRefs[i];
        }

       }


       StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()                        //Calls this routine every 0.2s
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while(true)
        {
            yield return wait;
            FieldOfViewCheck();
        }

    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position,radius, targetMask);    //Returns an array with all colliders touching or inside the sphere.

        if(rangeChecks.Length!=0)                                                                 //Checking if we found some target collider on the targetMask
        {
            for(int i=0; i<rangeChecks.Length; i++)
            {
                if(rangeChecks[i].GetComponent<FirstPersonCinemachine>().enabled==true)     //if the character with FPC enabled (aka the player)
                {
                    target = rangeChecks[i].transform;
                    Vector3 directionToTarget = (target.position - transform.position).normalized;

                    if(Vector3.Angle(transform.forward,directionToTarget) < angle/2)     //if angle btwn (normal & target) < Viewing_angle/2 (=> player is in the FOV)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position,target.position);

                        if(!Physics.Raycast(transform.position,directionToTarget,distanceToTarget, obstructionMask))   //if there is not an obstacle btwn player & enemy
                            canSeePlayer=true;
                        else
                            canSeePlayer = false;

                    }
                    else
                        canSeePlayer = false;


                }
            }

        }
        else if(canSeePlayer)       //treverting to the original bool state of canSeePlayer 
            canSeePlayer=false;
        
        if(Vector3.Distance(target.position,transform.position)>radius)
            canSeePlayer=false;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

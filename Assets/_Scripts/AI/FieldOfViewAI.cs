using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ECM.Controllers;

public class FieldOfViewAI : MonoBehaviour
{
    public bool CanSeePlayer => canSeePlayer;
    public Transform CurrentPlayer => target;
    public Vector3 LastPlayerPosition => lastPlayerPosition;
    public float CurrentDistance => currentDistance;

    public float radius;
    [Range(0,360)] public float angle;

    private Transform target;
    private FirstPersonCinemachine targetControl;

    [HideInInspector] public GameObject[] playerRefs;
    [HideInInspector] public GameObject playerRef;

    private List<FirstPersonCinemachine> potentialTargets = new List<FirstPersonCinemachine>();
    private bool playerInRange = false;
    private float currentDistance;
    private Vector3 lastPlayerPosition; //the point where the player leave the field of view

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    private bool canSeePlayer; //I Change this to private and create a CanSeePlayer property that return the value of this


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

    private void OnDisable()
    {
        StopCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()                        //Calls this routine every 0.2s
    {
        //FieldOfViewCheck();
        Debug.Log("FOVRoutine, playerInRange = " + playerInRange);

        

        WaitForSeconds wait = new WaitForSeconds(0.3f);

        while (true)
        {
            yield return wait;

            if (playerInRange)
                VisionCheck();

            else
                CheckPlayer();
        }
    }

    private void VisionCheck()
    {
        currentDistance = Vector3.Distance(transform.position, target.position);

        if(currentDistance > radius || !targetControl.enabled)
        {
            lastPlayerPosition = target.position;

            target = null;
            playerInRange = false;

            if (canSeePlayer)
            {
                canSeePlayer = false;
                Debug.Log(gameObject.name + ": I CAN'T see player!");
            }
        }

        else if(currentDistance <= radius && targetControl.PlayerNoticable)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)     //if angle btwn (normal & target) < Viewing_angle/2 (=> player is in the FOV)
            {           

                if (!Physics.Raycast(transform.position + (Vector3.up * 1.59f), directionToTarget, currentDistance, obstructionMask))   //if there is no obstacle btwn player & enemy
                {
                    if(!canSeePlayer)
                    {
                        canSeePlayer = true;
                        Debug.Log(gameObject.name + ": I CAN SEE THE PLAYER!");
                    }
                }
            }
        }
        
    }

    private void CheckPlayer()
    {
        //Debug.Log("Player Check");
        Collider[] collidedCharacters = Physics.OverlapSphere(transform.position, radius, targetMask);

        //Debug.Log("Colliding characters: " + collidedCharacters.Length);
        if(collidedCharacters.Length <= 0)
        {
            if (target != null)
            {
                targetControl = null;
                target = null;
            }

            if(playerInRange != false)
                playerInRange = false;

            return;
        }    

        else if(collidedCharacters.Length > 0)
        {
            for (int i = 0; i < collidedCharacters.Length; ++i)
            {
                potentialTargets.Add(collidedCharacters[i].GetComponent<FirstPersonCinemachine>());
            }

            if (potentialTargets.Count <= 0)
                return;


            else if (potentialTargets.Count > 0)
            {
                foreach (FirstPersonCinemachine targetToChoose in potentialTargets)
                {
                    if (targetToChoose.enabled && (target != targetToChoose.transform || target == null))
                    {
                        target = targetToChoose.transform;
                        targetControl = target.GetComponent<FirstPersonCinemachine>();
                    }
          

                }

                if (target != null)
                {
                    playerInRange = true;
                    Debug.Log(gameObject.name + ": PLAYER IS IN RANGE");
                }
            }
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
                    Debug.Log("player is" + rangeChecks[i].name);
                    target = rangeChecks[i].transform;


                    Vector3 directionToTarget = (target.position - transform.position).normalized;

                    if(Vector3.Angle(transform.forward,directionToTarget) < angle/2)     //if angle btwn (normal & target) < Viewing_angle/2 (=> player is in the FOV)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position,target.position);

                        if(!Physics.Raycast(transform.position,directionToTarget,distanceToTarget, obstructionMask))   //if there is not an obstacle btwn player & enemy
                        {
                            Debug.Log("player is Oobeing watchedOOO" + rangeChecks[i].name);
                            canSeePlayer=true;

                            if(Vector3.Distance(target.position,transform.position)>radius || target.position==null)
                            canSeePlayer=false;

                        }
                            
                        else
                            canSeePlayer = false;

                    }
                    else
                        canSeePlayer = false;


                }
                break;
        
            }

        }
        else if(canSeePlayer)       //treverting to the original bool state of canSeePlayer 
            canSeePlayer=false;

        //Debug.Log(target.position);
        /**/


    }
}

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ECM.Controllers;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] List<GameObject> targets;

   // GameObject[] targets;
    NavMeshAgent navMeshAgent;
    //CinemachineVirtualCamera vcam;

    //public FirstPersonCinemachine camScript;
    

    // Start is called before the first frame update
    void Start()
    {
        /*if(targets==null)
            targets = GameObject.FindGameObjectsWithTag("character");*/

        navMeshAgent = GetComponent<NavMeshAgent>();

        //camScript = GetComponent<FirstPersonCinemachine>();


    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();  
    }

    public void FindTarget()
    {
        for(int i=0; i<targets.Count; i++)
        {
           if(targets[i].GetComponent<FirstPersonCinemachine>().enabled==true)
                navMeshAgent.SetDestination(targets[i].transform.position);
        }    

    }
}

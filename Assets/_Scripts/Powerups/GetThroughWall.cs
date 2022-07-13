using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetThroughWall : MonoBehaviour
{
    [SerializeField] Collider thisCollider, specialWall;
    

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(Physics.GetIgnoreCollision(thisCollider, specialWall));
    }

    private void Update()
    {
        //Try to avoid running physics in update if possible
        Physics.IgnoreCollision(thisCollider, specialWall, true);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetThroughWall : MonoBehaviour
{
    [SerializeField] Collider thisCollider, specialWall;

    private void Start()
    {
        Debug.Log(Physics.GetIgnoreCollision(thisCollider, specialWall));
    }

    private void Update()
    {

        Physics.IgnoreCollision(thisCollider, specialWall, true);
        
    }
}

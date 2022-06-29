using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignLookAt : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        gameObject.SetActive(false);
    }
    void Update()
    {
        transform.LookAt(cam.transform);
        transform.rotation = cam.transform.rotation;
    }
}

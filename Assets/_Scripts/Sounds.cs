using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    private AudioSource playerAudio;
    public AudioClip footStep;

    // Start is called before the first frame update
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        
    }



    public void PlayFootstep()
    {
        playerAudio.PlayOneShot(footStep);
        Debug.Log("Play footstep!");


    }
}

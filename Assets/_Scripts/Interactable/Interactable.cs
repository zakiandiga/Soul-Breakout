using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM.Controllers;

public class Interactable : MonoBehaviour
{
    protected FirstPersonCinemachine currentInteractingPlayer;
    protected FirstPersonCinemachine nullPlayer;
    [SerializeField] protected GameObject sign;

    protected bool canInteract = false;

    protected virtual void StartInteract()
    {
        Debug.Log(gameObject.name + " START interact with: " + currentInteractingPlayer.name);
    }

    protected virtual void StopInteract()
    {
        Debug.Log(gameObject.name + " STOP interact with: " + currentInteractingPlayer.name);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {        
        if (currentInteractingPlayer == null)
        {
            if (other.GetComponent<FirstPersonCinemachine>() != null)
            {
                if (other.GetComponent<FirstPersonCinemachine>().enabled)
                {
                    currentInteractingPlayer = other.GetComponent<FirstPersonCinemachine>();
                    
                    if (currentInteractingPlayer.enabled && !canInteract)
                    {
                        canInteract = true;
                        if (sign != null)
                        {
                            sign.SetActive(true);
                        }
                    }
                }
            }
        }

        else
            return;
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (currentInteractingPlayer == null)
        {
            if (other.GetComponent<FirstPersonCinemachine>() != null)
            {
                if (other.GetComponent<FirstPersonCinemachine>().enabled)
                {
                    currentInteractingPlayer = other.GetComponent<FirstPersonCinemachine>();
                    
                    if (currentInteractingPlayer.enabled && !canInteract)
                    {
                        canInteract = true;
                        if (sign != null)
                        {
                            sign.SetActive(true);
                        }

                    }
                }
            }
            
            
        }

        else
            return;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if(currentInteractingPlayer == other.GetComponent<FirstPersonCinemachine>())
        {
            if (currentInteractingPlayer != null)
            {
                currentInteractingPlayer = null;
                canInteract = false;
                if (sign != null)
                {
                    sign.SetActive(false);
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (canInteract)
        {
            if (currentInteractingPlayer.IsInteracting)
                StartInteract();
            else
                StopInteract();

            if(!currentInteractingPlayer.enabled)
            {
                currentInteractingPlayer = nullPlayer;
                canInteract = false;
                if (sign != null)
                {
                    sign.SetActive(false);
                }
            }
        }
    }


}

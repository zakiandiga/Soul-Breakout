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

    protected void OnTriggerEnter(Collider other)
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

    protected void OnTriggerStay(Collider other)
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

    protected void OnTriggerExit(Collider other)
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
            if(currentInteractingPlayer.IsInteracting)
                Interact();

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

    protected virtual void Interact()
    {
        Debug.Log(gameObject.name + " interact with: " + currentInteractingPlayer.name);
    }
}

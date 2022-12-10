using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM.Controllers;

public class Interactable : MonoBehaviour
{
    protected ModifiedECMController currentInteractingPlayer;
    protected ModifiedECMController nullPlayer;
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
        currentInteractingPlayer = other.GetComponent<ModifiedECMController>();

        if (currentInteractingPlayer == null || !currentInteractingPlayer.enabled)
            return;

        SetReadyToInteract();
    }

    private void SetReadyToInteract()
    {
        if (canInteract)
            return;

        canInteract = true;
        if (sign != null)
        {
            sign.SetActive(true);
        }
    }



    protected virtual void OnTriggerStay(Collider other)
    {
        currentInteractingPlayer = other.GetComponent<ModifiedECMController>();

        if (currentInteractingPlayer == null || !currentInteractingPlayer.enabled)
            return;

        SetReadyToInteract();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (currentInteractingPlayer != other.GetComponent<ModifiedECMController>() || currentInteractingPlayer == null)
            return;

        SetCannotInteract();   
    }

    private void SetCannotInteract()
    {
        currentInteractingPlayer = null;
        canInteract = false;
        if (sign != null)
        {
            sign.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (!canInteract)
            return;

        if (!currentInteractingPlayer.enabled)
        {
            SetCannotInteract();
            return;
        }

        if(!currentInteractingPlayer.IsInteracting)
            StopInteract();            

        else
            StartInteract();
    }
}

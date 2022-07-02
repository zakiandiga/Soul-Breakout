using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskFill : Interactable
{
    public float CurrentProgress => currentProgress;
    public int MaxProgress => (int)maxProgress;
    public bool IsProgressing => isProgressing;
    public bool TaskFinished => taskFinished;

    [SerializeField] private int characterCode = 1;

    [SerializeField] private float maxProgress = 10f;
    private float currentProgress = 0f;

    private bool isProgressing = false;
    private bool taskFinished = false;

    protected override void StartInteract()
    {

        if (!isProgressing)
        {
            if (currentInteractingPlayer.CharacterCode == characterCode)
                isProgressing = true;
        }
    }

    protected override void StopInteract()
    {
        if (isProgressing)
        {
            if(!taskFinished)
            {
                Debug.Log("Task failed");
            }
            isProgressing = false;
            currentProgress = 0;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (taskFinished)
            return;

        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (taskFinished)
            return;

        base.OnTriggerStay(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (taskFinished)
            return;

        base.OnTriggerExit(other);
    }

    protected override void Update()
    {
        base.Update();

        if(!taskFinished && isProgressing && currentProgress < maxProgress)
        {
            currentProgress += Time.deltaTime;

            Debug.Log((int)currentProgress + "/" + maxProgress);
        }

        if (!taskFinished && currentProgress >= maxProgress)
        {
            Debug.Log("Task Done");
            taskFinished = true;
            isProgressing = false;
        }
    }
}

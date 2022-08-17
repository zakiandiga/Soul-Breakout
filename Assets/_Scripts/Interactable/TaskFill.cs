using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ECM.Controllers;

public class TaskFill : Interactable
{
    public float CurrentProgress => currentProgress;
    public int MaxProgress => (int)maxProgress;
    public bool IsProgressing => isProgressing;
    public bool TaskFinished => taskFinished;

    [SerializeField] private int blockCharacterCode = 1;

    [SerializeField] private float maxProgress = 10f;

    [SerializeField] private GameObject sprite;
    private Material currentMaterial;
    [SerializeField] private Material finishedMaterial;
    [SerializeField] private Material glowMaterial;
    [SerializeField] private Material normalMaterial;

    private float currentProgress = 0f;

    private bool isProgressing = false;
    private bool taskFinished = false;

    private TextMeshProUGUI signText;

   /* private void Awake() {
        FirstPersonCinemachine.OnPlayerNewBody += UpdateMaterial;
    }*/

    private void Start()
    {
            
        currentMaterial = normalMaterial;
        sprite.GetComponent<MeshRenderer>().material = currentMaterial;
        UpdateMaterial(1);
        signText = sign.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(currentMaterial);
    }

    protected override void StartInteract()
    {

        if (!isProgressing)
        {
            if (currentInteractingPlayer.CharacterCode == blockCharacterCode)
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

            //Debug.Log((int)currentProgress + "/" + maxProgress);
        }

        if (!taskFinished && currentProgress >= maxProgress)
        {
            Debug.Log("Task Done");
            taskFinished = true;
            signText.text = "TASK ALREADY COMPLETED";
            currentMaterial = finishedMaterial;
            sprite.GetComponent<MeshRenderer>().material = currentMaterial;
            Debug.Log(currentMaterial);
            isProgressing = false;
        }
    }

    private void UpdateMaterial(int characterCode)
    {
        Debug.Log("character code: " + characterCode);

        if(blockCharacterCode == characterCode && !taskFinished)
        {
            currentMaterial = glowMaterial;
            sprite.GetComponent<MeshRenderer>().material = currentMaterial;
        }
        else if(blockCharacterCode != characterCode && !taskFinished)
        {
            currentMaterial = normalMaterial;
            sprite.GetComponent<MeshRenderer>().material = currentMaterial;
        }
    }


private void OnEnable()
    {
        FirstPersonCinemachine.OnPlayerNewBody += UpdateMaterial;
        
    }
    private void OnDisable()
    {
        FirstPersonCinemachine.OnPlayerNewBody -= UpdateMaterial;
    }
}

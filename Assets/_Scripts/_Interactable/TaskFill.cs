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

    public ParticleSystem ghostEscape;

    [SerializeField] public int blockCharacterCode;

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
        UpdateMaterial(0);
        signText = sign.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void StartInteract()
    {
        if (isProgressing)
            return;

        if (currentInteractingPlayer.CharacterCode == blockCharacterCode)
            isProgressing = true;
    }

    protected override void StopInteract()
    {
        if (!isProgressing)
            return;

        isProgressing = false;
        currentProgress = 0;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (taskFinished)
            return;

        if (other.GetComponent<ModifiedECMController>().CharacterCode != blockCharacterCode)
            return;

        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (taskFinished)
            return;

        if (other.GetComponent<ModifiedECMController>().CharacterCode != blockCharacterCode)
            return;

        base.OnTriggerStay(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (taskFinished)
            return;

        if (other.GetComponent<ModifiedECMController>().CharacterCode != blockCharacterCode)
            return;

        base.OnTriggerExit(other);
    }

    private void ProgressingTask()
    {
        if (taskFinished || !isProgressing)
            return;

        currentProgress += Time.deltaTime;

        if (currentProgress <= MaxProgress)
            return;

        FinishingTask();
    }

    private void FinishingTask()
    {
        taskFinished = true;
        signText.text = "Soul Saved";
        //ghostEscape.Play();
        Instantiate(ghostEscape, transform.position + new Vector3(0.0f, 0.3f, 0.0f), Quaternion.identity);
        currentMaterial = finishedMaterial;
        sprite.GetComponent<MeshRenderer>().material = currentMaterial;
        isProgressing = false;
        Destroy(this.gameObject);
    }

    protected override void Update()
    {
        base.Update();

        ProgressingTask();
    }

    private void UpdateMaterial(int characterCode)
    {
        if (taskFinished)
            return;

        currentMaterial = blockCharacterCode == characterCode ? glowMaterial : normalMaterial;
        sprite.GetComponent<MeshRenderer>().material = currentMaterial;
    }


private void OnEnable()
    {
        ModifiedECMController.OnPlayerNewBody += UpdateMaterial;
        
    }
    private void OnDisable()
    {
        ModifiedECMController.OnPlayerNewBody -= UpdateMaterial;
    }
}

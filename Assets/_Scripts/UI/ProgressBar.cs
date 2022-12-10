using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    private TaskFill taskFill;
    private Slider slider;
    private TextMeshProUGUI text;

    private void Start()
    {
        taskFill = GetComponentInParent<TaskFill>();
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        slider.maxValue = taskFill.MaxProgress;
    }

    private void Update()
    {
        if (!taskFill.IsProgressing)
        {
            HideTaskWidget();
            return;
        }

        ShowTaskWidget();

        if (taskFill.TaskFinished)
        {
            text.text = "COMPLETED!";
            return;
        }
            
        var progInt = (int)taskFill.CurrentProgress;
        text.text = progInt.ToString() + " / " + taskFill.MaxProgress.ToString();
        slider.value = taskFill.CurrentProgress;
    }

    private void ShowTaskWidget()
    {
        if (text.gameObject.activeSelf && slider.gameObject.activeSelf)
            return;

        text.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
    }

    private void HideTaskWidget()
    {
        if (!text.gameObject.activeSelf && !slider.gameObject.activeSelf)
            return;

        text.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
    }

}

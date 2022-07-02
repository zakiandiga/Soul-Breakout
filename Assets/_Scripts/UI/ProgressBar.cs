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

        if(taskFill.IsProgressing)
        {
            if(!text.gameObject.activeSelf)
                text.gameObject.SetActive(true);

            if (!slider.gameObject.activeSelf)
                slider.gameObject.SetActive(true);

            if (!taskFill.TaskFinished)
            {
                var progInt = (int)taskFill.CurrentProgress;
                text.text = progInt.ToString() + " / " + taskFill.MaxProgress.ToString();
                slider.value = taskFill.CurrentProgress;
            }

            else
            {
                text.text = "COMPLETED";
            }
        }

        else if(!taskFill.IsProgressing)
        {
            if (text.gameObject.activeSelf)
                text.gameObject.SetActive(false);

            if (slider.gameObject.activeSelf)
                slider.gameObject.SetActive(false);
        }


    }

}

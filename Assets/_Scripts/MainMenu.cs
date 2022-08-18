using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject creditPanel;



    // Start is called before the first frame update
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);

    }

    public void OpenCreditMenu()
    {
        creditPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void OpenMainMenu()
    {
        mainPanel.SetActive(true);
        creditPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!!!");
        Application.Quit();
    }
}

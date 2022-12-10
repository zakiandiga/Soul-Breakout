using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string titleSceneName;
    public void StartGame()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject controlUi;
    
    public void PlayGame ()
    {
        SceneManager.LoadScene(2);
    }
    public void Controls()
    {
        controlUi.SetActive(true);
    }
    public void ControlsBack()
    {
        controlUi.SetActive(false);
        
    }
    public void QuitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

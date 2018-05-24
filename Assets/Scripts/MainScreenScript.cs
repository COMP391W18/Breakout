using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScreenScript : MonoBehaviour {
    
    public void LoadLevel()
    {
        SceneManager.LoadScene("Breakout");
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverMenu;

    private void OnEnable()
    {
        Mind.GameOver += EnableGameOverMenu; //subscribing to the event that gets called on game over.
        //this line means that when we call the Mind.GameOver event, EnableGameOverMenu gets called
    }

    private void OnDisable()
    {
        Mind.GameOver -= EnableGameOverMenu;
    }

    public void EnableGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

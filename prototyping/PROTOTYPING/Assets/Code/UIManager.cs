using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverMenuLose;
    public GameObject gameOverMenuWin;
    public String mainMenu = "mainMenu";
    public String nextLevel;

    private void OnEnable()
    {
        Mind.Lose += EnableGameOverMenuLose; //subscribing to the event that gets called on game over.
        //this line means that when we call the Mind.GameOver event, EnableGameOverMenu gets called
        Mind.Win += EnableGameOverMenuWin;
    }

    private void OnDisable()
    {
        Mind.Lose -= EnableGameOverMenuLose;
        Mind.Win -= EnableGameOverMenuWin;

    }

    public void EnableGameOverMenuLose()
    {
        gameOverMenuLose.SetActive(true);
    }

    public void EnableGameOverMenuWin()
    {
        gameOverMenuWin.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GOTOMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void EndTurnButton()
    {
        if (Mind.instance.battleStatus == Mind.BattleStatus.PLAYER_TURN)
        {
            Mind.instance.EndPlayerTurn();
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene((nextLevel));
    }
    
}

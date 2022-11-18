using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    //config - change for each scene
    public String mainMenu = "mainMenu";
    public String nextLevel;
    public String objective;
    
    //constant - stay same between prefab instances
    public Sprite[] movementDiceSides = new Sprite[6];
    public Sprite[] attackDiceSides = new Sprite[6];
    public GameObject levelSelect;
    public GameObject gameOverMenuLose;
    public GameObject gameOverMenuWin;
    public GameObject textScreen;
    private TMP_Text _objectiveText;

    public TMP_Text _treasureCount;
    public int treasureCount;


    private void Awake()
    {
        instance = this;
        _objectiveText = textScreen.GetComponent<TMP_Text>();
        treasureCount = 0;
        if (_objectiveText != null)
        {
            _objectiveText.text = objective;
        }
    }

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

    public void EnableLevelSelect()
    {
        levelSelect.SetActive(true);
    }

    public void DisableLevelSelect()
    {
        levelSelect.SetActive(false);
    }

    public void GOTOLevel(string level)
    {
        Debug.Log("level to load: " + level);
        SceneManager.LoadScene(level);
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

    public void UpdateTreasureCount()
    {
        treasureCount = treasureCount + 1;
        _treasureCount.text = "treasure:" + treasureCount + "/4";
    }
    
}

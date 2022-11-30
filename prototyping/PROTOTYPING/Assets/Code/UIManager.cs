using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
    
    public GameObject helpPanel;
    public List<GameObject> subPanels;
    private GameObject _activeHelpSubPanel;

    public TMP_Text _treasureCount;
    public int treasureCount;

    private bool _currentMuteState;
    private float _volume;


    private void Awake()
    {
        instance = this;
        _currentMuteState = false;
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

    //scene loading pt 1
    public void GOTOLevel(string level)
    {
        Debug.Log("level to load: " + level);
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    //endgame
    public void EnableGameOverMenuLose()
    {
        gameOverMenuLose.SetActive(true);
    }

    public void EnableGameOverMenuWin()
    {
        gameOverMenuWin.SetActive(true);
    }

    
    //buttons
    public void LockDice()
    {
        Mind.instance.LockDicePressed();
    }
    
    public void EndTurnButton()
    {
        if (Mind.instance.battleStatus == Mind.BattleStatus.PLAYER_TURN)
        {
            Mind.instance.EndPlayerTurn();
        }
    }

    
    //scene loading pt 2
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void GOTOMainMenu()
    {
        SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
    }
    
    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevel,LoadSceneMode.Single);
    }

    

    //help panel stuff
    public void toggleHelpPanel(bool state)
    {
        helpPanel.SetActive(state);
    }

    public void activeSubPanelOff()
    {
        helpPanel.SetActive(true);
        _activeHelpSubPanel.SetActive(false);
    }
    
    public void EnableSubPanel(int panel)
    {
        _activeHelpSubPanel = subPanels[panel];
        _activeHelpSubPanel.SetActive(true);
        helpPanel.SetActive(false);
    }


    public void toggleMute()
    {
        switch (_currentMuteState)
        {
            case false:
                _volume = AudioListener.volume;
                AudioListener.volume = 0;
                _currentMuteState = true;
                break;
            case true:
                AudioListener.volume = _volume;
                _currentMuteState = false;
                break;
        }
    }


    

    //treasure section
    public void UpdateTreasureCount()
    {
        treasureCount = treasureCount + 1;
        _treasureCount.text = "treasure:" + treasureCount + "/4";
    }
    
    public int getTreasureCount()
    {
        return treasureCount;
    }
}

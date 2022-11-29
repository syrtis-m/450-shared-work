using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Mind : MonoBehaviour
{
    //the mind class manages all player characters and AI characters in a scene.
    //it initializes characters using the Awake() function and then handles turn-management
    
    //outlets
    public static Mind instance; //singleton pattern. access mind in other files by Mind.instance
    
    //config
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public Button lockDiceButton;
    public Button endTurnButton;
    public List<GameObject> movementDice;
    public List<GameObject> attackDice;
    public List<GameObject> playerCharacters;
    public List<ItemSlot> itemSlots;
    public List<DragAndDrop> dragAndDropCharacters;
    public List<GameObject> aiCharacters = new List<GameObject>(0);//when a character dies, they should be deleted from the scene. no longer in a character array
    public Camera camera;//need the camera so that characters can pathfind
    public GameObject movementTilePrefab; //for movement tile rendering
    public GameObject attackTilePrefab;
    public GameObject enemy_turn_start;
    public GameObject player_turn_start;
    public Shader lineShader;
    public float playerSplashScreenTime = 2f;
    public float enemySplashScreenTime = 2f;
    public float aiTurnPauseFor = 1.5f;
    

    
    //state tracking
    public GameObject currentPlayer;
    public BattleStatus battleStatus;

    public enum BattleStatus
    {//use this to track turns in mind.cs
        PLAYER_TURN,
        AI_TURN,
    };

    public enum characterStatus
    {//use this to track where characters are in their movement stuff
        TURN_STARTED,
        DICE_LOCKED,
        MOVED,
        ATTACKED,
        DONE,
    }//when a character dies, they should be deleted from the scene. no longer in a character array

    public static event Action Lose; //this is our game over system using the unity event system https://youtu.be/ZfRbuOCAeE8

    public static event Action Win;

    public static event Action BeginPlayerTurnEvent; //talks with:
    //DragAndDrop
    //MovementDice
    //AttackDice
    //PlayerCharMvmt
    
    private void Awake()
    {//set up the mind & characters managed by it
        instance = this; //singleton pattern. access mind in other files by Mind.instance
    }

    void Start()
    {
        var objList = GameObject.FindGameObjectsWithTag("ai");

        foreach (var obj in objList)
        {
            aiCharacters.Add(obj);
        }
        currentPlayer = playerCharacters[0];
        BeginPlayerTurn();
    }

    public void ChangePlayer(GameObject newCharacter)
    {

        if (battleStatus == BattleStatus.AI_TURN)
        {
            return;
        }
        
        if (newCharacter.Equals(currentPlayer) == false)
        {
            currentPlayer.GetComponent<PlayerCharMvmt>().enabled = false;
            currentPlayer = newCharacter;
        }

    }

    public bool LockDiceEnabled()
    {
        foreach (DragAndDrop dragAndDropCharacter in dragAndDropCharacters)
        {
            if (dragAndDropCharacter.GetComponent<DragAndDrop>().slot == null)
            {
                lockDiceButton.interactable = false;
                return false;
            }
        }
        lockDiceButton.interactable = true;
        return true;
    }

    public void LockDicePressed()
    {
        foreach (DragAndDrop dragAndDropCharacter in dragAndDropCharacters)
        {
            dragAndDropCharacter.GetComponent<DragAndDrop>().diceLocked = true;
        }
        foreach(GameObject playerCharacter in playerCharacters)
        {
            playerCharacter.GetComponent<PlayerCharMvmt>().LockDice();
        }
        lockDiceButton.interactable = false;
        destroyHighlightTiles();
    }

    public static void destroyHighlightTiles()
    {//destroys all highlight tiles
        var highlightTiles = GameObject.FindGameObjectsWithTag ("highlight");
        //this works because the prefab tile has the tag highlight.
        for (int i = 0; i < highlightTiles.Length; i++)
        {
            Destroy(highlightTiles[i]);
        }
    }
    
    public void TrimCharAILists()
    {//this makes sure that there are only as many player characters or AI characters in lists as there should be.
        for(var i = playerCharacters.Count - 1; i > -1; i--)
        {
            if (playerCharacters[i] == null)
                playerCharacters.RemoveAt(i);
        }
        
        for(var i = aiCharacters.Count - 1; i > -1; i--)
        {
            if (aiCharacters[i] == null)
                aiCharacters.RemoveAt(i);
        }
    }

    public void IsPlayerTurnOver()
    {
        foreach (var character in playerCharacters)
        {
            if (character.GetComponent<PlayerCharMvmt>().getActionStatus() != characterStatus.DONE)
            {
                return;
            }
        }
        EndPlayerTurn();
    }

    private void BeginPlayerTurn()
    {
        StartCoroutine(player_turn_splash());
        lockDiceButton.interactable = false;
        endTurnButton.interactable = true;
        foreach (DragAndDrop dragAndDropCharacter in dragAndDropCharacters)
        {
            dragAndDropCharacter.GetComponent<DragAndDrop>().diceLocked = false;
            dragAndDropCharacter.GetComponent<DragAndDrop>().slot = null;
        }
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.GetComponent<ItemSlot>().slotCharacter = null;
        }
        foreach (GameObject playerCharacter in playerCharacters)
        {
            playerCharacter.GetComponent<PlayerCharMvmt>().resetChar();
        }

        BeginPlayerTurnEvent?.Invoke(); //broadcast event to shit that needs to know player turn has begun
        
        foreach (var character in aiCharacters)
        {//un-greys-out ai characters
            character.GetComponent<AICharacter>().resetColor();
        }
        
        Debug.Log("BeginPlayerTurn()");
        
        //show animation showing it's a player turn
        currentPlayer = playerCharacters[0];
    }
    
    //EndPlayerTurn needs to be separate from BeginPlayerTurn because we don't know the order that player characters move in.
    public void EndPlayerTurn()
    {
        endTurnButton.interactable = false;
        destroyHighlightTiles();
        Debug.Log("EndPlayerTurn()");
        //disable all characters
        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().enabled = false;
            character.GetComponent<PlayerCharMvmt>().setStatusDone(); //ensures they're all set to done if called from something else
            
        }
        
        TrimCharAILists();
        WinLossCheck();
    }


    private void AITurn()
    {

        Debug.Log("AITurn()");

        foreach (var character in aiCharacters)
        {
            character.GetComponent<AICharacter>().resetStatus();
        }
        
        
        IsAITurnOver();
    }

    public void IsAITurnOver()
    {//called by each

        foreach (var character in aiCharacters)
        {
            if ((character != null) && (character.GetComponent<AICharacter>().getActionStatus() != characterStatus.DONE))
            {
                character.GetComponent<AICharacter>().Turn();
                return;
            }
        }
        EndAITurn();
    }
    
    private void EndAITurn()
    {
        TrimCharAILists();
        
        Debug.Log($"aicharacter count: {aiCharacters.Count}");
        

        WinLossCheck();
    }


    public void WinLossCheck()
    {//check the win/loss condition at the end of each turn.
        if (Objective.instance != null)
        {
            if (Objective.instance.CheckObjectiveWin())
            {
                EndGameWin();
                return;
            }
        }
        
        if (battleStatus == BattleStatus.PLAYER_TURN)
        {
            battleStatus = BattleStatus.AI_TURN;
            if ((aiCharacters.Count > 0) && (playerCharacters.Count > 0))
            {
                StartCoroutine(enemy_turn_splash());
            }
            else if ((aiCharacters.Count == 0) && (playerCharacters.Count > 0))
            {
                EndGameWin();
            }
            else if ((aiCharacters.Count > 0) && (playerCharacters.Count == 0))
            {
                EndGameLose();
            }
            else
            {
                Debug.Log("error in WinLossCheck() end player turn");
            }
        } 
        else if (battleStatus == BattleStatus.AI_TURN)
        {
            battleStatus = BattleStatus.PLAYER_TURN;
            if ((aiCharacters.Count > 0) && (playerCharacters.Count > 0))
            {
                BeginPlayerTurn();
            }
            else if ((aiCharacters.Count == 0) && (playerCharacters.Count > 0))
            {
                EndGameWin();
            }
            else if ((aiCharacters.Count > 0) && (playerCharacters.Count == 0))
            {
                EndGameLose();
            }
            else
            {
                Debug.Log("error in WinLossCheck() end AI turn");
            }
        }
        else
        {
            //TODO add an else if for reaching an objective location
            Debug.Log("error in WinLossCheck()");
        }
    }

    public void EndGameLose()
    {
        Debug.Log("EndGameLose()");
        Time.timeScale = 0;
        Lose?.Invoke();//observer pattern using unity event system
    }

    public void EndGameWin()
    {
        Debug.Log("EndGameWin()");
        Time.timeScale = 0;
        Win?.Invoke();
    }
    
    
    IEnumerator player_turn_splash()
    {
        var obj = Instantiate(player_turn_start);
            
        yield return new WaitForSeconds(playerSplashScreenTime);

        Destroy(obj);
    }
    
    IEnumerator enemy_turn_splash()
    {
        var obj = Instantiate(enemy_turn_start);
            
        yield return new WaitForSeconds(enemySplashScreenTime);
        
        AITurn();
        
        Destroy(obj);
    }
}

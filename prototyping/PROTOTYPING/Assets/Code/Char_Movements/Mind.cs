using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mind : MonoBehaviour
{
    //the mind class manages all player characters and AI characters in a scene.
    //it initializes characters using the Awake() function and then handles turn-management
    
    //outlets
    public static Mind instance; //singleton pattern. access mind in other files by Mind.instance
    
    //config
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public List<GameObject> movementDice;
    public List<GameObject> attackDice;
    public List<GameObject> playerCharacters;
    public List<GameObject> aiCharacters;//when a character dies, they should be deleted from the scene. no longer in a character array
    public Camera camera;//need the camera so that characters can pathfind
    public GameObject movementTilePrefab; //for movement tile rendering
    public GameObject attackTilePrefab;
    public GameObject enemy_turn_start;
    public GameObject player_turn_start;
    public float playerSplashScreenTime = 3f;
    public float enemySplashScreenTime = 3f;
    
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
        MOVED,
        ATTACKED,
        DONE,
    }//when a character dies, they should be deleted from the scene. no longer in a character array

    public static event Action GameOver; //this is our game over system using the unity event system https://youtu.be/ZfRbuOCAeE8

    private void Awake()
    {//set up the mind & characters managed by it
        instance = this; //singleton pattern. access mind in other files by Mind.instance
        
        foreach (var character in playerCharacters)
        {//set the ground and collision tilemaps for all player characters
            character.GetComponent<PlayerCharMvmt>().setTilemaps(groundTilemap, collisionTilemap);
            character.GetComponent<PlayerCharMvmt>().setCamera(camera);
            character.GetComponent<PlayerCharMvmt>().setTilePrefab(movementTilePrefab);
            character.GetComponent<PlayerCharMvmt>().setAttackTilePrefab(attackTilePrefab);
        }

        foreach (var character in aiCharacters)
        {
            character.GetComponent<AICharacter>().setTilemaps(groundTilemap, collisionTilemap);
            character.GetComponent<AICharacter>().setCamera(camera);
            character.GetComponent<AICharacter>().setTilePrefab(movementTilePrefab);
            character.GetComponent<AICharacter>().setAttackTilePrefab(attackTilePrefab);
        }
    }

    void Start()
    {
        currentPlayer = playerCharacters[0];
        battleStatus = BattleStatus.PLAYER_TURN;
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
        
        foreach (var character in aiCharacters)
        {//un-greys-out ai characters
            character.GetComponent<AICharacter>().resetColor();
        }
        
        Debug.Log("BeginPlayerTurn()");
        //show animation showing it's a player turn
        currentPlayer = playerCharacters[0];

        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().resetStatus();
            character.GetComponent<PlayerCharMvmt>().rollDice();
            character.GetComponent<PlayerCharMvmt>().resetColor();
        }
    }
    
    //EndPlayerTurn needs to be separate from BeginPlayerTurn because we don't know the order that player characters move in.
    public void EndPlayerTurn()
    {
        battleStatus = BattleStatus.AI_TURN;
        destroyHighlightTiles();
        Debug.Log("EndPlayerTurn()");
        //disable all characters
        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().enabled = false;
            character.GetComponent<PlayerCharMvmt>().setStatusDone(); //ensures they're all set to done if called from something else
            
        }
        
        TrimCharAILists();

        if ((aiCharacters.Count) > 0 && (playerCharacters.Count > 0))
        {
            StartCoroutine(enemy_turn_splash());
        }
        else if ((aiCharacters.Count) == 0 || (playerCharacters.Count == 0))
        {
            EndGame();
        }
        else
        {
            EndGame();
        }
        
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
        

        if ((aiCharacters.Count > 0) && (playerCharacters.Count > 0))
        {
            BeginPlayerTurn();
        }
        else if ((aiCharacters.Count) == 0 || (playerCharacters.Count == 0))
        {
            EndGame();
        }
        else
        {
            EndGame();
        }
    }


    private void EndGame()
    {
        Debug.Log("EndGame()");
        
        GameOver?.Invoke();//observer pattern using unity event system
    }
    
    
    IEnumerator player_turn_splash()
    {
        var obj = Instantiate(player_turn_start);
            
        yield return new WaitForSeconds(playerSplashScreenTime);

        battleStatus = BattleStatus.PLAYER_TURN;
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

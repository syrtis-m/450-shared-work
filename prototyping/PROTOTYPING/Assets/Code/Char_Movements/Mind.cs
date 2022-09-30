using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mind : MonoBehaviour
{
    //the mind class manages all player characters and AI characters in a scene.
    //it initializes characters using the Awake() function and then handles turn-management
    
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public GameObject[] playerCharacters;
    public GameObject[] aiCharacters;
    public Camera camera;//need the camera so that characters can pathfind
    public GameObject movementTilePrefab; //for movement tile rendering
    
    private GameObject _currentPlayer;

    private BattleStatus _battleStatus;
    private int _aliveAICharacters;
    private int _alivePlayerCharacters;

    private enum BattleStatus
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
    }

    private void Awake()
    {//set up the mind & characters managed by it
        foreach (var character in playerCharacters)
        {//set the ground and collision tilemaps for all player characters
            character.GetComponent<PlayerCharMvmt>().setTilemaps(groundTilemap, collisionTilemap);
            character.GetComponent<PlayerCharMvmt>().setCamera(camera);
            character.GetComponent<PlayerCharMvmt>().setTilePrefab(movementTilePrefab);
            
            _alivePlayerCharacters += 1;
            
        }

        foreach (var character in aiCharacters)
        {
            character.GetComponent<AICharacter>().setTilemaps(groundTilemap, collisionTilemap);
            character.GetComponent<AICharacter>().setCamera(camera);
            character.GetComponent<AICharacter>().setTilePrefab(movementTilePrefab);
            _aliveAICharacters += 1;
        }
    }
    
    void Start()
    {
        _currentPlayer = playerCharacters[0];
        _battleStatus = BattleStatus.PLAYER_TURN;
        BeginPlayerTurn();
    }
    
    public void ChangePlayer(GameObject newCharacter)
    {
        if (_battleStatus == BattleStatus.AI_TURN)
        {
            return;
        }
        
        if (newCharacter.Equals(_currentPlayer) == false)
        {
            _currentPlayer.GetComponent<PlayerCharMvmt>().enabled = false;
            _currentPlayer = newCharacter;
        }
        else
        {
            _currentPlayer.GetComponent<PlayerCharMvmt>().enabled = false;
        }
    }

    public static void destroyHighlightTiles()
    {//destroys all highlight tiles
        var highlightTiles = GameObject.FindGameObjectsWithTag ("highlight");
        for (int i = 0; i < highlightTiles.Length; i++)
        {
            Destroy(highlightTiles[i]);
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
        BeginAITurn();
    }
    
    public void EndPlayerTurn()
    {
        //TODO implement
        _battleStatus = BattleStatus.AI_TURN;
        //disable all characters
        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().enabled = false;
            
        }
        
    }

    public void BeginPlayerTurn()
    {
        //show animation showing it's a player turn
        _battleStatus = BattleStatus.PLAYER_TURN;
        _currentPlayer = playerCharacters[0];
        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().resetStatus(); //reset movement status
            //roll the dice of each character & store that in that chartacter's movement var
        }
        
    }

    public void BeginAITurn()
    {//todo Implement
        //do a animation showing its an AI turn
        _battleStatus = BattleStatus.AI_TURN;
        //go through order of characters.
        foreach (var character in aiCharacters)
        {
            character.GetComponent<AICharacter>().resetStatus();
        }
        //end turn.
    }

    public void EndAITurn()
    {//todo implement
        
        
        BeginPlayerTurn();
    }
    
    
}

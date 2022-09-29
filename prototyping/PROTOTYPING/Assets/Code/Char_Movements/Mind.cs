using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mind : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public GameObject[] playerCharacters;
    public GameObject[] aiCharacters;
    
    
    private GameObject _currentPlayer;

    private BattleStatus _battleStatus;
    private int _aliveAICharacters;
    private int _alivePlayerCharacters;

    private enum BattleStatus
    {
        PLAYER_TURN_START,
        PLAYER_TURN,
        AI_TURN,
    };

    private void Awake()
    {//set up the mind & characters managed by it
        foreach (var character in playerCharacters)
        {//set the ground and collision tilemaps for all player characters
            character.GetComponent<PlayerCharMvmt>().setTilemaps(groundTilemap, collisionTilemap);
            _alivePlayerCharacters += 1;
        }

        foreach (var character in aiCharacters)
        {
            _alivePlayerCharacters += 1;
        }
        //todo implement foreach setter for ground and col tilemaps for AI characters.
    }


    void Start()
    {
        BeginPlayerTurn();
    }

    public void ChangePlayer(GameObject newCharacter)
    {//changes currently active player
        if (_battleStatus == BattleStatus.PLAYER_TURN_START)
        {//case where no character is selected yet
            _battleStatus = BattleStatus.PLAYER_TURN;
            _currentPlayer = newCharacter;
        }
        
        if (_battleStatus == BattleStatus.PLAYER_TURN)
        {//case where a character is selected
            _currentPlayer.GetComponent<PlayerCharMvmt>().enabled = false;
            _currentPlayer = newCharacter;
        }
        
    }

    public void IsPlayerTurnOver()
    {
        var i = 0;
        foreach (var character in playerCharacters)
        {
            i += character.GetComponent<PlayerCharMvmt>().getActionCounter();
        }

        if (i == 0)
        {
            EndPlayerTurn();
            BeginAITurn();
        }
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
        _battleStatus = BattleStatus.PLAYER_TURN_START;
        foreach (var character in playerCharacters)
        {
            //reset all movement counters
            character.GetComponent<PlayerCharMvmt>().setActionCounter(2);
        }
        
    }

    public void BeginAITurn()
    {//todo Implement
        //go through order of characters. 
        //end turn.
    }

    public void EndAITurn()
    {//todo implement
        
        
        BeginPlayerTurn();
    }
    
    
}

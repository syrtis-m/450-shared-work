using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Mind : MonoBehaviour
{
    //the mind class manages all player characters and AI characters in a scene.
    //it initializes characters using the Awake() function and then handles turn-management
    
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public List<GameObject> playerCharacters;
    public List<GameObject> aiCharacters;//when a character dies, they should be deleted from the scene. no longer in a character array
    public Camera camera;//need the camera so that characters can pathfind
    public GameObject movementTilePrefab; //for movement tile rendering
    public GameObject enemy_turn_start;
    public GameObject player_turn_start;
    
    public GameObject currentPlayer;

    private BattleStatus _battleStatus;

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
    }//when a character dies, they should be deleted from the scene. no longer in a character array

    private void Awake()
    {//set up the mind & characters managed by it
        foreach (var character in playerCharacters)
        {//set the ground and collision tilemaps for all player characters
            character.GetComponent<PlayerCharMvmt>().setTilemaps(groundTilemap, collisionTilemap);
            character.GetComponent<PlayerCharMvmt>().setCamera(camera);
            character.GetComponent<PlayerCharMvmt>().setTilePrefab(movementTilePrefab);
            
        }

        foreach (var character in aiCharacters)
        {
            character.GetComponent<AICharacter>().setTilemaps(groundTilemap, collisionTilemap);
            character.GetComponent<AICharacter>().setCamera(camera);
            character.GetComponent<AICharacter>().setTilePrefab(movementTilePrefab);

        }
    }

    

    void Start()
    {
        currentPlayer = playerCharacters[0];
        _battleStatus = BattleStatus.PLAYER_TURN;
        BeginPlayerTurn();
    }



    public void ChangePlayer(GameObject newCharacter)
    {

        if (_battleStatus == BattleStatus.AI_TURN)
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
        //BeginAITurn();
    }
    
    public void BeginPlayerTurn()
    {
        StartCoroutine(player_turn_splash());
        Debug.Log("BeginPlayerTurn()");
        //show animation showing it's a player turn
        _battleStatus = BattleStatus.PLAYER_TURN;
        currentPlayer = playerCharacters[0];
        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().resetStatus(); //reset movement status
            //roll the dice of each character & store that in that chartacter's movement var
        }
    }
    
    //EndPlayerTurn needs to be separate from BeginPlayerTurn because we don't know the order that player characters move in.
    public void EndPlayerTurn()
    {
        //TODO show art saying "AI TURN BEGIN"
        Debug.Log("EndPlayerTurn()");
        //disable all characters
        foreach (var character in playerCharacters)
        {
            character.GetComponent<PlayerCharMvmt>().enabled = false;
            
        }

        if (playerCharacters.Count > 0)
        {
            AITurn();
        }
        else
        {
            EndGame();
        }
        
    }
    

    public void AITurn()
    {
        StartCoroutine(enemy_turn_splash());
        
        Debug.Log("AITurn()");
        //do a animation showing its an AI turn
        _battleStatus = BattleStatus.AI_TURN;
        //go through order of characters.
        foreach (var character in aiCharacters)
        {
            if (character!=null)
            {
                character.GetComponent<AICharacter>().resetStatus();
            }
            
        }
        //randomize list of AI characters
        //iterate through list of AI characters
        
        foreach (var character in aiCharacters)
        {
            if (character!=null)
            {
                character.GetComponent<AICharacter>().Turn();
            }
        }

        if (aiCharacters.Count > 0)
        {
            BeginPlayerTurn();
        }
        else
        {
            EndGame();
        }
        
        
    }

    private void EndGame()
    {
        Debug.Log("EndGame()");
        //give a graphic showing the game is over
        //do a reset button
        throw new NotImplementedException();
    }


    //IDK if we need this
    /*public bool IsAITurnOver()
    {
        foreach (var character in aiCharacters)
        {
            if (character.GetComponent<AICharacter>().getActionStatus()!= characterStatus.DONE)
            {
                return false;
            }
        }

        return true;
    }*/

    
    IEnumerator player_turn_splash()
    {
        //create prefab
        
        var obj = Instantiate(player_turn_start);
            
        yield return new WaitForSeconds(3f);
        
        Destroy(obj);

        //kill prefab
    }
    
    
//IMPORTANT: it doesn't seem to show enemy splash - this is just because it shows enemy and player splash at the same time.
    IEnumerator enemy_turn_splash()
    {
        var obj = Instantiate(enemy_turn_start);
            
        yield return new WaitForSeconds(3f);
        
        Destroy(obj);
    }

    
    
}

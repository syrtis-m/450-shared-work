using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mind : MonoBehaviour
{
    public GameObject[] characters;

    public GameObject current;

    private BattleStatus _battleStatus;

    private enum BattleStatus
    {
        START,
        PLAYER_TURN,
        AI_TURN,
    };
    
    
    void Start()
    {
        //current = characters[0];
        _battleStatus = BattleStatus.START;
    }

    public void ChangePlayer(GameObject newCharacter)
    {//changes currently active player
        if (_battleStatus != BattleStatus.START)
        {
            current.GetComponent<PlayerCharMvmt>().enabled = false;
        }
        _battleStatus = BattleStatus.PLAYER_TURN;
        current = newCharacter;
    }
    public void EndPlayerTurn()
    {
        //TODO implement
    }
    
    
}

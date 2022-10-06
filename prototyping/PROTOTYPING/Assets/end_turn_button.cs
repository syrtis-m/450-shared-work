using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class end_turn_button : MonoBehaviour
{
    private Mind _mind;
    
    void Start()
    {
        _mind = this.GetComponentInParent<Mind>();
    }

    private void OnMouseDown()
    {
        if (_mind.battleStatus == Mind.BattleStatus.PLAYER_TURN)
        {
            _mind.EndPlayerTurn();
        }
    }
}

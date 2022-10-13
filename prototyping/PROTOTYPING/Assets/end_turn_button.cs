using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class end_turn_button : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public Color defaultColor = Color.white;
    public Color mouseOverColor = new Color(190, 190, 190);
    
    void Start()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        _spriteRenderer.color = mouseOverColor;
    }

    private void OnMouseExit()
    {
        _spriteRenderer.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (Mind.instance.battleStatus == Mind.BattleStatus.PLAYER_TURN)
        {
            Mind.instance.EndPlayerTurn();
        }
    }
}

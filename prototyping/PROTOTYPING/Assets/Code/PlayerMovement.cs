using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    //tutorial this code is from: https://youtu.be/YnwOoxtgZQI
    private PlayerControls _controls;

    public Tilemap groundTilemap;

    public Tilemap collisionTilemap;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    void Start()
    {
        _controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        //the bit Main.Movement directly references an action map.
    }
    
    private void Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            transform.position += (Vector3)direction;
            //if we can move there, go there. each tile in a tilemap has a cell size of 1. direction is normalized to 1, so this should work
        }        
    }
    
    private bool CanMove(Vector2 direction)
    {
        //get grid position and where we want to move, then check to see
        Vector3Int gridPos = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        //convert world pos to cell pos. get the transform position plus direction.
        if (!groundTilemap.HasTile(gridPos) || collisionTilemap.HasTile(gridPos))
        {
            //if the ground doesn't exist or there is a collision there, return false.
            return false;
        }

        return true;
    }

   
}

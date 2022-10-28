using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CharMovement : MonoBehaviour
{
    private CharControl _charControl;

    private Vector2 _MousePos;

    public Camera camera;
    
    public Tilemap groundTilemap;
    
    public Tilemap collisionTilemap;
    

    private void Awake()
    {
        _charControl = new CharControl();
        _charControl.Disable();

        _charControl.Main.MousePos.performed += OnMousePos; 
        //add a call to OnMousePos to Main.MousePos.performed
        //this makes it so OnMousePos is called every time _charControl.Main.MousePos.performed is called
    }

    private void OnMousePos(InputAction.CallbackContext context)
    {
        _MousePos = context.ReadValue<Vector2>();
    }

    private void OnEnable()
     {
         _charControl.Enable();
     }

    private void OnDisable()
    {
        _charControl.Disable();
    }

    private void Start()
    {
        //_charControl.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        _charControl.Main.Click.performed += ctx => Move();
    }
    
    

    private void Move()
    {
        var worldPos = camera.ScreenToWorldPoint((Vector3)_MousePos);
        var gridPos = groundTilemap.WorldToCell(worldPos); //grid position of the target cell
        var worldPos2 = groundTilemap.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        //converting to cell and back again autocenters the target position.

        var deltaPos = worldPos2 - transform.position;
        
        //Debug.Log("___________________________________________");
        //Debug.Log( groundTilemap.WorldToCell(transform.position));
        //Debug.Log(gridPos);
        //Debug.Log(worldPos2);
        //Debug.Log(deltaPos);
        
        if (CanMove(gridPos))
        {
            transform.position += deltaPos;
            //currently just adding the world vector to the position vector. need to 
        }
    }

    

    private bool CanMove(Vector3Int gridPos)
    {
        if (!groundTilemap.HasTile(gridPos))
        {
            Debug.Log("a");
            return false;
        }

        if (collisionTilemap.HasTile(gridPos))
        {
            Debug.Log("b"); //gridpos is off by one
            return false;
        }
        
        return true;
    }
}

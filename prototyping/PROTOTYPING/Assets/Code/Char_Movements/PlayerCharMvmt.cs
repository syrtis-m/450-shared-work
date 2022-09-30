using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharMvmt : MonoBehaviour
{
    //outlets
    
    public Mind mind; //used for managing n characters in a scene
    public int movementRange; //how many tiles the player can traverse in one turn
    public SpriteRenderer character;


   
    //internal use
    private Camera _camera;
    private Vector2 _mousePos; //mouse position
    private MultiChar _multiChar; //inputaction class
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private Mind.characterStatus _status;
    private GameObject _movementTile;//todo change back to private
    private Color currentColor;
    private int numberOfMovements;

    //set up the input action receiving info
    private void Awake()
    {
        _multiChar = new MultiChar();
        _multiChar.Main.MousePos.performed += OnMousePos;
    }

    private void OnMousePos(InputAction.CallbackContext context)
    {
        _mousePos = context.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        _multiChar.Enable();
    }

    private void OnDisable()
    {
        _multiChar.Disable();
    }
    
    public void setTilemaps(Tilemap ground, Tilemap collision)
    {
        _groundTilemap = ground;
        _collisionTilemap = collision;
    }

    public void resetStatus()
    {
        _status = Mind.characterStatus.TURN_STARTED;
    }

    public Mind.characterStatus getActionStatus()
    {
        return _status;
    }

    public void setCamera(Camera camera)
    {
        _camera = camera;
    }

    public void setTilePrefab(GameObject movementTile)
    {
        _movementTile = movementTile;
    }
    
    void OnMouseDown()
    {//triggers when you click the gameobject as long as it has a collider

        GetComponent<PlayerCharMvmt>().enabled = true;
        mind.ChangePlayer(this.gameObject);
        //Debug.Log("origin: " + transform.position);
        _pathFinding.drawTiles(transform.position, movementRange); //draw tiles

        
        if (GetComponent<PlayerCharMvmt>().enabled)
        {
            character = GetComponent<SpriteRenderer>();
            character.color = Color.yellow;
        }
        else
        {
            character = GetComponent<SpriteRenderer>();
            character.color = currentColor;
        }

    }

    
    // Start is called before the first frame update
    
    void Start()
    {
        //this is the function that takes the click and does something with it
        _multiChar.Main.Select.performed += ctx => Click();
        enabled = false;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile);
        character = GetComponent<SpriteRenderer>();
        currentColor = character.color;

    }
    

    private void Click() //"teleport to destination" movement
    {
        
        var worldPos = _camera.ScreenToWorldPoint((Vector3)_mousePos);
        var gridPos = _groundTilemap.WorldToCell(worldPos); //grid position of the target cell
        var worldPos2 = _groundTilemap.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        //converting to cell and back again autocenters the target position.
        //Debug.Log("worldPos: " + worldPos + " gridPos: " + gridPos + " worldPos2: " + worldPos2);

        var deltaPos = worldPos2 - transform.position;
        
        if (_pathFinding.CanMove(gridPos))
        {
            var dist = _pathFinding.FindPathDist(worldPos, transform.position);

            if (dist <= movementRange)
            {
                transform.position += deltaPos;
                Mind.destroyHighlightTiles();//clears the highlight tiles on movement
            }
            else
            {
                //deselect character
                numberOfMovements = numberOfMovements + 1;
                character = GetComponent<SpriteRenderer>();
                character.color = currentColor;
            }
        }
        var cellpos = groundTilemap.CellToWorld(gridPos);
        Collider2D colliderAtDest = Physics2D.OverlapPoint(cellpos + new Vector3(0.5f, 0.5f));
        if (colliderAtDest)
        {
            character = GetComponent<SpriteRenderer>();
            character.color = currentColor;
        }
        if (numberOfMovements > 0)
        {
            GetComponent<PlayerCharMvmt>().enabled = false;
        }
        /*
        else if (AI at that point)
        {
            //attack
        }
        else
        {
            //deselect character
        }*/
        
        mind.IsPlayerTurnOver();
    }
    
    
    
}

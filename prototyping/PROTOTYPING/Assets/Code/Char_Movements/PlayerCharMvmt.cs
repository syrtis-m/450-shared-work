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
    


   
    //internal use
    private Camera _camera;
    private Vector2 _mousePos; //mouse position
    private MultiChar _multiChar; //inputaction class
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private Mind.characterStatus _status;
    private GameObject _movementTile;//todo change back to private
    private SpriteRenderer _character; //this is the spriterenderer that handles color
    private Color _currentColor; //this is the color of the character sprite. we flip to color.yellow to show they are selected
    private int numberOfMovements;

    //set up the input action receiving info
    private void Awake()
    {
        _character = GetComponent<SpriteRenderer>();
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
            
            _character.color = Color.yellow;
        }
        else
        {
            
            _character.color = _currentColor;
        }

    }

    private void Update()
    {
        if (_status == Mind.characterStatus.DONE)
        {
            enabled = false;
        }
    }


    // Start is called before the first frame update
    
    void Start()
    {
        //this is the function that takes the click and does something with it
        _multiChar.Main.Select.performed += ctx => Click();
        enabled = false;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile);
        _currentColor = _character.color;

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
                _status = Mind.characterStatus.MOVED; //set status to moved after character moved.
                enabled = false;
                _character.color = _currentColor; //deactivate color after movement
            }
            else
            {
                //deselect character
                _character.color = _currentColor;
            }
        }
        /*
        else if (AI at that point)
        {
            //attack
        }*/
        else
        {
            //deselect character
        }
        
        Collider2D colliderAtDest = Physics2D.OverlapPoint(worldPos2);
        if (colliderAtDest)
        {
            _character.color = _currentColor;
        }

        if (_status == Mind.characterStatus.MOVED)
        {//TODO remove once we have attacks implemented
            _status = Mind.characterStatus.DONE;//temp update because attacks aren't in yet.
        }
        
        mind.IsPlayerTurnOver(); //this should be the very last thing in Click()
    }
    
    public void Die()
    {//TODO test
        //this func should be called when the character dies
        mind.playerCharacters.Remove(gameObject);//this should remove the dead AI character from mind
        Destroy(gameObject);
    }
    
    
    
}

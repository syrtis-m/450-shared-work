using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharMvmt : MonoBehaviour
{
    //outlets
    
    public int movementRange; //how many tiles the player can traverse in one turn
    public int attackRange;
    public MovementDice movementDice;
    public AttackDice attackDice;
    public int health; //player health
    public int atkDamage; //player attack

    //internal use
    private Camera _camera;
    private Vector2 _mousePos; //mouse position
    private MultiChar _multiChar; //inputaction class
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private Mind.characterStatus _status;
    private GameObject _movementTile;
    private GameObject _attackTile;
    private SpriteRenderer _character; //this is the spriterenderer that handles color
    private Color _currentColor; //this is the color of the character sprite. 
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

    public void setStatusDone()
    {
        _status = Mind.characterStatus.DONE;
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

    public void setAttackTilePrefab(GameObject attackTile)
    {
        _attackTile = attackTile;
    }

    void OnMouseDown()
    {//triggers when you click the gameobject as long as it has a collider

        if (_status == Mind.characterStatus.DONE)
        {
            return;
        }
        
        GetComponent<PlayerCharMvmt>().enabled = true;
        Mind.instance.ChangePlayer(this.gameObject);
        //Debug.Log("origin: " + transform.position);
        if (_status == Mind.characterStatus.TURN_STARTED)
        {
            _pathFinding.drawTiles(transform.position, movementRange); //draw tiles
        }
        else if (_status == Mind.characterStatus.MOVED)
        {
            _pathFinding.drawAttackTiles(transform.position, attackRange);
        }

        
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
    {//this just makes sure player is disabled once it's done. _status is reset in mind.BeginPlayerTurn()
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
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile, _attackTile);
        _currentColor = _character.color;
        
    }
    
    public void rollDice()
    {
        movementRange = movementDice.RollDice();
        //attackRange = movementRange + 1;
        atkDamage = attackDice.RollDice();
    }
    

    private void Click() //"teleport to destination" movement
    {
        
        
        var worldPos = _camera.ScreenToWorldPoint((Vector3)_mousePos);
        var gridPos = _groundTilemap.WorldToCell(worldPos); //grid position of the target cell
        var worldPos2 = _groundTilemap.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        var currentPosition = transform.position;
        //converting to cell and back again autocenters the target position.
        //Debug.Log("worldPos: " + worldPos + " gridPos: " + gridPos + " worldPos2: " + worldPos2);

        var deltaPos = worldPos2 - transform.position;
        Collider2D colliderAtDest = Physics2D.OverlapPoint(worldPos2);
        if (_pathFinding.CanMove(gridPos))
        {
            var dist = _pathFinding.FindPathDist(worldPos, currentPosition);

            if (_status == Mind.characterStatus.TURN_STARTED)
            {
                if (dist <= movementRange)
                {
                    transform.position += deltaPos;
                    _status = Mind.characterStatus.MOVED; //set status to moved after character moved.
                    enabled = false;
                }

            }
            else if (_status == Mind.characterStatus.MOVED)
            {
                if (dist <= attackRange && colliderAtDest)
                {
                    if (colliderAtDest.gameObject.GetComponent<AICharacter>())
                    {
                        Attack(colliderAtDest.gameObject);
                    }
                }
            }
            _character.color = _currentColor;
            Mind.destroyHighlightTiles();
        }

        // Temporary: How is attacked different then done?
        if (_status == Mind.characterStatus.ATTACKED)
        {
            _character.color = Color.grey;
            _status = Mind.characterStatus.DONE;
        }
        Mind.instance.IsPlayerTurnOver(); //this should be the very last thing in Click()
    }

    public void Attack(GameObject enemy)
    {
        Destroy(enemy);
        _status = Mind.characterStatus.ATTACKED;
    }
    
    public void Die()
    {//TODO fix this so it works.
        //also you can't modify a list while looping through it. so no testing of Die() while looping through characters
        //this func should be called when the character dies
        Mind.instance.playerCharacters.Remove(gameObject);//this should remove the dead AI character from mind
        Mind.instance.currentPlayer = Mind.instance.playerCharacters[0];
        Destroy(gameObject);
    }


    public void resetColor()
    {
        _character.color = _currentColor;
    }

    
}

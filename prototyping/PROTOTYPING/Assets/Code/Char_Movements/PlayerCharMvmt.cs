using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharMvmt : MonoBehaviour
{
    //outlets
    
    public int movementRange; //how many tiles the player can traverse in one turn
    public int attackRange; //default is 2, archer has 3
    public int attackMod = 0; //attack modifier: +1atk means +1 to outgoing attacks
    public int defenseMod = 0; //defense modifier: +1def means -1 to incoming attacks
    public int maxHealth; //max health. store this info for healing
    public int currentHealth; //player health
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
    private Color _currentColor = Color.white; //this is the color of the character sprite. 
    private HealthBar _healthBar;
    public DragAndDrop selector;

    //set up the input action receiving info
    private void Awake()
    {
        _character = GetComponent<SpriteRenderer>();
        _multiChar = new MultiChar();
        _multiChar.Main.MousePos.performed += OnMousePos;
        Mind.BeginPlayerTurnEvent += resetChar;
        currentHealth = maxHealth;
        _healthBar = GetComponentInChildren<HealthBar>();
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

    public void resetChar()
    {
        _status = Mind.characterStatus.TURN_STARTED;
        if (_character != null)
        {//need a check against null for some reason to prevent an error after char death IDK
            _character.color = _currentColor;
        }
        //movement and attack range reset now bc we have drag and drop dicerolling
        movementRange = 0;
        atkDamage = 0;
    }

    public void setStatusDone()
    {
        _status = Mind.characterStatus.DONE;
    }

    public Mind.characterStatus getActionStatus()
    {
        return _status;
    }
    

    void OnMouseDown()
    {//triggers when you click the gameobject as long as it has a collider

        if (_status == Mind.characterStatus.DONE)
        {
            enabled = false;
            return;
        }

        GetComponent<PlayerCharMvmt>().enabled = true;
        Mind.destroyHighlightTiles();
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
        _multiChar.Main.Select.started += ctx => Click();
        enabled = false;
        _camera = Mind.instance.camera;
        _groundTilemap = Mind.instance.groundTilemap;
        _collisionTilemap = Mind.instance.collisionTilemap;
        _movementTile = Mind.instance.movementTilePrefab;
        _attackTile = Mind.instance.attackTilePrefab;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile, _attackTile);
        _currentColor = Color.white;
        _character.color = Color.white;
    }
   
    public void AssignDiceValues(int movementDice, int attackDice)
    {
        movementRange = movementDice;
        atkDamage = attackDice + attackMod;
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
                if (dist <= movementRange && colliderAtDest)
                {
                    if (worldPos2 == transform.position)
                    {
                        _status = Mind.characterStatus.MOVED;
                    }
                    
                    if (!(colliderAtDest.gameObject.GetComponent<AICharacter>() || colliderAtDest.gameObject.GetComponent<PlayerCharMvmt>()))
                    {
                        if (colliderAtDest.gameObject.GetComponent<HealthPowerup>())
                        {
                            currentHealth = maxHealth;
                            _healthBar.SetHealth(currentHealth);
                            Destroy(colliderAtDest.gameObject);
                        }
                        if (colliderAtDest.gameObject.GetComponent<AttackPowerup>())
                        {
                            atkDamage = atkDamage + 2;
                            Destroy(colliderAtDest.gameObject);
                        }
                        if (colliderAtDest.gameObject.GetComponent<RangePowerup>())
                        {
                            attackRange = attackRange + 1;
                            Destroy(colliderAtDest.gameObject);
                        }
                        if (colliderAtDest.gameObject.GetComponent<ArmorPowerup>())
                        {
                            maxHealth = maxHealth + 3;
                            currentHealth = currentHealth + 3;
                            _healthBar.SetMaxHealth(maxHealth, currentHealth);
                            Destroy(colliderAtDest.gameObject);
                        }
                        transform.position += deltaPos;
                        _status = Mind.characterStatus.MOVED; //set status to moved after character moved.
                    }
                }
                else if (dist <= movementRange)
                {
                    transform.position += deltaPos;
                    SoundManager.instance.PlayPlayerSoundMove();
                    _status = Mind.characterStatus.MOVED; //set status to moved after character moved.
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
            enabled = false;
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
        enemy.GetComponent<AICharacter>().takeDamage(atkDamage);
        _status = Mind.characterStatus.ATTACKED;
    }
    
    public void takeDamage(int atkAmnt)
    {// damage the character for a certain amount of health
        currentHealth -= (atkAmnt - defenseMod);
        _healthBar.SetHealth(currentHealth);
        if (currentHealth > 0)
        {
            SoundManager.instance.PlayPlayerSoundHurt();
        }
        //if dead then call destroy
        if (currentHealth <= 0)
        {
            SoundManager.instance.PlayPlayerSoundDeath();
            Die();
        }
    }
    
    public void Die()
    {
        Mind.instance.playerCharacters.Remove(gameObject);
        Destroy(gameObject);
        selector.Die();
        if (Mind.instance.playerCharacters.Count > 0)
        {
            Mind.instance.currentPlayer = Mind.instance.playerCharacters[0];
        }
    }
}

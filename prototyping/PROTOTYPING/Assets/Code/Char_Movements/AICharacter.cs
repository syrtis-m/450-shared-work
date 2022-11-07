using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Object = UnityEngine.Object;


public class AICharacter : MonoBehaviour
{
    //outlets
    public int noticeRange; //how many tiles away the AICharacter can notice playerCharacters. if a char is noticed, then attack + movement are allowed. default is 5
    public int movementRange = 3; //how many tiles the player can traverse in one turn
    public int maxHealth = 6; //max health -- we store this separately in case of healing. default is 6 as that's the max possible damage
    public int atkDamage = 3; //ai attack. default is 3
    public int attackRange = 2; //default is 2
    public float aiTurnPauseFor = 2f; //the amount of time the TurnCoroutine pauses for during execution.
    
    //state tracking
    public Mind.characterStatus status;
    public int currentHealth; //ai health

    
    //internal use - config
    private Camera _camera;
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private GameObject _movementTile;
    private GameObject _attackTile;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;
    private HealthBar _healthBar;

    private void Awake()
    {
        status = Mind.characterStatus.DONE;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
        currentHealth = maxHealth;
        _healthBar = GetComponentInChildren<HealthBar>();
        if (noticeRange < attackRange+movementRange)
        {
            throw new Exception("noticeRange < attackRange+movementRange. please fix <3");
        }
    }

    private void Start()
    {
        _camera = Mind.instance.camera;
        _groundTilemap = Mind.instance.groundTilemap;
        _collisionTilemap = Mind.instance.collisionTilemap;
        _movementTile = Mind.instance.movementTilePrefab;
        _attackTile = Mind.instance.attackTilePrefab;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile, _attackTile);
    }

    private void OnDrawGizmos()
    {
        //draws notice ranges of characters
        //because it uses ScanGrid, it doesn't draw on top of player characters even if they're in range

        if (_pathFinding != null)
        {
            
            Gizmos.color = _defaultColor;
            if (_defaultColor == Color.white)
            {
                Gizmos.color = Color.red;
            }

            var cellOrigin = _groundTilemap.WorldToCell(transform.position);
            var grid = _pathFinding.scanAttackGrid(cellOrigin, noticeRange);
            var gridSize = grid.Count;
            for (int i = 0; i < gridSize; i++)
            {
                var worldLoc = grid.Dequeue();
                Gizmos.DrawLine(transform.position, worldLoc);
            }
        }
    }

    public void resetStatus()
    {
        status = Mind.characterStatus.TURN_STARTED;
    }

    public Mind.characterStatus getActionStatus()
    {
        return status;
    }

    public void Turn()
    {
        StartCoroutine(TurnCoroutine());
    }

    IEnumerator TurnCoroutine()
    {
        var shortestDist = 10000;
        var closestPlayerLocation = new Vector3();
        var AIOrigin = _groundTilemap.WorldToCell(transform.position);
        var AI_location = _groundTilemap.CellToWorld(AIOrigin) + new Vector3(0.5f, 0.5f, 0);
        for (int i = 0; i < Mind.instance.playerCharacters.Count; i++) //Find closest player
        {
            var playerOrigin = _groundTilemap.WorldToCell(Mind.instance.playerCharacters[i].transform.position);
            var player_location = _groundTilemap.CellToWorld((playerOrigin)) + new Vector3(0.5f, 0.5f, 0);
            var dist = _pathFinding.FindPathDist(player_location, AI_location);
            if (dist <= shortestDist)
            {
                shortestDist = dist;
                closestPlayerLocation = player_location;
            }   
        }

        if (shortestDist <= noticeRange)
        {//case: within range to "notice" the player
            _pathFinding.drawLine(transform.position,closestPlayerLocation,Color.blue,duration:(aiTurnPauseFor/2));
            yield return new WaitForSeconds(aiTurnPauseFor/2);//we have this inside the notice case so that when it doesn't notice a character, it's turn goes faster
            
            Queue<Vector3> grid = _pathFinding.scanGrid(AIOrigin, movementRange);
            Debug.Log(grid.Count);
            var shortestMovementDist = 10000;
            var movementCell = new Vector3();
            var gridSize = grid.Count;
            for (int j = 0; j < gridSize; j++) //Scan the grid to find the block to move to and get closer to the player
            {
                var worldLoc = grid.Dequeue();
                var move_dist = _pathFinding.FindPathDist(closestPlayerLocation, worldLoc);
                //Debug.Log(worldLoc);
                //Debug.Log(grid.Count);
                if (move_dist <= shortestMovementDist && move_dist >= 1)
                {
                    shortestMovementDist = move_dist;
                    movementCell = worldLoc;
                }
            }
            //Debug.Log(shortestMovementDist);
            var deltaPos = movementCell - transform.position;
            Collider2D colliderAtDest = Physics2D.OverlapPoint(movementCell);
            if (colliderAtDest && (colliderAtDest.gameObject.GetComponent<HealthPowerup>() || colliderAtDest.gameObject.GetComponent<AttackPowerup>() || colliderAtDest.gameObject.GetComponent<RangePowerup>() || colliderAtDest.gameObject.GetComponent<ArmorPowerup>()))
            {
                Destroy(colliderAtDest.gameObject);
            }
            transform.position += deltaPos;
            status = Mind.characterStatus.MOVED;
            var currentAIPosition = _groundTilemap.WorldToCell(transform.position);
            var currentAILocation = _groundTilemap.CellToWorld(currentAIPosition) + new Vector3(0.5f, 0.5f, 0);
            for (int i = 0; i < Mind.instance.playerCharacters.Count; i++) //Find first player < atkrange
            {
                var playerOrigin = _groundTilemap.WorldToCell(Mind.instance.playerCharacters[i].transform.position);
                var player_location = _groundTilemap.CellToWorld((playerOrigin)) + new Vector3(0.5f, 0.5f, 0);
                var dist = _pathFinding.FindPathDist(player_location, currentAILocation);
                if (dist <= attackRange)
                {
                    yield return new WaitForSeconds(aiTurnPauseFor / 4);
                    _pathFinding.drawLine(transform.position, player_location, Color.red,duration:(aiTurnPauseFor/4));
                    yield return new WaitForSeconds(aiTurnPauseFor / 4);
                    Attack(Mind.instance.playerCharacters[i]);
                    break;
                }
            }
        }
        
        
        status = Mind.characterStatus.DONE;
        yield return new WaitForSeconds(aiTurnPauseFor/2);
        _spriteRenderer.color = Color.grey; //show it isn't active anymore
        

        Mind.instance.IsAITurnOver();

    }

    public void resetColor()
    {//un-greys-out character
        _spriteRenderer.color = _defaultColor;
    }

    public void Attack(GameObject player)
    {
        
        player.GetComponent<PlayerCharMvmt>().takeDamage(atkDamage);
        status = Mind.characterStatus.ATTACKED;
    }
    
    public void takeDamage(int atkAmnt)
    {// damage the character for a certain amount of health
        currentHealth -= atkAmnt;
        _healthBar.SetHealth(currentHealth);
        //if dead then call destroy
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Mind.instance.aiCharacters.Remove(gameObject);
        Destroy(gameObject);
    }

    public void OnMouseDown()
    {
        //draw movement + attack range
        if (_pathFinding != null)
        {
            
            Gizmos.color = _defaultColor;
            if (_defaultColor == Color.white)
            {
                Gizmos.color = Color.red;
            }

            var cellOrigin = _groundTilemap.WorldToCell(transform.position);
            var grid = _pathFinding.scanAttackGrid(cellOrigin, attackRange+movementRange);
            var gridSize = grid.Count;
            for (int i = 0; i < gridSize; i++)
            {
                var worldLoc = grid.Dequeue();
                Object.Instantiate(_attackTile,worldLoc,quaternion.identity);
            }
        }
    }

    public void OnMouseUp()
    {
        Mind.destroyHighlightTiles();
    }
}

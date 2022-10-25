using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;
using System.Collections.Generic;


public class AICharacter : MonoBehaviour
{
    //outlets
    public int movementRange; //how many tiles the player can traverse in one turn
    public int maxHealth; //max health -- we store this separately in case of healing
    public int currentHealth; //ai health
    public int atkDamage; //ai attack
    public int attackRange;
    public float aiTurnPauseFor = 2f; //the amount of time the TurnCoroutine pauses for during execution.
    
    //internal use
    private Camera _camera;
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private Mind.characterStatus _status;
    private GameObject _movementTile;
    private GameObject _attackTile;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;
    private HealthBar _healthBar;
    
    public void setTilemaps(Tilemap ground, Tilemap collision)
    {//config tilemapsand pathfinding objects
        _groundTilemap = ground;
        _collisionTilemap = collision;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile, _attackTile);
    }

    private void Awake()
    {
        _status = Mind.characterStatus.DONE;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
        currentHealth = maxHealth;
        _healthBar = GetComponentInChildren<HealthBar>();
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

    public void setAttackTilePrefab(GameObject attackTile)
    {
        _attackTile = attackTile;
    }

    public void Turn()
    {
        StartCoroutine(TurnCoroutine());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator TurnCoroutine()
    {
        yield return new WaitForSeconds(aiTurnPauseFor/2);
        
        //todo implement this
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
        transform.position += deltaPos;
        _status = Mind.characterStatus.MOVED;
        var currentAIPosition = _groundTilemap.WorldToCell(transform.position);
        var currentAILocation = _groundTilemap.CellToWorld(currentAIPosition) + new Vector3(0.5f, 0.5f, 0);
        for (int i = 0; i < Mind.instance.playerCharacters.Count; i++) //Find closest player to attack
        {
            var playerOrigin = _groundTilemap.WorldToCell(Mind.instance.playerCharacters[i].transform.position);
            var player_location = _groundTilemap.CellToWorld((playerOrigin)) + new Vector3(0.5f, 0.5f, 0);
            var dist = _pathFinding.FindPathDist(player_location, currentAILocation);
            if (dist <= attackRange)
            {
                yield return new WaitForSeconds(aiTurnPauseFor / 2);
                Attack(Mind.instance.playerCharacters[i]);
                break;
            }
        }
        _status = Mind.characterStatus.DONE;
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
        _status = Mind.characterStatus.ATTACKED;
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
    

}

using UnityEngine;
using UnityEngine.Tilemaps;

public class AICharacter : MonoBehaviour
{
    //outlets
    public int movementRange; //how many tiles the player can traverse in one turn
    public int health; //ai health
    public int atkDamage; //ai attack
    
    //internal use
    private Camera _camera;
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private Mind.characterStatus _status;
    private GameObject _movementTile;
    private GameObject _attackTile;



    public void setTilemaps(Tilemap ground, Tilemap collision)
    {//config tilemapsand pathfinding objects
        _groundTilemap = ground;
        _collisionTilemap = collision;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile, _attackTile);
    }

    private void Start()
    {
        _status = Mind.characterStatus.DONE;
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
        //todo implement this
        //this is the turn for an AI character
        //scan board
        //identify nearest (dist) character
        //move towards nearest character or attack character
        //set _status to DONE
    }
    
    public void Attack()
    {
        //todo implement this
    }

    public void Die()
    {
        //TODO test
        //also you can't modify a list while looping through it. so no testing of Die() while looping through characters
        //this func should be called when the character dies
        Mind.instance.playerCharacters.Remove(gameObject);//this should remove the dead AI character from mind
        
        Mind.instance.currentPlayer = Mind.instance.playerCharacters[0];
        Destroy(gameObject);
    }
}

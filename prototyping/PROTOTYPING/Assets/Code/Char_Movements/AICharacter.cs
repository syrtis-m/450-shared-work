using UnityEngine;
using UnityEngine.Tilemaps;

public class AICharacter : MonoBehaviour
{
    //outlets
    public Mind mind; //used for managing n characters in a scene
    public int movementRange; //how many tiles the player can traverse in one turn
    
    //internal use
    private Camera _camera;
    private PathFinding _pathFinding; //PathFinding.cs
    private Tilemap _groundTilemap; //set by mind.start()
    private Tilemap _collisionTilemap; //set by mind.start()
    private Mind.characterStatus _status;
    private GameObject _movementTile;


    public void setTilemaps(Tilemap ground, Tilemap collision)
    {//config tilemapsand pathfinding objects
        _groundTilemap = ground;
        _collisionTilemap = collision;
        _pathFinding = new PathFinding(_groundTilemap, _collisionTilemap, _movementTile);
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
    
    
    public void startTurn()
    {//this is the turn for an AI character
        //scan board
        //identify nearest (dist) character
        //move towards nearest character
    }
}

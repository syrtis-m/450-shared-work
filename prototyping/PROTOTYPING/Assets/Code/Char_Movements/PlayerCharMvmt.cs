using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharMvmt : MonoBehaviour
{
    //outlets
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public Camera camera;
    public Mind mind; //used for managing n characters in a scene
    public int movementRange; //how many tiles the player can traverse in one turn
   
    //internal use
    private Vector2 _mousePos;
    private MultiChar _multiChar;
    private PathFinding _pathFinding; //a pathfinding class
    
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

    private void OnMouseDown()
    {//triggers when you click the gameobject as long as it has a collider

        GetComponent<PlayerCharMvmt>().enabled = true;
        mind.ChangePlayer(this.gameObject);
        _pathFinding = new PathFinding(groundTilemap, collisionTilemap);
    }


    // Start is called before the first frame update
    
    void Start()
    {
        //this is the function that takes the click and does something with it
        _multiChar.Main.Select.performed += ctx => Click();
        GetComponent<PlayerCharMvmt>().enabled = false;
    }
    

    private void Click() //"teleport to destination" movement
    {
        
        var worldPos = camera.ScreenToWorldPoint((Vector3)_mousePos);
        var gridPos = groundTilemap.WorldToCell(worldPos); //grid position of the target cell
        var worldPos2 = groundTilemap.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0);
        //converting to cell and back again autocenters the target position.
        //Debug.Log("worldPos: " + worldPos + " gridPos: " + gridPos + " worldPos2: " + worldPos2);

        var deltaPos = worldPos2 - transform.position;
        
        
        if (_pathFinding.CanMove(gridPos))
        {
            int dist = _pathFinding.FindPathDist(worldPos, transform.position);

            if (dist <= movementRange)
            {
                transform.position += deltaPos;
            }
            
            
        }
    }
    
}

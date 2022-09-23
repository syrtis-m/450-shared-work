using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerCharMvmt : MonoBehaviour
{
    //config
    public Tilemap groundTilemap;
    public Tilemap collisionTilemap;
    public Camera camera;
    public Mind mind; //used for managing n characters in a scene
    
    //internal use
    private Vector2 _mousePos;
    private MultiChar _multiChar;
    
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

        var deltaPos = worldPos2 - transform.position;
        
        // Debug.Log("___________________________________________");
        // Debug.Log( groundTilemap.WorldToCell(transform.position));
        // Debug.Log(transform.position);
        // Debug.Log(gridPos);
        // Debug.Log(worldPos2);
        // Debug.Log(deltaPos);
        
        if (CanMove(gridPos, worldPos2))
        {
            transform.position += deltaPos;
            //currently just adding the world vector to the position vector. need to 
        }
    }
    
    //this func is called on every click
    /*private void Click() //follow path movement
    {
        if (/*character selected#1#)
        {
            if (/*second click on cell#1#)
            {
                //move along path.
                //maybe store the path as a linkedlist of cell coordinates, then iterate over it to move
            }
            
            if (/*first click on cell#1#)
            {
                if (/*if cell is in range of selected character && CanMove is true#1#)
                {
                    //teleport character
                    //show attack options
                }
                else
                {
                    //deselect character
                }
                //teleport them there
                //store selected cell coordinates
            }
        }
        else
        {
            if (/*click on character#1#)
            {
                //store "clicked character TRUE"
                //select character mode
            }
            else
            {
                //deselect currently selected character
            }
        }
        throw new NotImplementedException();
    }
    */
    

    //detects if you can move to a selected tile.
    private bool CanMove(Vector3Int gridPos, Vector2 worldPos)
    {
        if (!groundTilemap.HasTile(gridPos))
        {
            return false;
        }

        if (collisionTilemap.HasTile(gridPos))
        {
            return false;
        }

        Collider2D colliderAtDest = Physics2D.OverlapPoint(worldPos);//checking for a gameobject with physics at that point
        
        //if a collider at the destination exists, don't go there
        if (colliderAtDest)
        {
            return false;
            //NOTE - THIS MEANS ALL AI/PLAYER CHARACTERS MUST HAVE A COLLIDER.
        }
        
        return true;
    }
}

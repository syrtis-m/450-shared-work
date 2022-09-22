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


    // Start is called before the first frame update
    void Start()
    {
        //this is the function that takes the click and does something with it
        //TODO implement
        _multiChar.Main.Select.performed += ctx => Click();
    }

    //this func is called on every click
    private void Click()
    {
        if (/*character selected*/)
        {
            if (/*second click on cell*/)
            {
                //move along path.
                //maybe store the path as a linkedlist of cell coordinates, then iterate over it to move
            }
            
            if (/*first click on cell*/)
            {
                if (/*if cell is in range of selected character && CanMove is true*/)
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
            if (/*click on character*/)
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

    //detects if you can move to a selected tile.
    private bool CanMove(Vector3Int gridPos)
    {
        if (!groundTilemap.HasTile(gridPos))
        {
            return false;
        }

        if (collisionTilemap.HasTile(gridPos))
        {
            return false;
        }
        
        return true;
    }
}

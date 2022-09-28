using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridItem {
    public int x;
    public int y;
    public int d;

    public GridItem(int x, int y, int d)
    {
        this.x = x; //x position
        this.y = y; //y position
        this.d = d; //distance
    }
}
public class PathFinding
{//PathFinding class has methods that are useful for, well, pathfinding.
    /*Everything that's commented out is done bc PathFinding code should be called from other scripts
    //private CharControl _charControl;

    //private Vector2 _MousePos;

    //public Camera _camera;

    //public Tilemap groundTilemap;

    public Tilemap collisionTilemap;
    
    private void Awake()
    {
        _charControl = new CharControl();

        _charControl.Main.MousePos.performed += OnMousePos;
        //add a call to OnMousePos to Main.MousePos.performed
        //this makes it so OnMousePos is called every time _charControl.Main.MousePos.performed is called
    }

    private void OnMousePos(InputAction.CallbackContext context)
    {
        _MousePos = context.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        _charControl.Enable();
    }

    private void OnDisable()
    {
        _charControl.Disable();
    }
    private void Start()
    {
        _charControl.Main.Click.performed += ctx => FindPath();
    }*/

    private Tilemap _groundTilemap;
    private Tilemap _collisionTilemap;
    
    public PathFinding(Tilemap ground, Tilemap collision)
    {
        _groundTilemap = ground;
        _collisionTilemap = collision;
    }


    public bool CanMove(Vector3Int gridPos)
    {//detects if you can move to a selected tile.

        if (!_groundTilemap.HasTile(gridPos))
        {
            return false;
        }

        if (_collisionTilemap.HasTile(gridPos))
        {
            return false;
        }
        
        var cellpos = _groundTilemap.CellToWorld(gridPos);
        Collider2D colliderAtDest = Physics2D.OverlapPoint(cellpos + new Vector3(0.5f,0.5f));
        //checking for a gameobject with physics at that point
        //adjust cellpos slightly to test at the center of the cell, not an edge
        
        //if a collider at the destination exists, don't go there
        if (colliderAtDest)
        {
            return false;
            //NOTE - THIS MEANS ALL AI/PLAYER CHARACTERS MUST HAVE A COLLIDER.
        }
        
        return true;
    }

    public bool[,] FindTraversable()
    {//generate a boolean array of what tiles are traversable
        bool[,] travelledPath = new Boolean[_groundTilemap.size.x, _groundTilemap.size.y]; //https://forum.unity.com/threads/getting-and-storing-tilemap-positions-into-a-2d-array.628732/
        for (int x = _groundTilemap.origin.x, i = 0; i < (_groundTilemap.size.x); x++, i++)
        {
            for (int y = _groundTilemap.origin.y, j = 0; j < (_groundTilemap.size.y); y++, j++)
            {
                if (CanMove(new Vector3Int(x,y,0)))
                {
                    travelledPath[i, j] = true;
                }
                else
                {
                    travelledPath[i, j] = false;
                }
 
            }
        }

        return travelledPath;
    }
    
    //minimal modifications, doesn't work anymore.
    public int FindPathDist(Vector2 worldPos, Vector3 origin)
    {
        bool[,] travelledPath = FindTraversable();
        //I split FindTraversable out so we can call it in other variants of algs 
        
        //these adjust for different sizes of groundtilemap
        int gridXoffset = _groundTilemap.cellBounds.max.x;
        int gridYoffset = _groundTilemap.cellBounds.max.y;

        var gridPos = _groundTilemap.WorldToCell(worldPos);

        int targetX = gridPos.x + gridXoffset;
        int targetY = gridPos.y + gridYoffset;
        
        int startX = ((int)Math.Floor(origin.x)) + gridXoffset;
        int startY = ((int)Math.Floor(origin.y)) + gridYoffset;
        
        //Debug.Log("start: " + startX + ", "+startY+" target: "+targetX+", "+targetY);
        GridItem start = new GridItem(startX, startY, 0); //https://www.geeksforgeeks.org/shortest-distance-two-cells-matrix-grid/
        Queue <GridItem> path = new Queue<GridItem>();
        path.Enqueue(start);
        travelledPath[startX, startY] = false;
        while (path.Count > 0)
        {
            GridItem g = path.Peek();
            path.Dequeue();
            if (g.x == targetX && g.y == targetY)
            {
                //Debug.Log(g.d);
                return g.d;
            }
            if (g.x < (_groundTilemap.size.x - 1))
            {
                if (travelledPath[g.x + 1, g.y])
                {
                    path.Enqueue(new GridItem(g.x + 1, g.y, g.d + 1));
                    travelledPath[g.x + 1, g.y] = false;
                }
            }
            if (g.x > 0)
            {
                if (travelledPath[g.x - 1, g.y])
                {
                    path.Enqueue(new GridItem(g.x - 1, g.y, g.d + 1));
                    travelledPath[g.x - 1, g.y] = false;
                }
            }
            if (g.y < (_groundTilemap.size.y - 1))
            {
                if (travelledPath[g.x, g.y + 1])
                {
                    path.Enqueue(new GridItem(g.x, g.y + 1, g.d + 1));
                    travelledPath[g.x, g.y + 1] = false;
                }
            }
            if (g.y > 0)
            {
                if (travelledPath[g.x, g.y - 1])
                {
                    path.Enqueue(new GridItem(g.x, g.y - 1, g.d + 1));
                    travelledPath[g.x, g.y - 1] = false;
                }
            }
        }
        return -1;
    }
}

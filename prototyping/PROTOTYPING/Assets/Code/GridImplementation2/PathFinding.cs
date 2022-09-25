using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class GridItem {
    public int x;
    public int y;
    public int d;

    public GridItem(int x, int y, int d)
    {
        this.x = x;
        this.y = y;
        this.d = d;
    }
}
public class PathFinding : MonoBehaviour
{
    private CharControl _charControl;

    private Vector2 _MousePos;

    public Camera _camera;

    public Tilemap groundTilemap;

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
    }

    private void Update()
    {
        
    }
    private int FindPath()
    {
        bool[,] travelledPath = new Boolean[groundTilemap.size.x, groundTilemap.size.y]; //https://forum.unity.com/threads/getting-and-storing-tilemap-positions-into-a-2d-array.628732/
        for (int x = groundTilemap.origin.x, i = 0; i < (groundTilemap.size.x); x++, i++)
        {
            for (int y = groundTilemap.origin.y, j = 0; j < (groundTilemap.size.y); y++, j++)
            {
                if (collisionTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    travelledPath[i, j] = false;
                }
                else
                {
                    travelledPath[i, j] = true;
                }
            }
        }
        var worldPos = _camera.ScreenToWorldPoint((Vector3)_MousePos);
        var gridPos = groundTilemap.WorldToCell(worldPos);
        int targetX = gridPos.x + 5;
        int targetY = gridPos.y + 5;
        int startX = ((int)Math.Floor(transform.position.x)) + 5;
        int startY = ((int)Math.Floor(transform.position.y)) + 5;
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
                Debug.Log(g.d);
                return g.d;
            }
            if (g.x < (groundTilemap.size.x - 1))
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
            if (g.y < (groundTilemap.size.y - 1))
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

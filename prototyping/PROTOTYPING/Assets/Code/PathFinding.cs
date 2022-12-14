using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;



public class GridItem {
    public readonly int x;
    public readonly int y;
    public readonly int d;

    public GridItem(int x, int y, int d)
    {
        this.x = x; //x position
        this.y = y; //y position
        this.d = d; //distance
    }
}
public class PathFinding
{//PathFinding class for when you need to do pathfinding. each character & AI instaniates a copy
   
    private readonly Tilemap _groundTilemap;
    private readonly Tilemap _collisionTilemap;
    private readonly GameObject _movementTile;
    private readonly GameObject _attackTile;
    private Shader _lineShader;



    public PathFinding(Tilemap ground, Tilemap collision, GameObject movementTile, GameObject attackTile)
    {
        _groundTilemap = ground;
        _collisionTilemap = collision;
        _movementTile = movementTile;
        _attackTile = attackTile;
        _lineShader = Mind.instance.lineShader;
    }


    public bool CanMove(Vector3Int cellPos)
    {//detects if you can move to a selected tile. takes a cell position (use .WorldToCell to get)

        if (!_groundTilemap.HasTile(cellPos))
        {
            return false;
        }

        if (_collisionTilemap.HasTile(cellPos))
        {
            return false;
        }
        
        var worldPos = _groundTilemap.CellToWorld(cellPos);
        Collider2D colliderAtDest = Physics2D.OverlapPoint(worldPos + new Vector3(0.5f,0.5f));
        //checking for a gameobject with physics at that point
        //adjust cellpos slightly to test at the center of the cell, not an edge
        
        //if a collider at the destination exists, don't go there
        if (colliderAtDest)
        { //todo consider removing player and AI from this list so you can do chokepoint tactics
            if (colliderAtDest.gameObject.GetComponent<AICharacter>() || colliderAtDest.gameObject.GetComponent<PlayerCharMvmt>() || colliderAtDest.gameObject.GetComponent<HealthPowerup>() || colliderAtDest.gameObject.GetComponent<AttackPowerup>() || colliderAtDest.gameObject.GetComponent<RangePowerup>() || colliderAtDest.gameObject.GetComponent<ArmorPowerup>() || colliderAtDest.gameObject.GetComponent<Objective>() || colliderAtDest.gameObject.GetComponent<Treasure>())
            {//UPDATE THIS LIST WHENEVER YOU ADD SOMETHING WITH A COLLIDER THAT YOU WANT TO BE ABLE TO MOVE ON TOP OF
                return true;
            }//need this block for AI movement pathing to work.
            return false; 
            //This is changed so that the distance between a player and an enemy can be calculated
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

    //todo fix bug where coordinates are getting screwed up. tiles being drawn in the wrong place
    //todo fix bug where currently too few tiles are getting created
    
    public void drawTiles (Vector2 worldPos, int movementRange)
    {
        Mind.destroyHighlightTiles();
        //create a tilePrefab at each xy coordinate of every griditem in targets
        var cellOrigin = _groundTilemap.WorldToCell(worldPos);
        var grid = scanGrid(cellOrigin, movementRange);
        var gridSize = grid.Count;
        for (int i = 0; i < gridSize; i++)
        {
            var worldLoc = grid.Dequeue();
            //Debug.Log($"x:{worldLoc.x}, y: {worldLoc.y}, z:{worldLoc.z}");
            Object.Instantiate(_movementTile,worldLoc,quaternion.identity);
            
        }
    }

    public void drawAttackTiles (Vector2 worldPos, int attackRange)
    {
        var cellOrigin = _groundTilemap.WorldToCell(worldPos);
        var grid = scanAttackGrid(cellOrigin, attackRange);
        var gridSize = grid.Count;
        for (int i = 0; i < gridSize; i++)
        {
            var worldLoc = grid.Dequeue();
            Object.Instantiate(_attackTile, worldLoc, quaternion.identity);
        }
    }

    //todo rewrite so it can take into account fixed attack ranges  
    public Queue<Vector3> scanGrid(Vector3Int cell_origin, int range)
    {//scans the grid to see where you can move to
        var world_origin = _groundTilemap.CellToWorld((cell_origin)) + new Vector3(0.5f, 0.5f, 0);//used with finddist
        //Debug.Log($"world_origin: {world_origin}");
        
        var grid = new Queue<Vector3>();
        
        var minx = world_origin.x - range;
        var maxx = world_origin.x + range;
        var miny = world_origin.y - range;
        var maxy = world_origin.y + range;
        //Debug.Log($"minx: {minx}, maxx: {maxx},  miny: {miny}, maxy: {maxy}");


        for (float i = minx; i <= maxx; i++)
        {
            for (float j = miny; j <= maxy; j++)
            {
                var target_cell = _groundTilemap.WorldToCell(new Vector3(i, j, 0));
                //Debug.Log($"target_cell: {target_cell.x}, {target_cell.y}, {target_cell.z}");
                
                //todo check for an object with tag highlight at that point
                if (CanMove(target_cell))
                {
                    var target_world = _groundTilemap.CellToWorld(target_cell) + new Vector3(0.5f,0.5f, 0);
                    Collider2D colliderAtDest = Physics2D.OverlapPoint(target_world);
                    if (!colliderAtDest || colliderAtDest.gameObject.GetComponent<HealthPowerup>() || colliderAtDest.gameObject.GetComponent<AttackPowerup>() || colliderAtDest.gameObject.GetComponent<RangePowerup>() || colliderAtDest.gameObject.GetComponent<ArmorPowerup>() || colliderAtDest.gameObject.GetComponent<Treasure>())
                    {
                        var dist = FindPathDist(target_world, world_origin);
                        //Debug.Log($"dist: {dist}, target_world: {target_world.x}, {target_world.y}, {target_world.z}");
                        if ((dist <= range) && (dist != -1))
                        {
                            //var obj = Object.Instantiate(_movementTile,target_world,quaternion.identity);
                            grid.Enqueue(target_world);
                        }
                    }
                }
            }
        }

        return grid;
    }

    public Queue<Vector3> scanAttackGrid(Vector3Int cell_origin, int range)
    {//scans the grid to see where you can move to
        var world_origin = _groundTilemap.CellToWorld((cell_origin)) + new Vector3(0.5f, 0.5f, 0);//used with finddist
                                                                                                  //Debug.Log($"world_origin: {world_origin}");

        var grid = new Queue<Vector3>();

        var minx = world_origin.x - range;
        var maxx = world_origin.x + range;
        var miny = world_origin.y - range;
        var maxy = world_origin.y + range;
        //Debug.Log($"minx: {minx}, maxx: {maxx},  miny: {miny}, maxy: {maxy}");


        for (float i = minx; i <= maxx; i++)
        {
            for (float j = miny; j <= maxy; j++)
            {
                var target_cell = _groundTilemap.WorldToCell(new Vector3(i, j, 0));
                //Debug.Log($"target_cell: {target_cell.x}, {target_cell.y}, {target_cell.z}");
                if (CanMove(target_cell))
                {
                    var target_world = _groundTilemap.CellToWorld(target_cell) + new Vector3(0.5f, 0.5f, 0);
                    Collider2D colliderAtDest = Physics2D.OverlapPoint(target_world);
                    if (!colliderAtDest || !colliderAtDest.gameObject.GetComponent<PlayerCharMvmt>())
                    {
                        var dist = FindPathDist(target_world, world_origin);
                        //Debug.Log($"dist: {dist}, target_world: {target_world.x}, {target_world.y}, {target_world.z}");

                        if ((dist <= range) && (dist != -1))
                        {
                            //var obj = Object.Instantiate(_attackTile, target_world, quaternion.identity);
                            grid.Enqueue(target_world);
                        }
                    }
                }
            }
        }

        return grid;
    }


    public int FindPathDist(Vector2 targetPos_world, Vector3 origin_world)
    {//find a path to a certain cell.
        //both worldPos and origin need to be in the world context, not cell context
        
        bool[,] travelledPath = FindTraversable();
        //I split FindTraversable out so we can call it in other variants of algs 
        
        //need to adjust for if 
        
        //these adjust for different sizes of groundtilemap
        int gridXoffset = _groundTilemap.cellBounds.max.x;
        int gridYoffset = _groundTilemap.cellBounds.max.y;
        
        var gridPos = _groundTilemap.WorldToCell(targetPos_world);

        int targetX = gridPos.x + gridXoffset;
        int targetY = gridPos.y + gridYoffset;
        
        int startX = ((int)Math.Floor(origin_world.x)) + gridXoffset;
        int startY = ((int)Math.Floor(origin_world.y)) + gridYoffset;
        
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
    }//we know this function works.
    
    public void drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(_lineShader);
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        
        if (duration != 0)
        {//so if duration is 0 it doesn't destroy
            GameObject.Destroy(myLine, duration);
        }

    }
}

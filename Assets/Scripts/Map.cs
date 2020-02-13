using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    int _mapSize = 20;

    public int mapSize => _mapSize;
    public int halfMapSize => _mapSize / 2;
    public Tile a => current;
    public Tile[,] pos => _pos;
    Tile tileFrom => _pos[fX, fY];
    Tile tileTo => _pos[tX, tY];
    int fX, fY, tX, tY;
    public TilePath thee;

    Tile current;
    Tile[,] _pos;
    public List<Tile> openTiles, closedTiles;

    public Map(int size)
    {
        _pos = new Tile[size, size];
        _mapSize = size;

        Assign();
    }

    public void BlockTiles(List<Vector2Int> b)
    {
        for (int i = 0; i < b.Count; i++)
        {
            _pos[b[i].x, b[i].y].free = false;
        }
    }

    void Assign()
    {
        // first we init
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                _pos[i, j] = new Tile(i, j);
            }
        }
    }

    public List<Vector2Int> CalcAvailbleMoves(Vector2Int source)
    {
        // get a reference to what we're going to return
        List<Vector2Int> ret = new List<Vector2Int>();

        // loop through the entire map size
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                // if the tile is in the right distance from the source & that tile is not the tile the source is on,
                if ((ManhattanDistance(source, pos[i, j].v2Int) <= GM.maxMoves) && source != pos[i, j].v2Int)
                {
                    // also IF that space is reachable in 5 or less moves
                    if (FindPath(source.x, source.y, pos[i, j].x, pos[i, j].y)?.dist <= 5)
                    {
                        // add it to the return reference
                        ret.Add(pos[i, j].v2Int);
                    }
                }
            }
        }

        return ret;
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        // calculate the X + Y distance between a & b
        return (Mathf.Abs(a.x - b.x) + (Mathf.Abs(a.y - b.y)));
    }

    public static int SelectAnAngle(List<Vector2Int> toCheck, float iX, float iY, Vector2Int whoLoc)
    {
        int ret = -1;

        // 
        float[] temp = new float[toCheck.Count];

        // Get all availible select positions & give them an ID
        for (int i = 0; i < toCheck.Count; i++)
        {
            temp[i] = GM.XYtoDeg(toCheck[i].x - whoLoc.x, toCheck[i].y - whoLoc.y);
        }

        float lowestDist = Mathf.Infinity;

        // Get the distance from desired point to all availible selected positions
        for (int i = 0; i < temp.Length; i++)
        {
            float desiredPoint = GM.XYtoDeg(iX, iY);

            float dist = Mathf.Abs(desiredPoint - temp[i]);

            // 
            if (dist < lowestDist)
            {
                lowestDist = dist;
                ret = i;
            }
        }

        // Return the availible selected position with the lowest distance
        return ret;
    }

    public TilePath FindPath(int _fX, int _fY, int _tX, int _tY)
    {
        return PathFinder(_fX, _fY, _tX, _tY);
    }

    public TilePath FindLimitedPath(int _fX, int _fY, int _tX, int _tY)
    {

        // if the tile we are trying to reach is out of range,
        if (ManhattanDistance(new Vector2Int(_fX, _fY), new Vector2Int(_tX, _tY)) > GM.maxMoves)
        {
            // appologize. right now.
            Debug.Log($"Sorry out of range!");

            return null;
        }

        return PathFinder(_fX, _fY, _tX, _tY);
    }

    TilePath PathFinder(int _fX, int _fY, int _tX, int _tY)
    {
        float start = Time.time;
        thee = new TilePath(new List<Tile>());

        fX = _fX;
        fY = _fY;
        tX = _tX;
        tY = _tY;

        // set list of tiles to be evaluated
        openTiles = new List<Tile>();
        // set list of tiles already evaluated
        closedTiles = new List<Tile>();

        // add current node to the open list
        AddTileToOpen(tileFrom);

        // While we still have tiles in the open tiles list
        while (openTiles.Count > 0)
        {
            // current tile = tile with the lowest f cost in the open set (start off with only 1 tile in the open tiles list so that is the lowest)
            current = GetLowestFCost();

            // if current == target, then we're done
            if (current == tileTo)
            {
                //Debug.Log($"AT END! Elapsed Time: {Time.time - start}");

                // 
                thee.path.Add(current);

                // While the current tile's parent does not = our starting tile,
                while (current.parent != tileFrom)
                {
                    // add it to the path
                    thee.path.Add(current.parent);
                    // update what the current node is
                    current = current.parent;
                }

                // Because we trace from the end to the start, we need to reverse the path
                thee.path.Reverse();

                return thee;
            }

            // if not then well Process this tile for exploration
            ProcessTile(current);
        }

        //Debug.Log($"CAN'T FIND! Elapsed Time: {Time.time - start}");

        return null;
    }

    void AddTileToOpen(Tile t)
    {
        if (!openTiles.Contains(t))
        {
            openTiles.Add(t);
        }
    }

    void ProcessTile(Tile c)
    {
        // Get & set all of the neighbors for that tile, which will also add it to the open tiles list
        current.SetNeighbors(GetNeighbors(current, current.parent, tileTo));

        // remove current from open since it's ben processed
        openTiles.Remove(c);

        // add current to closed since its been processed
        closedTiles.Add(c);
    }

    float GetDistance(Vector2 a, Vector2 b)
    {
        return (a - b).sqrMagnitude;
    }

    List<Tile> GetNeighbors(Tile current, Tile _from, Tile to)
    {
        _from = current;

        List<Tile> ret = new List<Tile>();

        // Set our current tile's fCost
        float gc = GetDistance(new Vector2(current.x, current.y), new Vector2(_from.x, _from.y));
        float hc = GetDistance(new Vector2(current.x, current.y), new Vector2(to.x, to.y));

        current.SetGnH(gc, hc);

        Tile n = _pos[current.x - 1, current.y];
        // set our neighbor's fcost
        if (current.x - 1 > 0 && !openTiles.Contains(n) && !closedTiles.Contains(n) && _pos[current.x - 1, current.y].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
        }

        n = _pos[current.x, current.y - 1];
        // 
        if (current.y - 1 > 0 && !openTiles.Contains(n) && !closedTiles.Contains(n) && _pos[current.x, current.y - 1].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
        }

        n = _pos[current.x + 1, current.y];
        // 
        if (current.x + 1 < mapSize - 1 && !openTiles.Contains(n) && !closedTiles.Contains(n) && _pos[current.x + 1, current.y].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
        }

        n = _pos[current.x, current.y + 1];
        // 
        if (current.y + 1 < mapSize - 1 && !openTiles.Contains(n) && !closedTiles.Contains(n) && _pos[current.x, current.y + 1].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
        }

        return ret;
    }

    Tile GetLowestFCost()
    {
        // 
        Tile lowest = new Tile(0, 0);
        lowest.SetGnH(0, Mathf.Infinity);

        // 
        for (int i = 0; i < openTiles.Count; i++)
        {
            if (openTiles[i].fCost < lowest.fCost)
            {
                lowest = openTiles[i];
            }
        }

        // 
        return lowest;
    }
}

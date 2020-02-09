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
    Tile current;

    Tile[,] pos;
    List<Tile> openTiles, closedTiles;

    public Map(int size)
    {
        pos = new Tile[size, size];
        _mapSize = size;
    }

    public void Assign()
    {
        // first we init
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                pos[i, j] = new Tile(i, j);
            }
        }

        // then we set the neighbors
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                pos[i, j].SetNeighbors(GetNeighbors(pos[i, j]));
            }
        }
    }

    public void FindPath(int fX, int fY, int tX, int tY)
    {
        // set list of tiles to be evaluated
        openTiles = new List<Tile>();
        // set list of tiles already evaluated
        closedTiles = new List<Tile>();

        // add current node to the open list
        openTiles.Add(pos[fX, fY]);

        // 
        while (openTiles.Count > 0)
        {
            // current tile = tile with the lowest f cost
            current = openTiles[0];

            // 
            current = GetLowestFCost(current);

            // remove current from open
            openTiles.Remove(current);

            // add current to closed
            closedTiles.Add(current);

            // if current == target, then we're done
            if (current == pos[tX, tY])
            {
                Debug.Log("AT END!");

                break;
            }

            // For testing
            for (int i = 0; i < current.neighbors.Count; i++)
            {
                Debug.Log($"{current.neighbors[i].loc}");
            }

            // 
            foreach (Tile n in current.neighbors)
            {
                // if the neighbor is occupied or if the neighbor is in the closed list,
                if (n.occupied || closedTiles.Contains(n))
                {
                    // skip it
                    continue;
                }

                // if new path to neighbor is shorter, or neighbor is not in open
                if (false)
                {
                    // set f cost of neighbor
                    // set parent of neighbor to current

                    // if neighbor is not in open
                    // add neighbor to open
                }
            }
        }
    }

    List<Tile> GetNeighbors(Tile current)
    {
        List<Tile> ret = new List<Tile>();

        // 
        if (current.y - 1 >= 0)
        {
            ret.Add(pos[current.x, current.y - 1]);
        }

        // 
        if (current.x + 1 < mapSize)
        {
            ret.Add(pos[current.x + 1, current.y]);
        }

        // 
        if (current.y + 1 < mapSize)
        {
            ret.Add(pos[current.x, current.y + 1]);
        }

        // 
        if (current.x - 1 >= 0)
        {
            ret.Add(pos[current.x - 1, current.y]);
        }

        return ret;
    }

    Tile GetLowestFCost(Tile currentTile)
    {
        for (int i = 0; i < openTiles.Count; i++)
        {
            // if they are the same tile,
            if (openTiles[i] == currentTile)
            {
                // skip it & dont waste time.
                continue;
            }

            // if the looped tile is cheaper than our current tile,
            // OR
            // the looped tile & the current node cost the same, && the looped tile has a lower hCost than the current tile
            if (openTiles[i].fCost < currentTile.fCost || (openTiles[i].fCost == currentTile.fCost && openTiles[i].hCost < currentTile.hCost))
            {
                // switch our current tile to that looped tile
                currentTile = openTiles[i];
            }
        }

        return currentTile;
    }
}

public class Tile
{
    //public Tile[] neighbor = new Tile[4];
    public bool occupied;

    public int x => _x;
    public int y => _y;
    public float fCost => gCost + hCost;
    public string loc => $"{x}, {y}";
    public List<Tile> neighbors => _neighbors;
    public float gCost, hCost; // g = dist from start pos; h = dis from end;

    Tile parent;
    List<Tile> _neighbors;
    bool open, closed;
    int _x, _y;

    public Tile(int X, int Y)
    {
        _x = X;
        _y = Y;

    }

    public void SetNeighbors(List<Tile> N)
    {
        if (_neighbors == null)
        {
            _neighbors = N;
        }
    }
}

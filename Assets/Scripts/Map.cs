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
    Tile tileFrom => current.parent == null ? current.parent : current;
    Tile tileTo => pos[tX, tY];
    int fX, fY, tX, tY;
    public TilePath thee = new TilePath(new List<Tile>());

    Tile current;
    Tile[,] pos;
    public List<Tile> openTiles, closedTiles;

    public Map(int size)
    {
        pos = new Tile[size, size];
        _mapSize = size;
    }

    public void BlockTiles(Vector2Int[] b)
    {
        for (int i = 0; i < b.Length; i++)
        {
            pos[b[i].x, b[i].y].free = false;
        }
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
    }

    public IEnumerator FindPath(float t, int _fX, int _fY, int _tX, int _tY)
    {
        fX = _fX;
        fY = _fY;
        tX = _tX;
        tY = _tY;

        BlockTiles(GameObject.Find("CTRL").GetComponent<ctrl>().block);

        // first we set the neighbors
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                //pos[i, j].SetNeighbors(GetNeighbors(pos[i, j], pos[fX, fY], pos[tX, tY]));
            }
        }

        // set list of tiles to be evaluated
        openTiles = new List<Tile>();
        // set list of tiles already evaluated
        closedTiles = new List<Tile>();

        // add current node to the open list
        AddTileToOpen(pos[fX, fY]);

        // 
        while (openTiles.Count > 0)
        {
            // current tile = tile with the lowest f cost in the open set (start off with only 1 tile in the open tiles list so that is the lowest)
            current = GetLowestFCost();

            // if current == target, then we're done
            if (current == pos[tX, tY])
            {
                Debug.Log("AT END!");

                    thee._path.Add(current);

                while (current != pos[fX, fY])
                {
                    thee._path.Add(current.parent);
                    current = current.parent;
                }

                break;
            }

            // if not then well Process this tile for exploration
            ProcessTile(current);

            yield return new WaitForSeconds(t);
        }
    }

    void AddTileToOpen(Tile t)
    {
        openTiles.Add(t);
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

        Tile n = pos[current.x - 1, current.y];
        // set our neighbor's fcost
        if (current.x - 1 >= 0 && !closedTiles.Contains(n) && pos[current.x - 1, current.y].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
            //Debug.Log($"NEI: {n.dist} - {n.fCost}");
        }

        n = pos[current.x, current.y - 1];
        // 
        if (current.y - 1 >= 0 && !closedTiles.Contains(n) && pos[current.x, current.y - 1].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
            //Debug.Log($"NEI: {n.dist} - {n.fCost}");
        }

        n = pos[current.x + 1, current.y];
        // 
        if (current.x + 1 < mapSize && !closedTiles.Contains(n) && pos[current.x + 1, current.y].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
            //Debug.Log($"NEI: {n.dist} - {n.fCost}");
        }

        n = pos[current.x, current.y + 1];
        // 
        if (current.y + 1 < mapSize && !closedTiles.Contains(n) && pos[current.x, current.y + 1].free)
        {
            gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
            hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

            n.SetGnH(gc, hc);
            n.SetParent(current);
            AddTileToOpen(n);
            ret.Add(n);
            //Debug.Log($"NEI: {n.dist} - {n.fCost}");
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

public class Tile
{
    //public Tile[] neighbor = new Tile[4];
    public bool free = true;

    public int x => _x;
    public int y => _y;
    public float fCost => gCost + hCost;
    public string loc => $"{x}, {y}";
    public string dist => $"{gCost}, {hCost}";
    public List<Tile> neighbors => _neighbors;
    public float gCost => _gCost;
    public float hCost => _hCost; // g = dist from start pos; h = dis from end;
    public Tile parent => _parent;

    Tile _parent;
    List<Tile> _neighbors;
    bool open, closed;
    int _x, _y;
    float _gCost, _hCost;

    public Tile(int X, int Y)
    {
        _x = X;
        _y = Y;
    }

    public void SetNeighbors(List<Tile> N)
    {
        _neighbors = N;
    }

    public void SetGnH(float g, float h)
    {
        _gCost = g;
        _hCost = h;
    }

    public void SetParent(Tile p)
    {
        _parent = p;
    }
}

public struct TilePath
{
    public List<Tile> _path;

    public TilePath(List<Tile> p)
    {
        _path = p;
    }
}

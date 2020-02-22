using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    //public Tile[] neighbor = new Tile[4];
    public bool free = true;

    public int x => _x;
    public int y => _y;
    public int eX => _eX;
    public int eY => _eY;
    public float fCost => gCost + hCost;
    public string loc => $"{x}, {y}";
    public string dist => $"{gCost}, {hCost}";
    public List<Tile> neighbors => _neighbors;
    public float gCost => _gCost; // g = dist from start pos
    public float hCost => _hCost; // h = dis from end
    public Tile parent => _parent;
    public Vector2Int v2Int => new Vector2Int(x, y);
    public Vector2Int expression => new Vector2Int(eX, eY);

    GameObject _obj;
    Renderer _rend;

    Tile _parent;
    List<Tile> _neighbors;
    bool open, closed;
    int _x, _y; // raw x & y
    int _eX, _eY; // expression x & y, modified by the map
    float _gCost, _hCost;
    char _type;
    public char type => _type;

    public Tile(int X, int Y, char type = 'o', int halfMapSize = 0)
    {
        _x = X;
        _y = Y;
        _type = type;

        _eX = X - halfMapSize;
        _eY = Y - halfMapSize;

        _obj = GameObject.Instantiate(Resources.Load<GameObject>("Objs/Tile"));
        _obj.transform.position = new Vector3(eX, .05f, eY);
        _obj.transform.localScale = Vector3.one * .95f;

        _rend = _obj.GetComponent<Renderer>();

        _obj.SetActive(false);
        _obj.transform.SetParent(GM.tilesParent);
    }

    public void ToggleRender(bool t, Color c)
    {
        if (type == 'p' || type == 'w')
        {
            _obj.SetActive(true);
            ChangeTileColor(Color.red);
        }
        else
        {
            _obj.SetActive(t);
            ChangeTileColor(c);
        }
    }

    void ChangeTileColor(Color c)
    {
        _rend.material.SetColor("Tile_Color", c);
    }

    public void AssignType(char t)
    {
        _type = t;
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

public class TilePath
{
    List<Tile> _path;
    public List<Tile> path => _path;
    public int dist => path.Count;

    public TilePath(List<Tile> p)
    {
        _path = p;
    }
}

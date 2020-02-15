using System.Collections.Generic;
using System.IO;
using UnityEngine;

class FightCTRL : MonoBehaviour
{
    public string levelName;
    [Range(0, 1)]
    public float dist = 1;
    [Range(-1, 1)]
    public float angleX = 0, angleY = 0;
    public GameObject one, two;

    int _time;
    public int time => _time;

    int _turn;
    public int turn => _turn % 2;

    public int angleSelect;
    Map map;
    List<Tile> selLoc = new List<Tile>();
    List<Tile> loc = new List<Tile>();
    TilePath p;
    Fighter[] fighter = new Fighter[2];


    void Start()
    {
        map = new Map(levelName);

        fighter[0] = new Fighter(one, map.mapSize);
        fighter[1] = new Fighter(two, map.mapSize);

        map.SetFighters(fighter);

        foreach (Tile ve in map.loc)
        {
            if (!ve.free)
            {
                Color c = ve.type == 'w' ? new Color(.75f, .25f, 0) : ve.type == 'p' ? (Color.yellow + Color.white) / 2 : Color.black;
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                g.transform.position = new Vector3(ve.x - map.halfMapSize, 1.5f, ve.y - map.halfMapSize);
                g.GetComponent<Renderer>().material.SetColor("_BaseColor", c);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < map.loc.GetLength(0); i++)
            {
                for (int j = 0; j < map.loc.GetLength(0); j++)
                {
                    map.loc[i, j].ToggleRender(false, Color.clear);
                }
            }

            (List<Tile> loc, List<Tile> selLoc, TilePath path, int angleSelect) outp = Map.OutputLocation(map, fighter[0].expression, fighter[1].expression, dist, angleX, angleY);

            p = outp.path;
            loc = outp.loc;
            selLoc = outp.selLoc;
            angleSelect = outp.angleSelect;
        }

        // 
        for (int i = 0; i < loc.Count; i++)
        {
            loc[i].ToggleRender(true, Color.blue);
        }

        // 
        for (int i = 0; i < selLoc.Count; i++)
        {
            selLoc[i].ToggleRender(true, i == angleSelect ? Color.yellow : Color.cyan);
        }

        // 
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            fighter[0].obj.transform.position += Vector3.forward;
        }

        // 
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            fighter[0].obj.transform.position -= Vector3.forward;
        }

        // 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            fighter[0].obj.transform.position += Vector3.right;
        }

        // 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            fighter[0].obj.transform.position -= Vector3.right;
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (p != null)
            {
                for (int i = 0; i < p.path.Count; i++)
                {
                    Gizmos.color = i == p.path.Count - 1 ? Color.yellow : Color.white;
                    Gizmos.DrawWireCube(new Vector3(p.path[i].eX, 1, p.path[i].eY), Vector3.one);
                }
            }

            foreach (Tile ve in map.loc)
            {
                if (!ve.free)
                {
                    //Gizmos.color = ve.type == 'w' ? new Color(.75f, .25f, 0) : ve.type == 'p' ? (Color.yellow + Color.white) / 2 : Color.black;
                    //Gizmos.DrawCube(new Vector3(ve.x - map.halfMapSize, 1, ve.y - map.halfMapSize), Vector3.one);
                }
            }
        }
    }
}

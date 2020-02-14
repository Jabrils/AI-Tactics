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
    public Vector2Int good, bad;

    int _time;
    public int time => _time;

    int _turn;
    public int turn => _turn % 2;

    public int angleSelect;
    Map map;
    List<Tile> selLoc = new List<Tile>();
    List<Tile> loc = new List<Tile>();
    TilePath p;

    void Start()
    {
        map = new Map(levelName);
    }

    void Update()
    {
        (List<Tile> loc, List<Tile> selLoc, TilePath path, int angleSelect) outp = Map.OutputLocation(map, good, bad, dist, angleX, angleY);

        p = outp.path;
        loc = outp.loc;
        selLoc = outp.selLoc;
        angleSelect = outp.angleSelect;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < loc.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(new Vector3(loc[i].eX, 1, loc[i].eY), Vector3.one);
            }

            for (int i = 0; i < selLoc.Count; i++)
            {
                Gizmos.color = i == angleSelect ? Color.yellow : Color.cyan;
                Gizmos.DrawCube(new Vector3(selLoc[i].eX, 1, selLoc[i].eY), Vector3.one);
            }

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
                    Gizmos.color = ve.type == 'w' ? new Color(.75f, .25f, 0) : ve.type == 'p' ? (Color.yellow + Color.white)/2 : Color.black;
                    Gizmos.DrawCube(new Vector3(ve.x - map.halfMapSize, 1, ve.y - map.halfMapSize), Vector3.one);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lMove_Concept : MonoBehaviour
{
    [Range(0, 1)]
    public float dist = 1;
    [Range(-1, 1)]
    public float angleX = 0, angleY = 0;
    public Vector2Int good, bad;
    List<Tile> loc = new List<Tile>();
    List<Tile> selLoc = new List<Tile>();
    Map map;
    TilePath p;
    int angleSelect => Map.SelectAnAngle(selLoc, angleX, angleY, bad);

    // Start is called before the first frame update
    void Start()
    {
        map = new Map(20 + 1);

        List<Vector2Int> block = new List<Vector2Int>();
        block.Add(bad);

        map.BlockTiles(block);
    }

    void Update()
    {
        Loc outp = Map.OutputLocation(map, good, bad, dist, angleX, angleY);

        p = outp.path;
        loc = outp.loc;
        selLoc = outp.selLoc;
    }

    void OnDrawGizmos()
    {
        // 
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
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(new Vector3(ve.x, 1, ve.y), Vector3.one);
                }
            }

            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(good.x - map.halfMapSize, 1, good.y - map.halfMapSize), Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(bad.x - map.halfMapSize, 1, bad.y - map.halfMapSize), Vector3.one);
        }
    }
}

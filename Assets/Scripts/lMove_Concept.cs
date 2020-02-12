using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lMove_Concept : MonoBehaviour
{
    public bool doTick;
    [Range(0,1)]
    public float dist = 1;
    public Vector2Int good, bad;
    public List<Vector2Int> block = new List<Vector2Int>();
    List<Vector2Int> pos = new List<Vector2Int>();
    List<Vector2Int> selPos = new List<Vector2Int>();
    Map map;
    TilePath p;
    int tick = 0;

    // Start is called before the first frame update
    void Start()
    {
        map = new Map(20+1);

        block = new List<Vector2Int>();
        block.Add(bad);

        map.BlockTiles(block);

        pos = map.CalcAvailbleMoves(good);

        if (doTick)
        {
            StartCoroutine(Tick());
        }
    }

    void Update()
    {
        selPos = new List<Vector2Int>();

        int calc = Mathf.RoundToInt(dist * 10 + (Map.ManhattanDistance(good, bad) - GM.maxMoves));

        for (int i = 0; i < pos.Count; i++)
        {

            if (Map.ManhattanDistance(pos[i], bad) == Mathf.Clamp(calc, 1, calc))
            {
                selPos.Add(pos[i]);
            }
        }

        //p = map.FindLimitedPath(good.x, good.y, selPos[tick].x, selPos[tick].y);
    }

    IEnumerator Tick()
    {
        yield return new WaitForSeconds(1);
        tick++;
        StartCoroutine(Tick());
    }

    void OnDrawGizmos()
    {
        // 
        for (int i = 0; i < pos.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(pos[i].x, 1, pos[i].y), Vector3.one);
        }

        for (int i = 0; i < selPos.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(selPos[i].x, 1, selPos[i].y), Vector3.one);
        }

        if (Application.isPlaying && p != null)
        {
            for (int i = 0; i < p.path.Count; i++)
            {
                Gizmos.color = i == p.path.Count - 1? Color.yellow : Color.white;
                Gizmos.DrawCube(new Vector3(p.path[i].x, 1, p.path[i].y), Vector3.one);
            }
        }

        foreach (Vector2Int ve in block)
        {
            if (ve != bad)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(ve.x, 1, ve.y), Vector3.one);
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(good.x, 1, good.y), Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(bad.x, 1, bad.y), Vector3.one);
    }
}

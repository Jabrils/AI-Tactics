using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lMove_Concept : MonoBehaviour
{
    [Range(0,1)]
    public float dist = 1, angle = 0;
    public int sel;
    public Vector2Int good, bad;
    public List<Vector2Int> block = new List<Vector2Int>();
    List<Vector2Int> pos = new List<Vector2Int>();
    List<Vector2Int> selPos = new List<Vector2Int>();
    Map map;
    TilePath p;
    int angleCount => Mathf.Clamp(Mathf.RoundToInt(angle * selPos.Count), 0, selPos.Count-1);

    // Start is called before the first frame update
    void Start()
    {
        map = new Map(20+1);

        block = new List<Vector2Int>();
        block.Add(bad);

        map.BlockTiles(block);

        pos = map.CalcAvailbleMoves(good);
    }

    void Update()
    {
        // reset our selPos list every frame, this allows us to make changes to it
        selPos = new List<Vector2Int>();

        // now we calculate the selected range of tiles based on distance from the opponent
        int calc = Mathf.RoundToInt(dist * 10 + (Map.ManhattanDistance(good, bad) - GM.maxMoves));

        // we need a reference to a temp all selected positions list
        List<Vector2Int> tempAllSelPos = new List<Vector2Int>();

        // next we loop through all of the positions & see who is viable
        for (int i = 0; i < pos.Count; i++)
        {
            if (Map.ManhattanDistance(pos[i], bad) == Mathf.Clamp(calc, 1, calc))
            {
                tempAllSelPos.Add(pos[i]);
            }
        }

        // then we loop through the viable ones to grab only the top ones
        for (int i = 0; i < tempAllSelPos.Count; i++)
        {
        if (tempAllSelPos[i].y >= bad.y)
            {
                selPos.Add(tempAllSelPos[i]);
            }
        }
        // we have to reverse the list so that it is counter clockwise, this makes it according to radians
        selPos.Reverse();

        // then we loop through the viable ones to grab only the bottom ones
        for (int i = 0; i < tempAllSelPos.Count; i++)
        {
            if (tempAllSelPos[i].y < bad.y)
            {
                selPos.Add(tempAllSelPos[i]);
            }
        }

        p = map.FindLimitedPath(good.x, good.y, selPos[angleCount].x, selPos[angleCount].y);
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
            Gizmos.color = i == angleCount ? Color.yellow : Color.cyan;
            Gizmos.DrawCube(new Vector3(selPos[i].x, 1, selPos[i].y), Vector3.one);
        }

        if (Application.isPlaying && p != null)
        {
            for (int i = 0; i < p.path.Count; i++)
            {
                Gizmos.color = i == p.path.Count - 1? Color.yellow : Color.white;
                Gizmos.DrawWireCube(new Vector3(p.path[i].x, 1, p.path[i].y), Vector3.one);
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

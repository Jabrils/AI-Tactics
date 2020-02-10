using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrl : MonoBehaviour
{
    public float time;
    public Vector2Int from, to;
    public Vector2Int[] block;
    Map map;

    // Start is called before the first frame update
    void Start()
    {
        map = new Map(20);
        map.Assign();

        StartCoroutine(map.FindPath(time, from.x, from.y, to.x, to.y));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            // 
            for (int i = 0; i < map.openTiles.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(map.openTiles[i].x, 1, map.openTiles[i].y), Vector3.one);
            }

            Gizmos.color = Color.blue;

            // 
            for (int i = 0; i < map.closedTiles.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(map.closedTiles[i].x, 1, map.closedTiles[i].y), Vector3.one);
            }

            Gizmos.color = Color.cyan;

            for (int i = 0; i < map.thee._path.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(map.closedTiles[i].x, 1, map.closedTiles[i].y), Vector3.one);
            }

        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(from.x, 1, from.y), Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(to.x, 1, to.y), Vector3.one);

        foreach (Vector2Int ve in block)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(new Vector3(ve.x, 1, ve.y), Vector3.one);
        }
    }
}

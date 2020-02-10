using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ctrl : MonoBehaviour
{
    public bool randomize;
    public int mapSize;
    public Vector2Int from, to;
    public List<Vector2Int> block;
    Map map;

    // Start is called before the first frame update
    void Start()
    {
        GameObject g = GameObject.Find("ground");
        g.transform.localScale = new Vector3(mapSize, 1, mapSize);
        g.transform.position = new Vector3(mapSize / 2, 0, mapSize / 2);

        Camera.main.transform.position = new Vector3(mapSize / 2, mapSize, mapSize / 2);

        map = new Map(mapSize+1);
        map.Assign();

        if (randomize)
        {
            from = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
            to = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));

            block = new List<Vector2Int>();

            for (int i = 0; i < mapSize * 5; i++)
            {
                block.Add(new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize)));

                // 
                if (block[i].x == to.x && block[i].y == to.y || block[i].x == from.x && block[i].y == from.y)
                {
                    block[i] = new Vector2Int(0, 0);
                }
            }
        }

        TilePath p = map.FindPath(from.x, from.y, to.x, to.y);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            to = new Vector2Int(Random.Range(0+1, mapSize), Random.Range(0+1, mapSize));

            while (block.Contains(to))
            {
                to = new Vector2Int(Random.Range(0 + 1, mapSize), Random.Range(0 + 1, mapSize));
            }

            TilePath p = map.FindPath(from.x, from.y, to.x, to.y);
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && map.openTiles != null)
        {

            // 
            for (int i = 0; i < map.openTiles.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(new Vector3(map.openTiles[i].x, 1, map.openTiles[i].y), Vector3.one);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(new Vector3(map.openTiles[i].x, 2, map.openTiles[i].y), new Vector3(map.openTiles[i].parent.x, 2, map.openTiles[i].parent.y));
            }


            // 
            for (int i = 0; i < map.closedTiles.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(new Vector3(map.closedTiles[i].x, 1, map.closedTiles[i].y), Vector3.one);
                Gizmos.color = Color.white;

                if (map.closedTiles[i].parent != null)
                {
                    Gizmos.DrawLine(new Vector3(map.closedTiles[i].x, 2, map.closedTiles[i].y), new Vector3(map.closedTiles[i].parent.x, 2, map.closedTiles[i].parent.y));
                }
            }

            // 
            for (int i = 0; i < map.thee._path.Count; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(new Vector3(map.thee._path[i].x, 1, map.thee._path[i].y), Vector3.one);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrl : MonoBehaviour
{
    public Vector2Int from, to;
    Map map;

    // Start is called before the first frame update
    void Start()
    {
        map = new Map(20);
        map.Assign();
        map.FindPath(from.x, from.y, to.x, to.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(from.x, 1, from.y), Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(to.x, 1, to.y), Vector3.one);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            // 
            for (int i = 0; i < map.a.neighbors.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(map.a.neighbors[i].x, 1, map.a.neighbors[i].y), Vector3.one);
            }
        }
    }
}

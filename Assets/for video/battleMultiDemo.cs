using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleMultiDemo : MonoBehaviour
{
    int[] side = new int[] { -10, 9 };
    Map map;

    // Start is called before the first frame update
    void Start()
    {
        SpawnHB(new Vector3(0, .5f, side[0]));
        SpawnHB(new Vector3(-1, .5f, side[0]));
        SpawnHB(new Vector3(-2, .5f, side[0]));

        SpawnHB(new Vector3(1, .5f, side[1]));
        SpawnHB(new Vector3(0, .5f, side[1]));
        SpawnHB(new Vector3(-1, .5f, side[1]));

        map = new Map(20);

        map.FindPath(0, 10, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnHB(Vector3 pos)
    {
        GameObject hb = Instantiate(Resources.Load<GameObject>("Objs/Haxbot"));
        hb.transform.position = pos;
    }
}

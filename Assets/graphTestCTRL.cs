using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class graphTestCTRL : MonoBehaviour
{
    public int val = 1;
    public GameObject mast;
    Graph graph;
    // Start is called before the first frame update
    void Start()
    {
        graph = new Graph(mast);

        graph.UpdateGraph();

        StartCoroutine(Up());
    }

    IEnumerator Up()
    {
        yield return new WaitForSeconds(.1f);

        graph.AddValueToGraph(Random.Range(-11,11));
        graph.UpdateGraph();
        print(graph.count);

        StartCoroutine(Up());
    }

    // Update is called once per frame
    void Update()
    {

    }
}

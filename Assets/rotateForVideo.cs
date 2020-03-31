using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateForVideo : MonoBehaviour
{
    public bool flip;
    float off;

    // Start is called before the first frame update
    void Start()
    {
        off = Random.value + .5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, (flip ? 180 : 0) + Mathf.Cos(Time.time * off)*25, 0);
    }
}

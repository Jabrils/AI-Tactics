using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {
    public bool x;
    public bool y = true;
    public bool z;
    public Vector3 roSpeed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.eulerAngles += (new Vector3(x ? roSpeed.x : 0, y ? roSpeed.y : 0, z ? roSpeed.z : 0));
	}
}
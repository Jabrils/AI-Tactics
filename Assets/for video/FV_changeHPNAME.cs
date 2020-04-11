using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// FOR VIDEO PURPOSES ONLY
public class FV_changeHPNAME : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //print(GetComponentsInChildren<TextMeshPro>()[1].name);
        GetComponentsInChildren<TextMeshPro>()[1].text = name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

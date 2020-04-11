using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickLoadHaxbotTexture : MonoBehaviour
{
    public string who;

    // Start is called before the first frame update
    void Awake()
    {
        QuickLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void QuickLoad()
    {
        HaxbotData hbD = HXB.LoadHaxbot(gameObject, who);

        // 
        for (int j = 0; j < 2; j++)
        {
            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.material.SetTexture(GM.mainTexture, hbD.txt2d);
            }
        }
    }
}

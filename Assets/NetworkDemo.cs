using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class NetworkDemo : MonoBehaviour
{
    public bool rand;
    GameObject canv;
    RectTransform canvRT;

    // Start is called before the first frame update
    void Start()
    {
        //Random.seed = 29092966;
        canv = GameObject.Find("bucket");
        canvRT = canv.GetComponent<RectTransform>();

        int inp = 20, hid = 12, outp = 3;

        float[] state = new float[inp];
        float[][] hidden = new float[hid][];
        float[][] outs = new float[outp][];

        float[] hOut = new float[hid];
        float[] oOut = new float[outp];
        float[] fin = new float[outp];

        // 
        for (int i = 0; i < state.Length; i++)
        {
            state[i] = Random.Range(-1f, 1f);
        }

        // 
        for (int i = 0; i < hidden.Length; i++)
        {
            hidden[i] = new float[inp];

            for (int j = 0; j < hidden[i].Length; j++)
            {
                hidden[i][j] = 1 * (rand ? Random.value : 1);

                hOut[i] += hidden[i][j] * state[j];
            }
        }

        // 
        for (int i = 0; i < outs.Length; i++)
        {
            outs[i] = new float[hid];

            for (int j = 0; j < outs[i].Length; j++)
            {
                outs[i][j] = 1 * (rand ? Random.value : 1);

                oOut[i] += outs[i][j] * hOut[j];
            }

            fin[i] = AI.Sigmoid(oOut[i]);
        }

        // inputs
        for (int i = 0; i < inp; i++)
        {
            int size = 50;

            GameObject add = Instantiate(Resources.Load<GameObject>("Demo/node"));
            add.transform.SetParent(canv.transform);

            RectTransform t = add.GetComponent<RectTransform>();
            t.sizeDelta = Vector2.one * size;

            t.localPosition = new Vector2(-900, ((inp * size) / 2) - (i * size));

            // 
            GameObject w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            TextMeshProUGUI wTxt = w.GetComponent<TextMeshProUGUI>();
            wTxt.text = $"{(Mathf.Round(state[i] * 100) / 100)}";
            wTxt.fontSizeMax = 17;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;
        }


        // hidden
        for (int i = 0; i < hid; i++)
        {
            int size = 70, space = 10;

            GameObject add = Instantiate(Resources.Load<GameObject>("Demo/node"));
            add.transform.SetParent(canv.transform);

            RectTransform t = add.GetComponent<RectTransform>();
            t.sizeDelta = Vector2.one * size;

            float startLoc = (hid * size) / 2;

            t.localPosition = new Vector2(-450, ((startLoc) - (i * (size + space))));

            Vector2 p = t.localPosition;

            // 
            GameObject w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            TextMeshProUGUI wTxt = w.GetComponent<TextMeshProUGUI>();
            float show = hOut[i];
            wTxt.text = $"{(Mathf.Round(show * 100) / 100)}";
            wTxt.fontSizeMax = 17;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;

            // 
            for (int j = 0; j < inp; j++)
            {
                int size2 = 7;
                add = Instantiate(Resources.Load<GameObject>("Demo/node"));
                add.transform.SetParent(canv.transform);

                t = add.GetComponent<RectTransform>();
                t.sizeDelta = Vector2.one * size2;

                float r = size / 2;
                float place = (-j / Mathf.PI) / 2;

                t.localPosition = new Vector2((Mathf.Sin(place) * r) + p.x, (Mathf.Cos(place) * r) + p.y);

                w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
                w.transform.SetParent(add.transform);
                wTxt = w.GetComponent<TextMeshProUGUI>();
                wTxt.text = $"{(Mathf.Round(hidden[i][j] * 100) / 100)}";

                wRT = w.GetComponent<RectTransform>();
                wRT.localPosition = Vector2.zero;
            }
        }


        // out
        for (int i = 0; i < outp; i++)
        {
            int size = 200, space = 100;

            GameObject add = Instantiate(Resources.Load<GameObject>("Demo/node"));
            add.transform.SetParent(canv.transform);

            RectTransform t = add.GetComponent<RectTransform>();
            t.sizeDelta = Vector2.one * size;

            float startLoc = (outp * size) / 2;

            t.localPosition = new Vector2(0, ((startLoc) - (i * (size + space))));

            Vector2 p = t.localPosition;

            // 
            GameObject w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            TextMeshProUGUI wTxt = w.GetComponent<TextMeshProUGUI>();
            float show = oOut[i];
            wTxt.text = $"{(Mathf.Round(show * 100) / 100)}";
            wTxt.fontSizeMax = 30;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;

            // 
            for (int j = 0; j < hid; j++)
            {
                int size2 = 30;
                add = Instantiate(Resources.Load<GameObject>("Demo/node"));
                add.transform.SetParent(canv.transform);

                t = add.GetComponent<RectTransform>();
                t.sizeDelta = Vector2.one * size2;

                float r = size / 2;
                float place = -j / Mathf.PI;

                t.localPosition = new Vector2((Mathf.Sin(place) * r) + p.x, (Mathf.Cos(place) * r) + p.y);

                w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
                w.transform.SetParent(add.transform);
                wTxt = w.GetComponent<TextMeshProUGUI>();
                wTxt.text = $"{(Mathf.Round(outs[i][j] * 100) / 100)}";
                wTxt.fontSizeMax = 18;

                wRT = w.GetComponent<RectTransform>();
                wRT.localPosition = Vector2.zero;
            }
        }

        // final
        for (int i = 0; i < outp; i++)
        {
            int size = 200, space = 100;

            GameObject add = Instantiate(Resources.Load<GameObject>("Demo/node"));
            add.transform.SetParent(canv.transform);

            RectTransform t = add.GetComponent<RectTransform>();
            t.sizeDelta = Vector2.one * size;

            float startLoc = (outp * size) / 2;

            t.localPosition = new Vector2(450, ((startLoc) - (i * (size + space))));

            // 
            GameObject w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            TextMeshProUGUI wTxt = w.GetComponent<TextMeshProUGUI>();
            float show = fin[i];
            wTxt.text = $"{(Mathf.Round(show * 100) / 100)}";
            wTxt.fontSizeMax = 30;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            canv.transform.localScale += Vector3.one * .05f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            canv.transform.localScale -= Vector3.one * .05f;
        }

        float move = 10;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            canvRT.localPosition -= Vector3.right * move;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            canvRT.localPosition += Vector3.right * move;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            canvRT.localPosition -= Vector3.up * move;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            canvRT.localPosition += Vector3.up * move;
        }
    }
}

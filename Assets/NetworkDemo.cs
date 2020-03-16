using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.IO;

public class NetworkDemo : MonoBehaviour
{
    public bool randWeights;
    public float lR = .01f;
    GameObject canv;
    RectTransform canvRT, pointTo;
    public int oppDecide = -1;
    int inp = 20, hid = 12, outp = 3;
    float[] state;
    float[][] W_hidden;
    float[][] W_out;
    float[] hOut;
    float[] oOut;
    float[] fin;
    float[] derivF;
    float[] derivO;
    float[] derivH;
    float[][] D_out;
    float[][] D_hidden;
    TextMeshProUGUI[] inpTxt, hLTxt, hLDTxt, oTxt, oDTxt, fTxt, fDTxt, rTxt, rDTxt;
    TextMeshProUGUI[][] hLWTxt, hLWDTxt, oWTxt, oWDTxt;
    int dp, decided = -1;
    List<string[]> data = new List<string[]>(), dataOG = new List<string[]>();
    int[] reward = new int[3];
    float[] error = new float[3];

    // Start is called before the first frame update
    void Start()
    {
        //Random.seed = 29092960;
        canv = GameObject.Find("bucket");
        canvRT = canv.GetComponent<RectTransform>();

        // decide
        GameObject pointGO = Instantiate(Resources.Load<GameObject>("Demo/node"));
        pointGO.transform.SetParent(canv.transform);

        pointGO.GetComponent<Image>().color = Color.yellow;
        pointTo = pointGO.GetComponent<RectTransform>();

        LoadDataset();

        // 

        state = new float[inp];
        W_hidden = new float[hid][];
        W_out = new float[outp][];

        hOut = new float[hid];
        oOut = new float[outp];
        fin = new float[outp];

        derivF = new float[outp];
        derivO = new float[outp];
        derivH = new float[hid];

        D_out = new float[outp][];
        D_hidden = new float[hid][];

        inpTxt = new TextMeshProUGUI[inp];

        hLTxt = new TextMeshProUGUI[hid];
        hLDTxt = new TextMeshProUGUI[hid];
        oTxt = new TextMeshProUGUI[outp];
        oDTxt = new TextMeshProUGUI[outp];
        fTxt = new TextMeshProUGUI[outp];
        rTxt = new TextMeshProUGUI[outp];
        fDTxt = new TextMeshProUGUI[outp];
        rDTxt = new TextMeshProUGUI[outp];

        hLWTxt = new TextMeshProUGUI[hid][];
        hLWDTxt = new TextMeshProUGUI[hid][];
        oWTxt = new TextMeshProUGUI[outp][];
        oWDTxt = new TextMeshProUGUI[outp][];

        for (int i = 0; i < hLWTxt.Length; i++)
        {
            hLWTxt[i] = new TextMeshProUGUI[inp];
            hLWDTxt[i] = new TextMeshProUGUI[inp];
        }

        for (int i = 0; i < oWTxt.Length; i++)
        {
            oWTxt[i] = new TextMeshProUGUI[hid];
            oWDTxt[i] = new TextMeshProUGUI[hid];
        }

        // 
        for (int i = 0; i < W_hidden.Length; i++)
        {
            W_hidden[i] = new float[inp];

            for (int j = 0; j < W_hidden[i].Length; j++)
            {
                float dice = Random.value;
                W_hidden[i][j] = 1 * (randWeights ? Random.Range(-1f, 1f) : dice > .5 ? 1 : -1);

                hOut[i] += W_hidden[i][j] * state[j];
            }

            // 
            D_hidden[i] = new float[inp];
        }

        // 
        for (int i = 0; i < W_out.Length; i++)
        {
            // 
            W_out[i] = new float[hid];

            // 
            for (int j = 0; j < W_out[i].Length; j++)
            {
                float dice = Random.value;
                W_out[i][j] = 1 * (randWeights ? Random.Range(-1f, 1f) : dice > .5 ? 1 : -1);

                oOut[i] += W_out[i][j] * hOut[j];
            }

            // 
            D_out[i] = new float[hid];
        }

        ForwardPass(data, true);

        //                     //
        // //               // //
        // // // VIS-UAL // // //
        // //               // //
        //                     //

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
            inpTxt[i] = w.GetComponent<TextMeshProUGUI>();
            inpTxt[i].fontSizeMax = 17;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;
        }

        //UpdateInpTxt();

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
            hLTxt[i] = w.GetComponent<TextMeshProUGUI>();
            hLTxt[i].fontSizeMax = 17;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;

            // deriv
            w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            hLDTxt[i] = w.GetComponent<TextMeshProUGUI>();
            hLDTxt[i].fontSizeMax = 10;

            wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.right * (size / 2);

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
                hLWTxt[i][j] = w.GetComponent<TextMeshProUGUI>();

                wRT = w.GetComponent<RectTransform>();
                wRT.localPosition = Vector2.zero;

                // deriv
                w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
                w.transform.SetParent(add.transform);
                hLWDTxt[i][j] = w.GetComponent<TextMeshProUGUI>();
                hLWDTxt[i][j].fontSizeMax = 2;

                wRT = w.GetComponent<RectTransform>();
                wRT.localPosition = Vector2.up * (size2 / 2);
            }
        }

        //UpdateHLTxt();

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
            oTxt[i] = w.GetComponent<TextMeshProUGUI>();
            float show = oOut[i];
            oTxt[i].text = $"{(Mathf.Round(show * 100) / 100)}";
            oTxt[i].fontSizeMax = 30;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;

            // deriv
            w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            oDTxt[i] = w.GetComponent<TextMeshProUGUI>();
            show = derivO[i];
            oDTxt[i].text = $"{(Mathf.Round(show * 1000) / 1000)}";
            oDTxt[i].fontSizeMax = 30;

            wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.right * (size / 2);

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
                oWTxt[i][j] = w.GetComponent<TextMeshProUGUI>();
                oWTxt[i][j].text = $"{(Mathf.Round(W_out[i][j] * 100) / 100)}";
                oWTxt[i][j].fontSizeMax = 18;

                wRT = w.GetComponent<RectTransform>();
                wRT.localPosition = Vector2.zero;

                // deriv
                w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
                w.transform.SetParent(add.transform);
                oWDTxt[i][j] = w.GetComponent<TextMeshProUGUI>();
                show = D_out[i][j];
                oWDTxt[i][j].text = $"{(Mathf.Round(show * 1000) / 1000)}";
                oWDTxt[i][j].fontSizeMax = 10;

                wRT = w.GetComponent<RectTransform>();
                wRT.localPosition = Vector2.up * (size2 / 2);
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

            t.localPosition = new Vector2(300, ((startLoc) - (i * (size + space))));

            // weight
            GameObject w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            fTxt[i] = w.GetComponent<TextMeshProUGUI>();
            fTxt[i].fontSizeMax = 30;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;

            // deriv
            w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            fDTxt[i] = w.GetComponent<TextMeshProUGUI>();
            fDTxt[i].fontSizeMax = 30;

            wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.up * (size / 2);
        }

        // reward
        for (int i = 0; i < outp; i++)
        {
            int size = 200, space = 100;

            GameObject add = Instantiate(Resources.Load<GameObject>("Demo/node"));
            add.transform.SetParent(canv.transform);

            RectTransform t = add.GetComponent<RectTransform>();
            t.sizeDelta = Vector2.one * size;

            float startLoc = (outp * size) / 2;

            t.localPosition = new Vector2(550, ((startLoc) - (i * (size + space))));

            // weight
            GameObject w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            rTxt[i] = w.GetComponent<TextMeshProUGUI>();
            rTxt[i].fontSizeMax = 30;

            RectTransform wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.zero;

            // error
            w = Instantiate(Resources.Load<GameObject>("Demo/weight"));
            w.transform.SetParent(add.transform);
            rDTxt[i] = w.GetComponent<TextMeshProUGUI>();
            rDTxt[i].fontSizeMax = 30;

            wRT = w.GetComponent<RectTransform>();
            wRT.localPosition = Vector2.up * (size / 2);
        }

        // decide

        int size3 = 150;

        pointTo.sizeDelta = Vector2.one * size3;

        float startLoc2 = (outp * size3) / 2;

        pointTo.localPosition = new Vector2(700, ((startLoc2) - (decided)));

        UpdateAllText();
    }

    void UpdateHLTxt()
    {
        for (int i = 0; i < hLTxt.Length; i++)
        {
            float show = hOut[i];
            hLTxt[i].text = $"{(Mathf.Round(show * 100) / 100)}";
            show = derivH[i];
            hLDTxt[i].text = $"{(Mathf.Round(show * 1000) / 1000)}";

            for (int j = 0; j < hLWTxt[i].Length; j++)
            {
                hLWTxt[i][j].text = $"{(Mathf.Round(W_hidden[i][j] * 100) / 100)}";
                show = D_hidden[i][j];
                hLWDTxt[i][j].text = $"{(Mathf.Round(show * 1000) / 1000)}";
            }
        }
    }

    void UpdateOutTxt()
    {
        for (int i = 0; i < oTxt.Length; i++)
        {
            float show = oOut[i];
            oTxt[i].text = $"{(Mathf.Round(show * 100) / 100)}";
            show = derivO[i];
            oDTxt[i].text = $"{(Mathf.Round(show * 1000) / 1000)}";

            show = reward[i];
            rTxt[i].text = "" + show;
            rDTxt[i].text = $"{error[i]}";

            show = fin[i];
            fTxt[i].text = $"{(Mathf.Round(show * 10000) / 10000)}";

            show = derivF[i];
            fDTxt[i].text = $"{(Mathf.Round(show * 100) / 100)}";

            for (int j = 0; j < oWTxt[i].Length; j++)
            {
                oWTxt[i][j].text = $"{(Mathf.Round(W_out[i][j] * 100) / 100)}";
                show = D_out[i][j];
                oWDTxt[i][j].text = $"{(Mathf.Round(show * 1000) / 1000)}";
            }
        }

        Vector2[] forFucksSake = new Vector2[] { new Vector2(700, 300), new Vector2(700, 0), new Vector2(700, -300) };

        pointTo.localPosition = forFucksSake[decided] + (Vector2.right * 100);
    }

    void UpdateInpTxt()
    {
        for (int i = 0; i < inpTxt.Length; i++)
        {
            inpTxt[i].text = $"{(Mathf.Round(state[i] * 100) / 100)}";
        }
    }

    void ForwardPass(List<string[]> data, bool newInp = false)
    {
        if (newInp)
        {
            int dp = Random.Range(0, data.Count);

            string[] r = data[dp][2].Split(',');

            // 
            for (int i = 0; i < reward.Length; i++)
            {
                reward[i] = int.Parse(r[i]);
            }

            string[] newInpTemp = data[dp][0].Split(',');

            for (int i = 0; i < state.Length; i++)
            {
                state[i] = float.Parse(newInpTemp[i]);
            }
        }

        // 
        for (int i = 0; i < W_hidden.Length; i++)
        {
            hOut[i] = 0;

            for (int j = 0; j < W_hidden[i].Length; j++)
            {

                hOut[i] += W_hidden[i][j] * state[j];
            }

            // 
            D_hidden[i] = new float[inp];
        }

        // 
        for (int i = 0; i < W_out.Length; i++)
        {
            oOut[i] = 0;

            // 
            for (int j = 0; j < W_out[i].Length; j++)
            {
                oOut[i] += W_out[i][j] * hOut[j];
            }

            // 
            fin[i] = AI.Sigmoid(oOut[i]);

            // 
            D_out[i] = new float[hid];
        }

        float max = 0;

        // Highest score wins
        for (int i = 0; i < fin.Length; i++)
        {
            if (fin[i] > max)
            {
                max = fin[i];
                decided = i;
            }
        }

        BackProp();
    }

    void BackProp()
    {
        for (int i = 0; i < 3; i++)
        {
            // back prop
            error[i] = lR * Mathf.Pow(reward[i] * fin[i], 2);

            derivF[i] = 2 * (lR * Mathf.Pow(reward[i], 2)) * fin[i];

            // 
            derivO[i] = AI.Sigmoid(oOut[i], true) * derivF[i];

            //
            for (int j = 0; j < D_out[i].Length; j++)
            {
                D_out[i][j] = hOut[j] * derivO[i];

                derivH[j] = W_out[i][j] * derivO[i];

                // 
                for (int k = 0; k < D_hidden[j].Length; k++)
                {
                    D_hidden[j][k] = state[k] * derivH[j];
                    W_hidden[j][k] += D_hidden[j][k] * Mathf.Sign(reward[i]);
                }

                W_out[i][j] += D_out[i][j] * Mathf.Sign(reward[i]);
            }
        }
    }

    private void LoadDataset()
    {
        string dataset = "";

        // 
        using (StreamReader sR = new StreamReader("masterLog.tsv"))
        {
            dataset = sR.ReadToEnd();
        }

        string[] line = dataset.Split('\n');

        // 
        for (int i = 0; i < line.Length; i++)
        {
            data.Add(line[i].Split('\t'));
        }

        dataOG = new List<string[]>(data);
    }

    void UpdateAllText()
    {
        UpdateInpTxt();
        UpdateHLTxt();
        UpdateOutTxt();
    }

    // Update is called once per frame
    void Update()
    {
        //if (data.Count > 94)
        //{
        ForwardPass(data, true);
        UpdateAllText();
        //}

        if (Input.GetKey(KeyCode.W))
        {
            canv.transform.localScale += Vector3.one * .05f;
        }

        if (Input.GetKey(KeyCode.Q))
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ForwardPass(data);
            UpdateAllText();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            W_out[0][0] -= .1f;
            ForwardPass(data);
            UpdateAllText();
        }


        if (Input.GetKeyDown(KeyCode.Equals))
        {
            ForwardPass(dataOG, false);
            UpdateAllText();
        }
    }
}

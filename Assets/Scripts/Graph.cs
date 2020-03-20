using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct Graph
{
    public List<GraphObj> objs;
    public int max;
    public float count => objs.Count;
    public GameObject self;
    bool accumulative;
    public float highest;
    public float performance;

    public Graph(GameObject self, bool accumulative = false)
    {
        this.self = self;
        this.max = 1;
        this.accumulative = accumulative;
        this.highest = 0;
        this.performance = 0;

        objs = new List<GraphObj>();
    }

    public void UpdateGraph()
    {
        float xMax = 500;

        for (int i = 0; i < count; i++)
        {
            GraphObj gO = objs[i];

            float theFig = (1 - ((float)gO.inp / max)) * 100;

            objs[i].self.rectTransform.offsetMin = new Vector2((i / (float)count) * xMax + 1, gO.isPos ? 100 : theFig);
            objs[i].self.rectTransform.offsetMax = new Vector2(-((xMax+1) - ((xMax) / count) * (i + 1)), !gO.isPos ? -100 : -theFig);
        }
    }


    public void AddValueToGraph(int val)
    {
        float nVal = (objs.Count > 0 && accumulative) ? val + objs[objs.Count - 1].value : val;

        highest = nVal > highest ? nVal : highest;

        float inp = Mathf.Abs(nVal);
        bool isPos = Mathf.Sign(nVal) == 1;

        max = inp > max ? (int)inp : max;

        GameObject gO = GameObject.Instantiate(Resources.Load<GameObject>("UI/Bar"));
        gO.transform.SetParent(self.GetComponentInChildren<Image>().transform);

        Color[] col = new Color[2];

        ColorUtility.TryParseHtmlString("#b0ff8e", out col[0]);
        ColorUtility.TryParseHtmlString("#ff7e5a", out col[1]);

        Image thatBar = gO.GetComponent<Image>();
        thatBar.rectTransform.localScale = Vector3.one;
        thatBar.color = isPos ? col[0] : col[1];

        GraphObj nGraphObj = new GraphObj(thatBar, isPos, inp, nVal);

        objs.Add(nGraphObj);

        if (accumulative)
        {
            performance = nVal;
        }
        else
        {
            List<float> ret = new List<float>();

            for (int i = 0; i < objs.Count; i++)
            {
                ret.Add(objs[i].value);
            }

            performance = ret.Average();
        }
    }
}

public struct GraphObj
{
    public bool isPos;
    public float value;
    public float inp;
    public Image self;

    public GraphObj(Image self, bool isPos, float inp, float value)
    {
        this.self = self;
        this.isPos = isPos;
        this.inp = inp;
        this.value = value;
    }
}

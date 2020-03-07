using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class menuCTRL : MonoBehaviour
{
    public GameObject[] bot;
    public RectTransform[] menu;
    public TextMeshProUGUI[] name;
    public TextMeshProUGUI toolTipTxt;
    public TMP_Dropdown[] dd_Haxbot, dd_Intelli;
    public Image bCol;
    HaxbotData[] hbD;
    List<TMP_Dropdown.OptionData> dd = new List<TMP_Dropdown.OptionData>();
    bool botsExist;
    string path;
    AI_Config[] aI_Config;

    // Start is called before the first frame update
    void Start()
    {
        hbD = new HaxbotData[2];

        path = Path.Combine(Application.dataPath, "Bots");

        botsExist = Directory.Exists(path);

        // 
        if (botsExist)
        {
            // 
            string[] allBots = Directory.GetDirectories(path);

            // 
            for (int i = 0; i < dd_Haxbot.Length; i++)
            {
                dd_Haxbot[i].ClearOptions();
                dd_Intelli[i].ClearOptions();
            }

            // 
            aI_Config = Resources.LoadAll<AI_Config>("AI Configs");

            // 
            List<TMP_Dropdown.OptionData> dd2 = new List<TMP_Dropdown.OptionData>();

            // 
            for (int i = 0; i < aI_Config.Length; i++)
            {
                dd2.Add(new TMP_Dropdown.OptionData(aI_Config[i].name));
            }

            // 
            for (int i = 0; i < allBots.Length; i++)
            {
                dd.Add(new TMP_Dropdown.OptionData(allBots[i].Split('\\')[2]));
            }

            for (int i = 0; i < 2; i++)
            {
                // 
                dd_Haxbot[i].AddOptions(dd);
                //
                dd_Intelli[i].AddOptions(dd2);
                // 
                LoadNSetHaxbot(i);
            }
        }

        MenuInit();
    }

    void MenuInit()
    {
        // 
        dd_Haxbot[0].onValueChanged.AddListener(delegate { LoadNSetHaxbot(0); });
        dd_Haxbot[1].onValueChanged.AddListener(delegate { LoadNSetHaxbot(1); });

        // 
        dd_Intelli[0].onValueChanged.AddListener(delegate { SetIntelli(0); });
        dd_Intelli[1].onValueChanged.AddListener(delegate { SetIntelli(1); });

        // 
        SetIntelli(0);
        SetIntelli(1);

        // set the proper menu
        ChangeMenu(0);
    }

    void LoadNSetHaxbot(int i)
    {
        hbD[i] = HXB.LoadHaxbot(bot[i], dd[dd_Haxbot[i].value].text);
        name[i].text = hbD[i].name;
        GM.hbName[i] = hbD[i].name;
    }

    void SetIntelli(int i)
    {
        //print($"{dd_Intelli[i].value} -> {aI_Config[dd_Intelli[i].value]}");
        GM.intelli[i] = aI_Config[dd_Intelli[i].value];
    }

    // Update is called once per frame
    void Update()
    {
        bCol.color = Color.Lerp(Color.white, Color.red, Mathf.Abs(Mathf.Sin(Time.time)));
    }

    public void ChangeMenu(int w)
    {
        for (int i = 0; i < menu.Length; i++)
        {
            menu[i].gameObject.SetActive(i == w);
        }
    }

    public void GoTo(string where)
    {
        SceneManager.LoadScene(where);
    }

    public void SetToolTip(string tt)
    {
        toolTipTxt.gameObject.SetActive(tt != "");
        toolTipTxt.text = tt;
    }
}

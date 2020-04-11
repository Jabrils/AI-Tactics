using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class menuCTRL : MonoBehaviour
{
    public Image mainBG, authorsNote;
    public GameObject[] bot;
    public GameObject[] forrest;
    public RectTransform[] menu;
    public TextMeshProUGUI[] name, percentage, txtShowoff, txt_BrainSize;
    public TextMeshProUGUI toolTipTxt, vText, roundsTxt, bSpeedTxt, txt_BestOf;
    public TMP_Dropdown[] dd_Haxbot, dd_Intelli, dd_aiType;
    public Image bCol, img_BestOf;
    public RectTransform title;
    public Scrollbar[] scroll;
    public Transform[] panelSwitch;
    public Button createAI;
    public TMP_InputField brainInp;
    public Slider[] percSlider, sliShowoff, sli_BrainSize;
    public Scrollbar[] scro_Learning;
    public RectTransform msgBox;
    public Slider roundsSli, bSpeedSli;
    public GameObject flames;
    public AudioClip menuSFX, aNote;

    AudioSource menuAud;
    HaxbotData[] hbD;
    List<TMP_Dropdown.OptionData> dd = new List<TMP_Dropdown.OptionData>();
    bool brainsExist;
    string botsPath, brainsPath;
    bool[] nn_strat => new bool[] { Mathf.RoundToInt(scroll[0].value) == 1, Mathf.RoundToInt(scroll[1].value) == 1, Mathf.RoundToInt(scroll[2].value) == 1, Mathf.RoundToInt(scroll[3].value) == 1, };
    List<string> newAI_Config = new List<string>();
    int menuState;
    bool[] _isNN => new bool[] { GM.intelli[0] == null || GM.intelli[0].isHuman ? false : GM.intelli[0].usingAttackNN, GM.intelli[1] == null || GM.intelli[1].isHuman ? false : GM.intelli[1].usingAttackNN };
    bool matchHasNN => _isNN[0] || _isNN[1];
    bool authorNoting;

    // Start is called before the first frame update
    void Start()
    {
        hbD = new HaxbotData[2];
        GM.botsExist = false;

        // 
        MenuInit();

        vText.text = $"v{Application.version}";
    }

    void LoadBattleData()
    {
        botsPath = Path.Combine(Application.dataPath, "Bots");
        brainsPath = Path.Combine(Application.dataPath, "Brains");

        GM.botsExist = Directory.Exists(botsPath);
        brainsExist = Directory.Exists(brainsPath);

        // 
        if (!GM.botsExist)
        {
            Directory.CreateDirectory(botsPath);
        }

        // 
        int botsCount = Directory.GetFiles(botsPath).Length;

        // 
        GM.botsExist = botsCount > 0;

        dd = new List<TMP_Dropdown.OptionData>();
        List<TMP_Dropdown.OptionData> dd2 = new List<TMP_Dropdown.OptionData>();

        // 
        dd2.Add(new TMP_Dropdown.OptionData("Human"));

        // 
        if (brainsExist)
        {
            // 
            string[] aI_Config = Directory.GetFiles(brainsPath);

            // reset
            newAI_Config = new List<string>();

            // 
            for (int i = 0; i < aI_Config.Length; i++)
            {
                if (aI_Config[i].Contains(".sbr") && !aI_Config[i].Contains(".meta"))
                {
                    newAI_Config.Add(aI_Config[i]);
                }
            }

            // 
            for (int i = 0; i < newAI_Config.Count; i++)
            {
                string name = "";

                // 
                {
                    // 
                    using (StreamReader sR = new StreamReader(newAI_Config[i]))
                    {
                        name = sR.ReadToEnd().Split('\n')[0];
                    }

                    // 
                    dd2.Add(new TMP_Dropdown.OptionData(name));
                }
            }
        }

        // 
        if (GM.botsExist)
        {
            // 
            string[] allBots = Directory.GetDirectories(botsPath);

            // 
            for (int i = 0; i < allBots.Length; i++)
            {
                dd.Add(new TMP_Dropdown.OptionData(allBots[i].Split('\\')[2]));
            }
        }
        else
        {
            StartCoroutine(ShowMessage("You do not have any bots installed! Please visit the gamejolt page to download some bots!"));
        }

        // 
        for (int i = 0; i < 2; i++)
        {
            dd_Haxbot[i].ClearOptions();
            dd_Intelli[i].ClearOptions();

            // 
            dd_Haxbot[i].AddOptions(dd);
            //
            dd_Intelli[i].AddOptions(dd2);
            // 
            LoadNSetHaxbot(i);
        }
    }

    void MenuInit()
    {
        // 
        menuAud = gameObject.AddComponent<AudioSource>();
        menuAud.clip = menuSFX;

        // 
        ColorUtility.TryParseHtmlString("#A1FF7E", out GM.color[0]);
        ColorUtility.TryParseHtmlString("#FF6843", out GM.color[1]);
        ColorUtility.TryParseHtmlString("#44CAFF", out GM.color[2]);

        // 
        roundsSli.value = GM.totalRounds;

        bSpeedSli.value = GM.battleSpd;

        bSpeedTxt.text = $"Battle Speed: {(GM.battleSpd == 51 ? "MAX" : "" + GM.battleSpd)}";

        // 
        for (int i = 0; i < 2; i++)
        {
            GM.isForrest[i] = false;
            sliShowoff[i].value = GM.explSetter[i];
            scro_Learning[i].value = GM.nnIsLearning[i] ? 0 : 1;
            GM.nnIsLearning[i] = scro_Learning[i].value == 0 && _isNN[i] ? true : false;
        }

        // 
        dd_Haxbot[0].onValueChanged.AddListener(delegate { GM.haxBotChoice[0] = dd_Haxbot[0].value; LoadNSetHaxbot(0); menuAud.Play(); });
        dd_Haxbot[1].onValueChanged.AddListener(delegate { GM.haxBotChoice[1] = dd_Haxbot[1].value; LoadNSetHaxbot(1); menuAud.Play(); });

        // 
        dd_Intelli[0].onValueChanged.AddListener(delegate { GM.intelliChoice[0] = dd_Intelli[0].value; SetIntelli(0); menuAud.Play(); });
        dd_Intelli[1].onValueChanged.AddListener(delegate { GM.intelliChoice[1] = dd_Intelli[1].value; SetIntelli(1); menuAud.Play(); });

        // 
        UpdatePanel(0);
        UpdatePanel(1);
        UpdatePanel(2);
        UpdatePanel(3);

        // set the proper menu
        ChangeMenu(0);
        CheckIfCanCreateAI();

        for (int i = 0; i < percentage.Length; i++)
        {
            UpdatePercentage(i);
        }

        UpdateBestOfUI();
    }

    void UpdateBestOfUI()
    {
        img_BestOf.color = GM.bestOf ? GM.green : GM.blue;
        txt_BestOf.text = GM.bestOf ? $"Match Type: Play best of {GM.totalRounds} rounds" : $"Match Type: Play all {GM.totalRounds} rounds";
    }

    public void ToggleLearningSlider(int i)
    {
        GM.nnIsLearning[i] = scro_Learning[i].value == 0 && _isNN[i] ? true : false;

        scro_Learning[i].GetComponent<Image>().color = GM.nnIsLearning[i] ? GM.green : GM.red;
    }

    public void SliderRoundsValChanged()
    {
        GM.totalRounds = (int)roundsSli.value * GM.roundsMultiplier;
        roundsTxt.text = $"Rounds {(GM.roundsMultiplier > 1 ? $"x{GM.roundsMultiplier}" : "")}: {GM.totalRounds}";

        UpdateBestOfUI();
    }

    public void SliderSpeedValChanged()
    {
        GM.battleSpd = bSpeedSli.value;
        bSpeedTxt.text = $"Battle Speed: {(GM.battleSpd == 51 ? "MAX" : "" + GM.battleSpd)}";
    }

    public void SliderShowoffChanged(int which)
    {
        GM.explSetter[which] = sliShowoff[which].value;
        txtShowoff[which].text = $"Showoff: {Mathf.Round(GM.explSetter[which] * 100)}%";
    }

    public void SupportDev()
    {
        Application.OpenURL("https://paypal.me/SEFDStuff");
    }

    public void UpdatePercentage(int who)
    {
        percentage[who].text = $"{Mathf.Round(percSlider[who].value * 100)}%";
    }

    public void SliderBrainSizeChanged(int which)
    {
        txt_BrainSize[which].text = $"Brain Size: {sli_BrainSize[which].value}";
    }

    public void CreateAIBrain()
    {
        // just a random hyperparam that I just chose at whim, 4 nodes per 1 output node for the HL
        int attHLNodesPer1 = (int)sli_BrainSize[3].value;
        // inputs are 26 different values
        int inpCount = 16;
        // attack constant output nodes
        int attOutNodeCount = 3;
        // attack HL node count
        int attHLNodeCount = attHLNodesPer1 * attOutNodeCount;

        // calulate how many weights we got for attack weight
        int attWeightCount = (inpCount * attHLNodeCount) + (attHLNodeCount * attOutNodeCount);

        print($"SET: {attWeightCount}");

        // 
        string[] strat = new string[4];

        // 
        int[] weightCount = new int[] { 84, 276, 84, attWeightCount };

        // 
        string ret = "";

        print($"Check: {weightCount[3]}");

        // 
        for (int j = 0; j < weightCount.Length; j++)
        {
            string ch = "";
            bool last = j == weightCount.Length - 1;

            // 
            if (nn_strat[j])
            {
                ch = $"{dd_aiType[j].value - 1}";
            }
            else
            {

                ch = last ? $"{percSlider[j].value},{percSlider[j + 1].value},{percSlider[j + 2].value}" : $"{percSlider[j].value}";
                ret += $"{ch}{(last ? "" : ";")}";
                continue;
            }

            //
            for (int i = 0; i < weightCount[j]; i++)
            {
                strat[j] += $"{(ch == "-1" ? UnityEngine.Random.Range(-1f, 1f).ToString() : ch)}{(i != weightCount[j] - 1 ? "," : "")}";
            }

            // 
            ret += $"{strat[j]}{(last ? "" : ";")}";
        }

        // 
        string bPath = Path.Combine(Application.dataPath, "Brains");

        // 
        if (!Directory.Exists(bPath))
        {
            Directory.CreateDirectory(bPath);
        }

        // 
        using (StreamWriter sW = new StreamWriter(Path.Combine(bPath, $"{brainInp.text}.sbr")))
        {
            sW.Write($"{brainInp.text}\n{attHLNodesPer1}\n{ret}");
        }

        StartCoroutine(ShowMessage($"Your AI Brain {brainInp.text} has been Saved in the data file/Brains!"));
    }

    public void CheckIfCanCreateAI()
    {
        createAI.interactable = brainInp.text != "";
    }

    public void UpdatePanel(int curr)
    {
        int which = curr * 2;

        scroll[curr].GetComponent<Image>().color = scroll[curr].value == 0 ? new Color32(114, 255, 90, 255) : new Color32(144, 90, 255, 255);
        scroll[curr].GetComponentInChildren<TextMeshProUGUI>().text = scroll[curr].value == 0 ? "Traditional" : "Neural Network";
        panelSwitch[which].gameObject.SetActive(scroll[curr].value == 0);
        panelSwitch[which + 1].gameObject.SetActive(scroll[curr].value != 0);
    }

    void LoadNSetHaxbot(int i)
    {
        if (GM.botsExist)
        {
            dd_Haxbot[i].gameObject.SetActive(true);
            int chosen = GM.haxBotChoice[i];

            hbD[i] = HXB.LoadHaxbot(bot[i], dd[chosen].text);
            name[i].text = hbD[i].name;
            GM.hbName[i] = hbD[i].name;

            // 
            for (int j = 0; j < 2; j++)
            {
                foreach (Renderer r in bot[j].GetComponentsInChildren<Renderer>())
                {
                    r.material.SetTexture(GM.mainTexture, hbD[j].txt2d);
                }
            }
        }
        else
        {
            dd_Haxbot[i].gameObject.SetActive(false);
        }
    }

    void SetIntelli(int i)
    {
        int chosen = GM.intelliChoice[i];

        GM.intelli[i] = chosen == 0 ? new AI_Config("Human", "Human\n0\nx") : AI.LoadIntelligence(newAI_Config[chosen - 1]);

        sliShowoff[i].gameObject.SetActive(_isNN[i]);
        scro_Learning[i].gameObject.SetActive(_isNN[i]);
        GM.nnIsLearning[i] = scro_Learning[i].value == 0 && _isNN[i] ? true : false;
    }

    // Update is called once per frame
    void Update()
    {
        bCol.color = Color.Lerp(Color.white, Color.red, Mathf.Abs(Mathf.Sin(Time.time)));
        title.localRotation = Quaternion.Euler(Vector3.forward * Mathf.Sin(Time.time * 5) * 2);

        // 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuState == 0)
            {
                Application.Quit();
            }
            else
            {
                ChangeMenu(0);
            }

            // 
            if (authorNoting)
            {
                EndAuthorsNote();
            }
                    
        }

        if (menuState == 1)
        {
            // 
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GM.roundsMultiplier++;

                GM.roundsMultiplier = Mathf.Clamp(GM.roundsMultiplier, 1, GM.roundsMultiplier);
            }

            // 
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GM.roundsMultiplier--;

                GM.roundsMultiplier = Mathf.Clamp(GM.roundsMultiplier, 1, GM.roundsMultiplier);
            }

            SliderRoundsValChanged();

            if (Input.GetKey(KeyCode.RightShift))
            {
                KeyCode[] keyPress = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2 };

                for (int i = 0; i < 2; i++)
                {
                    if (Input.GetKeyDown(keyPress[i]))
                    {
                        bot[i].SetActive(false);
                        forrest[i].SetActive(true);
                        name[i].text = "Gladiator Forrest";
                        GM.isForrest[i] = true;
                        dd_Haxbot[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        // 
        LoadDropDownData();
    }

    void LoadDropDownData()
    {
        for (int i = 0; i < 2; i++)
        {
            if (dd_Intelli[i].value != GM.intelliChoice[i])
            {
                dd_Intelli[i].value = GM.intelliChoice[i];
            }

            if (dd_Haxbot[i].value != GM.haxBotChoice[i])
            {
                dd_Haxbot[i].value = GM.haxBotChoice[i];
            }
        }
    }

    IEnumerator ShowMessage(string msg)
    {
        menuAud.Play();
        msgBox.gameObject.SetActive(true);
        msgBox.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        yield return new WaitForSeconds(3);
        msgBox.gameObject.SetActive(false);
    }

    public void ChangeMenu(int w)
    {
        menuAud.Play();

        menuState = w;
        toolTipTxt.text = "";

        if (w == 1)
        {
            // 
            LoadBattleData();
            SetIntelli(0);
            SetIntelli(1);
        }

        flames.gameObject.SetActive(w == 1); // ? new Vector3(0.19f, -1.81f, 1.47f) : Vector3.up * -10000;

        Color32[] col = new Color32[] { new Color32(213, 202, 255, 255), new Color32(63, 63, 63, 255), new Color32(81, 183, 255, 255), new Color32(0,0,0,255) };

        Camera.main.backgroundColor = col[w];

        // 
        for (int i = 0; i < menu.Length; i++)
        {
            menu[i].gameObject.SetActive(i == w);
        }
    }

    public void GoTo(string where)
    {
        menuAud.Play();

        string dt = DateTime.Now.ToString().Replace(" ", "_").Replace(":", "-").Replace("/", "-");
        GM.battleName = $"BattleOf_{GM.intelli[0].aiName}VS{GM.intelli[1].aiName}_{dt}";

        // 
        if (where == "battle" && GM.battleSpd == 51)
        {
            GM.maxSimSpeed = true;
        }
        else
        {
            GM.maxSimSpeed = false;
        }

        SceneManager.LoadScene(where);
    }

    public void ToggleBestOf()
    {
        menuAud.Play();

        if (GM.bestOf)
        {
            GM.bestOf = false;
        }
        else
        {
            GM.bestOf = true;
        }

        UpdateBestOfUI();
    }

    public void SetToolTip(string tt)
    {
        toolTipTxt.gameObject.SetActive(tt != "");
        toolTipTxt.text = tt;
    }

    public void AuthorsNote()
    {
        StartCoroutine(AuthorsNoteTimer());
    }

    IEnumerator AuthorsNoteTimer()
    {
        authorNoting = true;
        authorsNote.gameObject.SetActive(true);

        menuAud.clip = aNote;
        menuAud.Play();
        yield return new WaitForSeconds(1.75f * 60);
        EndAuthorsNote();
    }

    void EndAuthorsNote()
    {
        authorNoting = false;
        menuAud.clip = menuSFX;
        authorsNote.gameObject.SetActive(false);
    }
}

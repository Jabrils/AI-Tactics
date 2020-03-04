using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightCTRL : MonoBehaviour
{
    public enum Phase { Start, Battle, End };
    public static Phase phase;

    public enum InputType { Random, Zero, One, DummyAI, Human };

    public string levelName;
    public int mMoves = 5;
    public bool randomOutputs;
    public float btSpd;
    [Range(0, 1)]
    public float dist = 1;
    [Range(-1, 1)]
    public float angleX = 0, angleY = 0;
    public GameObject one, two;
    public AudioClip sfx_Walk, sfx_Draw, sfx_StepBack, sfx_Hit, sfx_Crit, sfx_Def, sfx_PowerUp, sfx_PowerDown, sfx_End;
    public bool areInBattle;
    public InputType p1, p2;

    int _turn;
    public int turn => _turn % 2;
    public int gmTurn => GM.turnSyncer % 2;
    public int time => _turn;
    public bool aHumanIsInvolved => humansInvolved[0] || humansInvolved[1];

    Map map;
    Fighter[] fighter = new Fighter[2];
    AudioSource aS;
    Loc[] outp = new Loc[2];

    float p_D = -1, p_aX = -1, p_aY = -1;
    bool[] _humansInvolved = new bool[2];
    int nextCandySpawn;
    int randNextFood => Random.Range(20, 40);
    public bool[] humansInvolved => _humansInvolved;

    public static Material[] txts;

    void Start()
    {
        // set the 
        GM.inpType[0] = p1;
        GM.inpType[1] = p2;

        // 
        AddToNextFoodSpawn();

        // 
        for (int i = 0; i < GM.inpType.Length; i++)
        {
            if (GM.inpType[i] == InputType.Human)
            {
                humansInvolved[i] = true;
            }
        }

        // 
        txts = new Material[] { Resources.Load<Material>("Mats/attack"), Resources.Load<Material>("Mats/defend"), Resources.Load<Material>("Mats/taunt") };

        // 
        map = new Map(this, levelName);

        // 
        fighter[0] = new Fighter(one, 0, map.mapSize);
        fighter[1] = new Fighter(two, 1, map.mapSize);

        // 
        fighter[0].SetOpponent(fighter[1]);
        fighter[1].SetOpponent(fighter[0]);

        // 
        fighter[0].LookAtOpponent();
        fighter[1].LookAtOpponent();

        // 
        map.SetFighters(fighter);

        // 
        map.SetCamTo(Map.CamMode.Field);

        // 
        aS = gameObject.AddComponent<AudioSource>();

        // 
        for (int i = 0; i < 2; i++)
        {
            outp[i] = Map.OutputLocation(map, fighter[i].expression, fighter[i == 0 ? 1 : 0].expression, dist, angleX, angleY);
        }

        // 
        foreach (Tile ve in map.loc)
        {
            if (!ve.free)
            {
                Color c = ve.type == 'w' ? new Color(.75f, .25f, 0) : Color.black;
                GameObject g = null;
            }
        }
    }

    void AddToNextFoodSpawn()
    {
        nextCandySpawn += randNextFood;
    }

    void Update()
    {
        GM.battleSpd = btSpd;

        GM.maxMoves = mMoves;

        // 
        if (_turn == nextCandySpawn)
        {
            SpawnCandy();
        }

        // 
        if (phase == Phase.Battle)
        {
            // 
            if (_turn == GM.turnSyncer)
            {
                ProcessTurn();
            }
        }

        // 
        ListenForControls();
    }

    void SpawnCandy()
    {
        // 
        GameObject food = Instantiate(Resources.Load<GameObject>("Objs/Hax! Bar"));
        // 
        int tileID = Random.Range(0, map.freeTiles.Count);
        // 
        Tile tile = map.freeTiles[tileID];
        // 
        tile.SetDressing(food);
        // 
        Vector2Int loc = tile.expression;
        // 
        map.candyTiles.Add(tile);
        // 
        map.freeTiles.RemoveAt(tileID);
        // 
        food.transform.position = new Vector3(loc.x, 1, loc.y);
        // 
        AddToNextFoodSpawn();
    }

    void ProcessTurn()
    {
        TakeTurn();

        // 
        if (outp[gmTurn].path != null)
        {
            List<Tile> tempP = new List<Tile>(outp[gmTurn].path.path);

            // 
            if (!areInBattle)
            {
                // 
                for (int i = 0; i < outp[gmTurn].loc.Count; i++)
                {
                    outp[gmTurn].loc[i].ToggleRender(true, (Color.blue + Color.red) / 2);
                }

                // 
                for (int i = 0; i < outp[gmTurn].selLoc.Count; i++)
                {
                    outp[gmTurn].selLoc[i].ToggleRender(true, i == outp[gmTurn].angleSelect ? Color.yellow : Color.cyan);

                    // 
                    if (i == outp[gmTurn].angleSelect)
                    {
                        tempP.Remove(outp[gmTurn].selLoc[i]);
                    }
                }

                // 
                for (int i = 0; i < tempP.Count; i++)
                {
                    tempP[i].ToggleRender(true, Color.white);
                }
            }
            else
            {
                map.ResetAllTiles();
            }
        }
    }

    public void PlaySFX(string c)
    {
        AudioClip ac = null;

        if (c == "walk")
        {
            ac = sfx_Walk;
        }
        else if (c == "draw")
        {
            ac = sfx_Draw;
        }
        else if (c == "stepback")
        {
            ac = sfx_StepBack;
        }
        else if (c == "hit")
        {
            ac = sfx_Hit;
        }
        else if (c == "def")
        {
            ac = sfx_Def;
        }
        else if (c == "crit")
        {
            ac = sfx_Crit;
        }
        else if (c == "powerup")
        {
            ac = sfx_PowerUp;
        }
        else if (c == "powerdown")
        {
            ac = sfx_PowerDown;
        }
        else if (c == "end")
        {
            ac = sfx_End;
        }

        aS.clip = ac;
        aS.Play();
    }

    void ListenForControls()
    {
        // 
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            phase = Phase.Start;
            GM.turnSyncer = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // 
        if (Input.GetKeyDown(KeyCode.Space) && phase != Phase.End)
        {
            phase = Phase.Battle;
        }

        // 
        if (Input.GetKeyDown(KeyCode.F))
        {
            map.SetCamTo(Map.CamMode.Field);
        }

        // 
        if (Input.GetKeyDown(KeyCode.T))
        {
            map.SetCamTo(Map.CamMode.Topdown);
        }

        // 
        if (Input.GetKeyDown(KeyCode.I))
        {
            map.SetCamTo(Map.CamMode.Isometric);
        }

        // 
        if (Input.GetKeyDown(KeyCode.A))
        {
            map.SetCamTo(Map.CamMode.Action);
        }

        // 
        if (Input.GetKeyDown(KeyCode.H))
        {
            map.SetCamTo(Map.CamMode.IsoAction);
        }
    }

    void TakeTurn()
    {
        // At the start of the turn, do a checkup on the fighters
        for (int i = 0; i < fighter.Length; i++)
        {
            fighter[i].stateData.Update(turn);
            fighter[i].CheckUp(time);
        }

        // We must reset the tile renderings
        map.ResetAllTiles();

        // 
        if (humansInvolved[turn])
        {
            if (p_D != dist || p_aX != angleX || p_aY != angleY)
            {
                // Get the movement data
                outp[turn] = Map.OutputLocation(map, fighter[turn].expression, fighter[turn == 0 ? 1 : 0].expression, dist, angleX, angleY);
                p_D = dist;
                p_aX = angleX;
                p_aY = angleY;
            }

            // 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // start our Coroutine of moving our fighter
                StartCoroutine(map.MoveFighter(time, outp[turn].loc.Count > 0, map, turn, GM.battleSpd, outp[turn].path));

                // incriment the turn
                _turn++;

                p_D = -1;
                p_aY = -1;
                p_aX = -1;
            }
            else if(Input.GetKeyDown(KeyCode.Backspace))
            {
                StartCoroutine(map.FIGHT(time, map, turn));

                // incriment the turn
                _turn++;

                p_D = -1;
                p_aY = -1;
                p_aX = -1;
            }
        }
        else
        {
            // 
            OutputStay s = OutputStay.Calculate(fighter[turn]);

            // Calculate if moving
            if (s.stay)
            {
                // calculate if AI even wants to battle
                OutputToBattle oB = OutputToBattle.CalculateOutput(new StateData());

                // if it's dice is > .5f, then we will count that as a yes!
                if (oB.decision > .5f)
                {
                    StartCoroutine(map.FIGHT(time, map, turn));
                }
                else // otherwise we'll just entirely skip its turn
                {
                    GM.turnSyncer++;
                }

                // incriment the turn
                _turn++;
            }
            else
            {
                // Calculate the move output
                OutputMove m = OutputMove.CalculateOutput(fighter[turn].stateData);

                // Get the movement data
                outp[turn] = Map.OutputLocation(map, fighter[turn].expression, fighter[turn == 0 ? 1 : 0].expression, randomOutputs ? m.distance : 0, m.angleX, m.angleY);

                // start our Coroutine of moving our fighter
                StartCoroutine(map.MoveFighter(time, outp[turn].loc.Count > 0, map, turn, GM.battleSpd, outp[turn].path));

                // incriment the turn
                _turn++;
            }
        }
    }

    public void StartABattle(int who)
    {
        StartCoroutine(map.FIGHT(time, map, who));
    }
}

public class Loc
{
    public List<Tile> loc;
    public List<Tile> selLoc;
    public TilePath path;
    public int angleSelect;

    public Loc(List<Tile> loc, List<Tile> selLoc, TilePath path, int angleSelect)
    {
        this.loc = loc;
        this.selLoc = selLoc;
        this.path = path;
        this.angleSelect = angleSelect;
    }
}
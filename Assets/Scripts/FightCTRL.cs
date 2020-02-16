using System.Collections.Generic;
using System.IO;
using UnityEngine;

class FightCTRL : MonoBehaviour
{
    public string levelName;
    public int mMoves = 5;
    public bool randomOutputs;
    [Range(0, 1)]
    public float dist = 1;
    [Range(-1, 1)]
    public float angleX = 0, angleY = 0;
    public GameObject one, two;

    int _time;
    public int time => _time;

    int _turn;
    public int turn => _turn % 2;

    int angleSelect;
    Map map;
    List<Tile> selLoc = new List<Tile>();
    List<Tile> loc = new List<Tile>();
    TilePath p;
    Fighter[] fighter = new Fighter[2];
    StateData stateData = new StateData();

    void Start()
    {
        // 
        map = new Map(levelName);

        // 
        fighter[0] = new Fighter(one, map.mapSize);
        fighter[1] = new Fighter(two, map.mapSize);

        // 
        fighter[0].SetOpponent(fighter[1]);
        fighter[1].SetOpponent(fighter[0]);

        map.SetFighters(fighter);

        GameObject blocksParent = new GameObject("Blocks");

        // 
        foreach (Tile ve in map.loc)
        {
            if (!ve.free)
            {
                Color c = ve.type == 'w' ? new Color(.75f, .25f, 0) : Color.black;
                GameObject g = null;

                // 
                if (ve.type == 'p')
                {
                    g = Instantiate(Resources.Load<GameObject>("Objs/Pillar"));
                    g.transform.position = new Vector3(ve.x - map.halfMapSize, .5f, ve.y - map.halfMapSize);
                }
                else if (ve.type == 'w')
                {
                    g = Instantiate(Resources.Load<GameObject>("Objs/Wall"));
                    g.transform.localScale = new Vector3(1, 2, 1);
                    g.transform.position = new Vector3(ve.x - map.halfMapSize, 1.5f, ve.y - map.halfMapSize);
                }

                // 
                g.transform.SetParent(blocksParent.transform);
            }
        }
    }

    void Update()
    {
        GM.maxMoves = mMoves;

        // 
        if (_turn == GM.turnSyncer)
        {
            TakeTurn();
        }
        
        // 
        for (int i = 0; i < loc.Count; i++)
        {
            loc[i].ToggleRender(true, (Color.blue + Color.red) / 2);
        }

        // 
        for (int i = 0; i < selLoc.Count; i++)
        {
            selLoc[i].ToggleRender(true, i == angleSelect ? Color.yellow : Color.cyan);
        }

        // 
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            fighter[0].obj.transform.position += Vector3.forward;
        }

        // 
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            fighter[0].obj.transform.position -= Vector3.forward;
        }

        // 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            fighter[0].obj.transform.position += Vector3.right;
        }

        // 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            fighter[0].obj.transform.position -= Vector3.right;
        }
    }

    void TakeTurn()
    {
        for (int i = 0; i < map.loc.GetLength(0); i++)
        {
            for (int j = 0; j < map.loc.GetLength(0); j++)
            {
                map.loc[i, j].ToggleRender(false, Color.clear);
            }
        }

        // 
        OutputMove o = OutputMove.CalculateOutput(stateData);

        // 
        (List<Tile> loc, List<Tile> selLoc, TilePath path, int angleSelect) outp = Map.OutputLocation(map, fighter[turn].expression, fighter[turn == 0 ? 1 : 0].expression, randomOutputs ? o.distance : dist, o.angleX, o.angleY);

        p = outp.path;
        loc = outp.loc;
        selLoc = outp.selLoc;
        angleSelect = outp.angleSelect;

        StartCoroutine(map.MoveFighter(turn, GM.battleSpd, p));

        _turn++;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (p != null)
            {
                for (int i = 0; i < p.path.Count; i++)
                {
                    Gizmos.color = i == p.path.Count - 1 ? Color.yellow : Color.white;
                    Gizmos.DrawWireCube(new Vector3(p.path[i].eX, 1, p.path[i].eY), Vector3.one);
                }
            }
        }
    }
}
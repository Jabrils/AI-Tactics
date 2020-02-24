using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Map
{
    public int mapSize => loc.GetLength(0);
    public int halfMapSize => mapSize / 2;
    Tile tileFrom => _loc[fX, fY];
    Tile tileTo => _loc[tX, tY];
    public Tile[,] loc => _loc;
    int fX, fY, tX, tY;
    public TilePath thee;
    Fighter[] fighter;
    Camera cam = Camera.main;
    Dictionary<int, string> battleDecLookUp = new Dictionary<int, string> { { 0, "Attack" }, { 1, "Defend" }, { 2, "Taunt" }, };
    Vector3 zoomUnit = new Vector3(2, 5, 2.5f);
    public float zoom = 1f;
    FightCTRL fC;
    CameraShake camShake;

    public enum CamMode { Field, Topdown, Free, Isometric };
    public CamMode camMode;

    Tile current;
    Tile[,] _loc;
    public List<Tile> openTiles, closedTiles;

    public Map(int size)
    {
        // 
        _loc = new Tile[size, size];

        // 
        MapInit();

        // 
        InitLocations();
    }

    public Map(FightCTRL fc, string path)
    {
        fC = fc;

        MapInit();

        LoadLeveData(path);
    }

    void MapInit()
    {
        camShake = Camera.main.GetComponent<CameraShake>();
        GM.tilesParent = new GameObject("Tiles").transform;
    }

    public void BlockTiles(List<Vector2Int> b)
    {
        for (int i = 0; i < b.Count; i++)
        {
            _loc[b[i].x, b[i].y].free = false;
        }
    }

    public void SetCamTo(CamMode c, int who = 0)
    {
        if (c == CamMode.Field)
        {
            cam.transform.position = new Vector3(-.5f, 20, -.5f);
            cam.transform.eulerAngles = new Vector3(90, 0, 0);
            cam.orthographic = true;

            camMode = CamMode.Field;
        }
        else if (c == CamMode.Topdown)
        {
            Vector3 p = fighter[who].obj.transform.position;

            cam.transform.position = new Vector3(p.x, p.y + 7.5f, p.z);
            cam.transform.LookAt(fighter[who].obj.transform);
            cam.orthographic = false;

            camMode = CamMode.Topdown;
        }
        else if (c == CamMode.Isometric)
        {
            Vector3 p = fighter[who].obj.transform.position;

            cam.transform.position = new Vector3(p.x - (zoomUnit.x * zoom), p.y + (zoomUnit.y * zoom), p.z - (zoomUnit.z * zoom));
            cam.transform.LookAt(fighter[who].obj.transform);
            cam.orthographic = false;

            camMode = CamMode.Isometric;
        }
    }

    public void SetFighters(Fighter[] f)
    {
        fighter = f;
    }

    void InitLocations()
    {
        // first we init
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                _loc[j, i] = new Tile(j, i, halfMapSize: halfMapSize);
            }
        }

        // 
        GM.mapSize = mapSize;
    }

    public List<Tile> CalcAvailbleMoves(Vector2Int source)
    {
        // get a reference to what we're going to return
        List<Tile> ret = new List<Tile>();

        // loop through the entire map size
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                // if the tile is in the right distance from the source & that tile is not the tile the source is on,
                if ((ManhattanDistance(source, loc[j, i].v2Int) <= GM.maxMoves) && source != loc[j, i].v2Int)
                //if ((Mathf.Abs(source.x - loc[j,i].x) <= GM.maxMoves) && source != loc[j, i].v2Int)
                {
                    // also IF that space is reachable in 5 or less moves
                    if (FindPath(source.x, source.y, loc[j, i].x, loc[j, i].y)?.dist <= GM.maxMoves)
                    {
                        // add it to the return reference
                        ret.Add(loc[j, i]);
                    }
                }
            }
        }

        return ret;
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        // calculate the X + Y distance between a & b
        return (Mathf.Abs(a.x - b.x) + (Mathf.Abs(a.y - b.y)));
    }

    public static int SelectAnAngle(List<Tile> toCheck, float iX, float iY, Vector2Int whoLoc)
    {
        int ret = -1;

        // 
        float[] temp = new float[toCheck.Count];

        // Get all availible select positions & give them an ID
        for (int i = 0; i < toCheck.Count; i++)
        {
            temp[i] = GM.XYtoDeg(toCheck[i].x - whoLoc.x, toCheck[i].y - whoLoc.y);
        }

        float lowestDist = Mathf.Infinity;

        // Get the distance from desired point to all availible selected positions
        for (int i = 0; i < temp.Length; i++)
        {
            float desiredPoint = GM.XYtoDeg(iX, iY);

            float dist = Mathf.Abs(desiredPoint - temp[i]);

            // 
            if (dist < lowestDist)
            {
                lowestDist = dist;
                ret = i;
            }
        }

        // Return the availible selected position with the lowest distance
        return ret;
    }

    public TilePath FindPath(int _fX, int _fY, int _tX, int _tY)
    {
        return PathFinder(_fX, _fY, _tX, _tY);
    }

    public TilePath FindLimitedPath(int _fX, int _fY, int _tX, int _tY)
    {
        // if the tile we are trying to reach is out of range,
        if (ManhattanDistance(new Vector2Int(_fX, _fY), new Vector2Int(_tX, _tY)) > GM.maxMoves)
        {
            // appologize. right now.
            Debug.Log($"Sorry out of range!");

            return null;
        }

        return PathFinder(_fX, _fY, _tX, _tY);
    }

    public void ResetAllTiles()
    {
        int l = loc.GetLength(0);

        for (int i = 0; i < l; i++)
        {
            for (int j = 0; j < l; j++)
            {
                loc[i, j].ToggleRender(false, Color.clear);
            }
        }
    }

    public void LoadLeveData(string levelName)
    {
        using (StreamReader sR = new StreamReader($"{Path.Combine(Application.dataPath, $"lvl_{levelName}{GM.lvlExt}")}"))
        {
            // make an algo that will convert text data of x = block o = free into a list of Vector2Ints
            string[] raw = sR.ReadToEnd().Split('\n');

            // 
            int mapSize = raw.Length;

            // 
            _loc = new Tile[mapSize, mapSize];

            // 
            InitLocations();

            // make a reference to the data mapped as chars
            char[,] mapped = new char[mapSize, mapSize];

            // 
            List<Vector2Int> block = new List<Vector2Int>();

            // loop through the mapped chars
            for (int i = 0; i < raw.Length; i++)
            {
                for (int j = 0; j < raw.Length; j++)
                {
                    char tileData = raw[i][j];

                    // fill in the mapped chars
                    mapped[j, i] = tileData;

                    // 
                    if (tileData == 'o')
                    {
                        // 
                        float dice = Random.value;

                        // 
                        if (dice < GM.randomProbability)
                        {
                            mapped[j, i] = 'p';
                        }

                        // 
                        tileData = mapped[j, i];
                    }

                    // assign type
                    loc[j, i].AssignType(tileData);

                    // ADD ALL THINGS, w, p etc
                    if (tileData == 'p' || tileData == 'w')
                    {
                        block.Add(new Vector2Int(j, i));
                    }
                }
            }

            // pass that to map.Block()
            BlockTiles(block);
        }
    }

    public static (List<Tile> loc, List<Tile> selLoc, TilePath path, int angleSelect) OutputLocation(Map map, Vector2Int self, Vector2Int opponent, float dist, float angleX, float angleY)
    {
        // reset our selPos list every frame, this allows us to make changes to it
        List<Tile> selLoc = new List<Tile>();

        // 
        int intervals = GM.maxMoves * 2;

        // now we calculate the selected range of tiles based on distance from the opponent
        int calc = Mathf.RoundToInt((dist * intervals) + (Map.ManhattanDistance(self, opponent) - GM.maxMoves));

        // we need a reference to a temp all selected positions list
        List<Vector2Int> tempAllSelPos = new List<Vector2Int>();

        // 
        List<Tile> loc = map.CalcAvailbleMoves(self);

        // 
        int angleSelect = 0;
        TilePath p = new TilePath(new List<Tile>());

        // 
        if (loc.Count > 0)
        {
            // we need a reference to the furthest distance in the availible spaces
            int closest = 10000;
            int furthest = 0;

            // 
            for (int i = 0; i < loc.Count; i++)
            {
                int temp = Map.ManhattanDistance(loc[i].v2Int, opponent);

                closest = temp < closest ? temp : closest;
                furthest = temp > furthest ? temp : furthest;
            }

            // next we loop through all of the positions & see who is viable
            for (int i = 0; i < loc.Count; i++)
            {
                // 
                if (Map.ManhattanDistance(loc[i].v2Int, opponent) == Mathf.Clamp(calc, closest, furthest))
                {
                    selLoc.Add(loc[i]);
                }
            }

            // 
            angleSelect = Map.SelectAnAngle(selLoc, angleX, angleY, opponent);

            // 
            // // 
            // // // THROWING ERRORS IF AGENT CAN'T MOVE
            // //
            // 

            // 
            p = map.FindLimitedPath(self.x, self.y, selLoc[angleSelect].x, selLoc[angleSelect].y);
        }

        return (loc, selLoc, p, angleSelect);
    }

    public IEnumerator MoveFighter(int time, bool canMove, Map map, int who, float speed, TilePath tp)
    {
        // 
        if (canMove)
        {
            // 
            int pathSpots = tp.path.Count;

            // 
            yield return new WaitForSeconds(1 / speed * 2);

            fighter[who].ChangeAnimation("Walk");

            // 
            for (int i = 0; i < pathSpots; i++)
            {
                fighter[who].MoveTo(time, map, tp.path[i].expression);
                yield return new WaitForSeconds(1 / speed);
            }

            fighter[who].ChangeAnimation("Idle");

            fighter[who].LookAtOpponent();
        }

        ResetAllTiles();

        map.fC.StartABattle(who);
    }

    public IEnumerator FIGHT(int time, Map map, int who)
    { 
        // 
        if (fighter[who].inAttackRange)
        {
            fC.areInBattle = true;

            // 
            OutputToBattle oB = OutputToBattle.CalculateOutput(new StateData());

            // 
            if (oB.decision > .5f)
            {
                // pass in state, output 1 2 or 3
                OutputAttack oA1 = OutputAttack.CalculateOutput(fighter[who].isStunned, fighter[who].stateData);

                // ask opp for their battle output
                OutputAttack oA2 = OutputAttack.CalculateOutput(fighter[who].opp.isStunned, fighter[who].opp.stateData);

                OutputAttack[] oA = new OutputAttack[] { oA1, oA2 };

                int[] did = new int[] { oA[0].decision, oA[1].decision };

                // 
                if (did[0] != -1)
                {
                    yield return new WaitForSeconds(1);
                    fC.PlaySFX("draw");

                    // change the animation to reveal battle results
                    fighter[who].ChangeAnimation(battleDecLookUp[did[0]], true);
                    fighter[who].opp.ChangeAnimation(battleDecLookUp[did[1]], true);

                    yield return new WaitForSeconds(2);

                    // idle animation the bots
                    fighter[who].ChangeAnimation("Idle");
                    fighter[who].opp.ChangeAnimation("Idle");
                }

                // result the battle
                fighter[who].Battle(time, oA, map);

                string sfx = "";

                if (oA[0].decision == 1 && oA[1].decision == 1)
                {
                    sfx = "stepback";
                }
                else if (oA[0].decision + oA[1].decision == 0)
                {
                    sfx = "hit";
                    camShake.ShakeCamera(.5f);
                }
                else if (oA[0].decision + oA[1].decision == 1)
                {
                    sfx = "def";
                    camShake.ShakeCamera(.25f);
                }
                else if (oA[0].decision + oA[1].decision == 2)
                {
                    sfx = "crit";
                    camShake.ShakeCamera();
                }
                else if (oA[0].decision + oA[1].decision == 3)
                {
                    sfx = "powerup";
                }
                else if (oA[0].decision + oA[1].decision == 4)
                {
                    sfx = "powerdown";
                }

                fC.PlaySFX(sfx);

                yield return new WaitForSeconds(2);

                // remove the damage from showing over the heads
                fighter[who].SetText(false, Color.clear);
                fighter[who].opp.SetText(false, Color.clear);

            }
        }

        // 
        bool[] lost = new bool[2];

        // 
        for (int i = 0; i < fighter.Length; i++)
        {
            if (fighter[i].hp <= 0)
            {
                lost[i] = true;
            }
        }

        // 
        if (lost[0] || lost[1])
        {
            PlaySFX("end");

            FightCTRL.phase = FightCTRL.Phase.End;

            // 
            if (lost[0] && !lost[1])
            {
                Debug.Log($"GAME OVER! {fighter[0].obj.name} LOST!");

                fighter[0].ChangeAnimation("Defeat");
                fighter[1].ChangeAnimation("Win");
            }
            else if (!lost[0] && lost[1])
            {
                Debug.Log($"GAME OVER! {fighter[1].obj.name} LOST!");

                fighter[0].ChangeAnimation("Win");
                fighter[1].ChangeAnimation("Defeat");
            }
            else if (lost[0] && lost[1])
            {
                Debug.Log($"GAME OVER! TIE GMAE!");
                fighter[0].ChangeAnimation("Defeat");
                fighter[1].ChangeAnimation("Defeat");
            }
        }

        // 
        fighter[who].LookAtOpponent();

        // 
        GM.turnSyncer++;
        fC.areInBattle = false;
    }

    public void PlaySFX(string sfx)
    {
        fC.PlaySFX(sfx);
    }

    TilePath PathFinder(int _fX, int _fY, int _tX, int _tY)
    {
        float start = Time.time;
        thee = new TilePath(new List<Tile>());

        // 
        fX = _fX;
        fY = _fY;
        tX = _tX;
        tY = _tY;

        // set list of tiles to be evaluated
        openTiles = new List<Tile>();
        // set list of tiles already evaluated
        closedTiles = new List<Tile>();

        // add current node to the open list
        AddTileToOpen(tileFrom);

        // While we still have tiles in the open tiles list
        while (openTiles.Count > 0)
        {
            // current tile = tile with the lowest f cost in the open set (start off with only 1 tile in the open tiles list so that is the lowest)
            current = GetLowestFCost();

            // if current == target, then we're done
            if (current == tileTo)
            {
                // 
                thee.path.Add(current);

                // While the current tile's parent does not = our starting tile,
                while (current.parent != tileFrom)
                {
                    // add it to the path
                    thee.path.Add(current.parent);
                    // update what the current node is
                    current = current.parent;
                }

                // Because we trace from the end to the start, we need to reverse the path
                thee.path.Reverse();

                return thee;
            }

            // if not then well Process this tile for exploration
            ProcessTile(current);
        }

        return null;
    }

    void AddTileToOpen(Tile t)
    {
        if (!openTiles.Contains(t))
        {
            openTiles.Add(t);
        }
    }

    void ProcessTile(Tile c)
    {
        // Get & set all of the neighbors for that tile, which will also add it to the open tiles list
        current.SetNeighbors(GetNeighbors(c, c.parent, tileTo));

        // remove current from open since it's ben processed
        openTiles.Remove(c);

        // add current to closed since its been processed
        closedTiles.Add(c);
    }

    float GetDistance(Vector2 a, Vector2 b)
    {
        return (a - b).sqrMagnitude;
    }

    List<Tile> GetNeighbors(Tile current, Tile _from, Tile to)
    {
        _from = current;
        //Debug.Log($"{current.x}, {current.y}: {current.free}");

        List<Tile> ret = new List<Tile>();

        // Set our current tile's fCost
        float gc = GetDistance(new Vector2(current.x, current.y), new Vector2(_from.x, _from.y));
        float hc = GetDistance(new Vector2(current.x, current.y), new Vector2(to.x, to.y));

        // set the g & h scores
        current.SetGnH(gc, hc);

        // need a reference to our temp tile
        Tile n = null;

        // if the spot to the left is within the map
        if (current.x - 1 >= 0)
        {
            // set the temp tile
            n = _loc[current.x - 1, current.y];

            // if the tile is not already in open tile set, or closed tile se, & it is free, & either player is not on the tile,
            if (!openTiles.Contains(n) && !closedTiles.Contains(n) && n.free && n.v2Int != fighter[0].expression && n.v2Int != fighter[1].expression)
            {
                // Get the g & h score
                gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
                hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

                // Set the g & h score
                n.SetGnH(gc, hc);
                // Set the parent
                n.SetParent(current);
                // Add the tile to the open set
                AddTileToOpen(n);
                // return the tile
                ret.Add(n);
            }
        }

        // if the spot to the bottom is within the map
        if (current.y - 1 >= 0)
        {
            // set the temp tile
            n = _loc[current.x, current.y - 1];

            // if the tile is not already in open tile set, or closed tile se, & it is free, & either player is not on the tile,
            if (!openTiles.Contains(n) && !closedTiles.Contains(n) && n.free && n.v2Int != fighter[0].expression && n.v2Int != fighter[1].expression)
            {
                // Get the g & h score
                gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
                hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

                // Set the g & h score
                n.SetGnH(gc, hc);
                // Set the parent
                n.SetParent(current);
                // Add the tile to the open set
                AddTileToOpen(n);
                // return the tile
                ret.Add(n);
            }
        }

        // if the spot to the right is within the map
        if (current.x + 1 <= mapSize - 1)
        {
            // set the temp tile
            n = _loc[current.x + 1, current.y];

            // if the tile is not already in open tile set, or closed tile se, & it is free, & either player is not on the tile,
            if (!openTiles.Contains(n) && !closedTiles.Contains(n) && n.free && n.v2Int != fighter[0].expression && n.v2Int != fighter[1].expression)
            {
                // Get the g & h score
                gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
                hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

                // Set the g & h score
                n.SetGnH(gc, hc);
                // Set the parent
                n.SetParent(current);
                // Add the tile to the open set
                AddTileToOpen(n);
                // return the tile
                ret.Add(n);
            }
        }

        // if the spot to the top is within the map
        if (current.y + 1 <= mapSize - 1)
        {
            // set the temp tile
            n = _loc[current.x, current.y + 1];

            // if the tile is not already in open tile set, or closed tile se, & it is free, & either player is not on the tile,
            if (!openTiles.Contains(n) && !closedTiles.Contains(n) && n.free && n.v2Int != fighter[0].expression && n.v2Int != fighter[1].expression)
            {
                // Get the g & h score
                gc = GetDistance(new Vector2(n.x, n.y), new Vector2(_from.x, _from.y));
                hc = GetDistance(new Vector2(n.x, n.y), new Vector2(to.x, to.y));

                // Set the g & h score
                n.SetGnH(gc, hc);
                // Set the parent
                n.SetParent(current);
                // Add the tile to the open set
                AddTileToOpen(n);
                // return the tile
                ret.Add(n);
            }
        }

        return ret;
    }

    Tile GetLowestFCost()
    {
        // 
        Tile lowest = new Tile(0, 0);
        lowest.SetGnH(0, Mathf.Infinity);

        // 
        for (int i = 0; i < openTiles.Count; i++)
        {
            if (openTiles[i].fCost < lowest.fCost)
            {
                lowest = openTiles[i];
            }
        }

        // 
        return lowest;
    }
}

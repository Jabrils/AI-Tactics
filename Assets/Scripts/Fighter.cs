using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

public class Fighter
{
    GameObject _obj;
    public GameObject obj => _obj;

    public string name => obj.name;

    Fighter _opp;
    public Fighter opp => _opp;

    Animator _anim;

    TextMeshPro _dmgTxt, _strTxt;

    SpriteRenderer _stunSpr;

    Transform _hpHolder;

    ParticleSystem _powerUp, _powerDown;

    MeshRenderer _display;

    int _ranAway;
    public int ranAway => _ranAway;

    int _maxMoves = 5;
    public int maxMoves;

    AI_Config _config;
    public AI_Config config => _config;

    public StateData stateData;

    public int x => Mathf.RoundToInt(obj.transform.position.x);
    public int y => Mathf.RoundToInt(obj.transform.position.z);

    public int eX => x + (mapSize / 2);
    public int eY => y + (mapSize / 2);

    int _waffleX;
    int _waffleY;

    public int waffleX => _waffleX;
    public int waffleY => _waffleY;

    int _myTurn;
    public int myTurn => _myTurn;

    int _hp = GM.maxHP;
    public int hp => _hp;
    public float hpPercent => (float)_hp / (float)GM.maxHP;

    int _str = 2;
    public int str => _str;

    int _lastStunned = -1;

    public Vector2Int v2Int => new Vector2Int(x, y);
    public Vector2Int expression => new Vector2Int(eX, eY);

    int distToOpp => Map.ManhattanDistance(v2Int, _opp.v2Int);
    bool _stunned;
    public bool isStunned => _stunned;

    public bool inAttackRange => distToOpp == 1;
    public bool strIsMin => _str == 1;

    int mapSize;

    public Fighter(GameObject gO, int myTurn, int mapSize, AI_Config aI)
    {
        _obj = gO;
        this.mapSize = mapSize;
        _myTurn = myTurn;
        _config = aI;

        // 
        _anim = _obj.GetComponentInChildren<Animator>();

        // 
        _hpHolder = gO.GetComponentsInChildren<SpriteRenderer>()[2].transform.parent;

        // 
        _stunSpr = gO.GetComponentsInChildren<SpriteRenderer>()[4];

        TextMeshPro[] grabTMP = gO.GetComponentsInChildren<TextMeshPro>();

        // 
        _dmgTxt = grabTMP[0];

        // 
        grabTMP[1].text = gO.name;

        // 
        _strTxt = grabTMP[2];

        // 
        _powerUp = gO.GetComponentsInChildren<ParticleSystem>()[0];

        // 
        _powerDown = gO.GetComponentsInChildren<ParticleSystem>()[1];

        //
        _display = gO.GetComponentsInChildren<MeshRenderer>()[6];

    }

    public void UpdateClosestCandy(List<Tile> waffle)
    {
        // 
        if (waffle.Count == 0)
        {
            _waffleX = 0;
            _waffleY = 0;
        }
        else
        {
            int id = -1;
            int dist = 1000;

            // 
            for (int i = 0; i < waffle.Count; i++)
            {
                int computeD = Map.ManhattanDistance(waffle[i].v2Int, expression);

                // 
                if (computeD < dist)
                {
                    id = i;
                    dist = computeD;
                }
            }

            // 
            _waffleX = waffle[id].x;
            _waffleY = waffle[id].y;
        }
    }

    public void CheckUp(Map map)
    {
        UpdateClosestCandy(map.waffleTiles);

        // 
        if (_stunned && (GM.time > _lastStunned + 1))
        {
            UnStun();
        }
    }

    public void MoveTo(int time, Map map, Vector2 where)
    {
        map.PlaySFX("walk");

        Vector3 next = new Vector3(where.x, .5f, where.y);

        LookAt(next);

        _obj.transform.position = next;
        _opp.LookAtOpponent();

        // 
        map.SetCamTo(Map.camMode, myTurn);
    }

    int[][] Rewarder(Map map, int x, int y)
    {
        return new int[][] { new int[] { Reward(map, 0, y), Reward(map, 1, y), Reward(map, 2, y) }, new int[] { -Reward(map, x, 0), -Reward(map, x, 1), -Reward(map, x, 2) } };
    }

    int Reward(Map map, int x, int y)
    {
        int[] str = new int[] { map.fighter[0].str, map.fighter[1].str };

        if (x == 0)
        {
            if (y == 0)
            {
                return str[0] - str[1];
            }
            else if (y == 1)
            {
                return Mathf.FloorToInt(str[0] / 3) - 2;
            }
            else if (y == 2)
            {
                return str[0] * 2;
            }
            else
            {
                Debug.Log($"ERROR: [{x},{y}]");
                return 0;
            }
        }
        else if (x == 1)
        {
            if (y == 0)
            {
                return 2 - Mathf.FloorToInt(str[1] / 3);
            }
            else if (y == 1)
            {
                return 0;
            }
            else if (y == 2)
            {
                return str[1] < 20 ? -PowerUpRemap(str[1] - str[0]) : 0;
            }
            else
            {
                Debug.Log($"ERROR: [{x},{y}]");
                return 0;
            }
        }
        else if (x == 2)
        {
            if (y == 0)
            {
                return -str[1] * 2;
            }
            else if (y == 1)
            {
                return str[0] < 20 ? PowerUpRemap(str[0] - str[1]) : 0;
            }
            else if (y == 2)
            {
                return TauntOff(str[0], str[1]);
            }
            else
            {
                Debug.Log($"ERROR: [{x},{y}]");
                return 0;
            }
        }
        else
        {
            Debug.Log($"ERROR: [{x},{y}]");
            return 0;
        }
    }

    int TauntOff(int str0, int str1)
    {
        if (str0 == 1)
        {
            return str0 - str1;
        }
        else if (str1 == 1)
        {
            return str1 - str0;
        }
        else if (str0 == str1)
        {
            return 0;
        }
        else if (str0 < str1)
        {
            return 1;
        }
        else if (str1 < str0)
        {
            return -1;
        }
        else
        {
            Debug.Log($"ERROR: [{str0},{str1}]");
            return 0;
        }
    }

    int PowerUpRemap(int inp)
    {
        return inp <= 0 ? 2 : inp;
    }

    public void Battle(int time, OutputAttack[] oA, Map map)
    {

        // SAVE ALL OF THESE FOR A COOL REPLAY FEATURE

        //oA1.DEBUGSetVals(0, 1, 0);
        //oA2.DEBUGSetVals(0, 1, 0);

        // reset ran away after every battle
        _ranAway = 0;

        int[][] r = Rewarder(map, oA[0].decision, oA[1].decision);

        //Debug.Log($"0: [{oA[0].decision}->{r[0][oA[0].decision]}][{r[0][0]},{r[0][1]},{r[0][2]}]\n1: [{oA[1].decision}->{r[1][oA[1].decision]}][{r[1][0]},{r[1][1]},{r[1][2]}]");

        // compare & calculate
        if (oA[0].decision == 0)
        {
            // ATTACK
            if (oA[1].decision == 0)
            {
                // Both take damage
                opp.TakeDmg(str);
                TakeDmg(opp.str);

                // how much i did to them - how much they did to me
                //reward = new float[] { str - opp.str, opp.str - str };
                //reward[0] = new float[] { str - opp.str, };
            }
            else if (oA[1].decision == 1)
            {
                // 
                int shieldStr = (int)Mathf.Floor(str / 3);
                // oppponent takes a damage for every 3 str you have
                opp.TakeDmg(shieldStr);
                // take damage & is stunned
                TakeDmg(1);
                // You get stunned
                Stun();

                // how much i did to them - how much they did to me
                //reward = new float[] { shieldStr - 2, 2 - shieldStr };
            }
            else if (oA[1].decision == 2)
            {
                int crit = str * 2;
                // opponent takes damage x2
                opp.TakeDmg(crit);

                // how much i did to them - how much they did to me
                //reward = new float[] { crit, -crit };
            }
        }
        else if (oA[0].decision == 1)
        {
            // DEFEND

            if (oA[1].decision == 0)
            {
                // 
                int oppShieldStr = (int)Mathf.Floor(opp.str / 3);
                // you take a damage for every 3 str your opponent has
                TakeDmg((int)Mathf.Floor(oppShieldStr));
                // opponent takes damage
                _opp.TakeDmg(1);
                // 
                _opp.Stun();

                // how much i did to them - how much they did to me
                //reward = new float[] { 2 - oppShieldStr, oppShieldStr - 2 };
            }
            else if (oA[1].decision == 1)
            {
                // take one step away from each other

                // 
                Vector2Int s1 = v2Int;
                Vector2Int s2 = _opp.v2Int;

                // 
                StepBackwardsFrom(time, map, s2);
                _opp.StepBackwardsFrom(time, map, s1);

                // how much i did to them - how much they did to me
                //reward = new float[] { 0, 0 };
            }
            else if (oA[1].decision == 2)
            {
                // opponent powers up
                _opp.PowerUp();

                // how much i did to them - how much they did to me
                //reward = new float[] { 3 - 0, 0 - 3 };
            }
        }
        else if (oA[0].decision == 2)
        {
            // TAUNT

            if (oA[1].decision == 0)
            {
                // 
                int oppCrit = _opp.str * 2;
                // you take damage x2
                TakeDmg(oppCrit);

                // how much i did to them - how much they did to me
                //reward = new float[] { -oppCrit, oppCrit };
            }
            else if (oA[1].decision == 1)
            {
                // you power up
                PowerUp();

                // how much i did to them - how much they did to me
                //reward = new float[] { 3 - 0, 0 - 3 };
            }
            else if (oA[1].decision == 2)
            {
                // both powers down
                PowerDown();
                _opp.PowerDown();

                // how much i did to them - how much they did to me
                //reward = new float[] { strIsMin ? 0 : -1, opp.strIsMin ? 0 : -1 };
            }
        }

        string[] sD = new string[] { stateData.rawState, opp.stateData.rawState };

        //float[] oOut, hOut;
        //float[][] W_hidden, W_out;
        //OutputAttack.ConvertFightWeights(config, stateData, out oOut, out hOut, out W_hidden, out W_out);

        // FIX THIS TO PROCESS BOTH NN


        // 
        if (!config.isHuman && config.usingAttackNN)
        {
            int z = 0;
            NNState nn = oA[z].nn;
            float[] state = stateData.fullState;
            float[] error = new float[nn.O_out.Length];
            float[] fin = new float[nn.O_out.Length]; // assign this 

            float[] derivF = new float[nn.O_out.Length];
            float[] derivO = new float[nn.O_out.Length];
            float[] derivH = new float[nn.H_out.Length];

            float[][] D_out = new float[nn.O_out.Length][];
            float[][] D_hidden = new float[nn.H_out.Length][];

            //Debug.Log($"oOut: {nn.O_out.Length}, hOut: {nn.H_out.Length}, wOut: [{nn.W_out.Length}][{nn.W_out[0].Length}], wHidden: [{nn.W_hidden.Length}][{nn.W_hidden[0].Length}]");

            // 
            for (int i = 0; i < nn.W_hidden.Length; i++)
            {
                D_hidden[i] = new float[state.Length];
            }

            // 
            for (int i = 0; i < fin.Length; i++)
            {
                // 
                fin[i] = AI.Sigmoid(nn.O_out[i]);
                // 
                D_out[i] = new float[nn.H_out.Length];
            }

            float[][] newWH = new float[0][];
            float[][] newWO = new float[0][];

            // BackProp
            for (int i = 0; i < r[z].Length; i++) //3
            {
                // back prop
                error[i] = GM.lR * Mathf.Pow(r[z][i] * fin[i], 2);
                //Debug.Log($"{error[i]} = {GM.lR} * { Mathf.Pow(r[z][i] * fin[i], 2)}");

                derivF[i] = 2 * (GM.lR * Mathf.Pow(r[z][i], 2)) * fin[i];

                // 
                derivO[i] = AI.Sigmoid(nn.O_out[i], true) * derivF[i];
                //Debug.Log($"{derivO[i]} = {AI.Sigmoid(nn.O_out[i], true)} * {derivF[i]}");
                //
                for (int j = 0; j < D_out[i].Length; j++) //12
                {
                    D_out[i][j] = nn.H_out[j] * derivO[i];
                    //Debug.Log($"{D_out[i][j]} = {nn.H_out[j]} * {derivO[i]}");

                    derivH[j] = nn.W_out[i][j] * derivO[i];

                    // 
                    for (int k = 0; k < D_hidden[j].Length; k++) //20
                    {
                        D_hidden[j][k] = state[k] * derivH[j];
                        //Debug.Log($"{k}/{D_hidden[j].Length} - {D_out[i].Length}");
                        newWH = nn.W_hidden;
                        newWH[j][k] += D_hidden[j][k] * Mathf.Sign(r[z][i]);
                    }

                    newWO = nn.W_out;
                    newWO[i][j] += D_out[i][j] * Mathf.Sign(r[z][i]);

                    //Debug.Log($"{a == nn.W_out[i][j]} -> {a-nn.W_out[i][j]}");
                    //Debug.Log($"{D_out[i][j]} * {Mathf.Sign(reward[i])}");
                }
            }

            // if we haven't toggled off their ablility to learn
            if (GM.nnIsLearning[z])
            {
                config.UpdateAttack(newWH, newWO);
            }
        }

        if (opp.config.usingAttackNN)
        {
            int z = 1;
            NNState nn = oA[z].nn;
            float[] state = opp.stateData.fullState;
            float[] error = new float[nn.O_out.Length];
            float[] fin = new float[nn.O_out.Length]; // assign this 

            float[] derivF = new float[nn.O_out.Length];
            float[] derivO = new float[nn.O_out.Length];
            float[] derivH = new float[nn.H_out.Length];

            float[][] D_out = new float[nn.O_out.Length][];
            float[][] D_hidden = new float[nn.H_out.Length][];

            //Debug.Log($"oOut: {nn.O_out.Length}, hOut: {nn.H_out.Length}, wOut: [{nn.W_out.Length}][{nn.W_out[0].Length}], wHidden: [{nn.W_hidden.Length}][{nn.W_hidden[0].Length}]");

            // 
            for (int i = 0; i < nn.W_hidden.Length; i++)
            {
                D_hidden[i] = new float[state.Length];
            }

            // 
            for (int i = 0; i < fin.Length; i++)
            {
                // 
                fin[i] = AI.Sigmoid(nn.O_out[i]);
                // 
                D_out[i] = new float[nn.H_out.Length];
            }

            float[][] newWH = new float[0][];
            float[][] newWO = new float[0][];

            // BackProp
            for (int i = 0; i < r[z].Length; i++) //3
            {
                // back prop
                error[i] = GM.lR * Mathf.Pow(r[z][i] * fin[i], 2);
                //Debug.Log($"{error[i]} = {GM.lR} * { Mathf.Pow(r[z][i] * fin[i], 2)}");

                derivF[i] = 2 * (GM.lR * Mathf.Pow(r[z][i], 2)) * fin[i];

                // 
                derivO[i] = AI.Sigmoid(nn.O_out[i], true) * derivF[i];
                //Debug.Log($"{derivO[i]} = {AI.Sigmoid(nn.O_out[i], true)} * {derivF[i]}");
                //
                for (int j = 0; j < D_out[i].Length; j++) //12
                {
                    D_out[i][j] = nn.H_out[j] * derivO[i];
                    //Debug.Log($"{D_out[i][j]} = {nn.H_out[j]} * {derivO[i]}");

                    derivH[j] = nn.W_out[i][j] * derivO[i];

                    // 
                    for (int k = 0; k < D_hidden[j].Length; k++) //20
                    {
                        D_hidden[j][k] = state[k] * derivH[j];
                        //Debug.Log($"{k}/{D_hidden[j].Length} - {D_out[i].Length}");
                        newWH = nn.W_hidden;
                        newWH[j][k] += D_hidden[j][k] * Mathf.Sign(r[z][i]);
                    }

                    newWO = nn.W_out;
                    newWO[i][j] += D_out[i][j] * Mathf.Sign(r[z][i]);

                    //Debug.Log($"{a == nn.W_out[i][j]} -> {a-nn.W_out[i][j]}");
                    //Debug.Log($"{D_out[i][j]} * {Mathf.Sign(reward[i])}");
                }
            }

            // if we haven't toggled off their ablility to learn
            if (GM.nnIsLearning[z])
            {
                opp.config.UpdateAttack(newWH, newWO);
            }
        }

        //Debug.Log($"{r[0][0]},{r[0][1]},{r[0][2]} - {r[1][0]},{r[1][1]},{r[1][2]}");

        //
        using (StreamWriter sW = File.AppendText("masterLog.tsv"))
        {
            for (int i = 0; i < 2; i++)
            {
                sW.WriteLine($"{sD[i]}\t{oA[i].decision}\t{r[i][0]},{r[i][1]},{r[i][2]}");
                int currentReward = r[i][oA[i].decision];
                map.fC.UpdateGraph(i, currentReward);
                GM.battleAvgThisMatch[i].Add(currentReward);
            }
        }
    }

    public void RanAway()
    {
        _ranAway++;
    }

    public void CheckIfRanTooMuch()
    {
        if (ranAway >= GM.maxRunAway)
        {
            Stun(1);
            _ranAway = 0;
        }
    }

    public void SetText(bool toggle, Color c, bool add = true, int dmg = 0)
    {
        _dmgTxt.color = c;
        _dmgTxt.enabled = toggle;
        _dmgTxt.text = $"{(add ? '+' : '-')}{dmg}";

        if (!toggle)
        {
            _display.enabled = false;
        }
    }

    public void ChangeAnimation(string decide, string isNN = "", bool display = false)
    {
        _anim.SetTrigger(decide);

        // 
        if (display)
        {
            _display.enabled = true;
            Material main = decide[0] == 'A' ? FightCTRL.txtBattle[0] : decide[0] == 'D' ? FightCTRL.txtBattle[1] : FightCTRL.txtBattle[2];
            Material side = isNN == "nn" ? FightCTRL.txtDecide[1] : isNN == "r" ? FightCTRL.txtDecide[0] : null;

            _display.materials = new Material[] { main, side };
        }
    }

    // 
    void StepBackwardsFrom(int time, Map map, Vector2Int from)
    {
        Vector2Int back = v2Int - (from - v2Int);

        int theX = back.x + map.halfMapSize;
        int theY = back.y + map.halfMapSize;

        bool xIsOkay = theX >= 0 && theX <= map.mapSize - 1;
        bool yIsOkay = theY >= 0 && theY <= map.mapSize - 1;

        // 
        if (xIsOkay && yIsOkay && map.loc[theX, theY].free)
        {
            MoveTo(time, map, back);
        }

        LookAtOpponent();
    }

    public void LookAt(Vector3 t)
    {
        obj.transform.LookAt(t);
    }

    public void LookAtOpponent()
    {
        obj.transform.LookAt(_opp.obj.transform);
    }

    public void PowerUp()
    {
        _powerUp.Play();
        int tS = _str;
        _str++;
        _str = Mathf.Clamp(_str, 1, GM.maxStr);
        _strTxt.text = $"{_str}";
        SetText(true, Color.yellow, dmg: _str - tS);
    }

    public void PowerDown()
    {
        _powerDown.Play();
        SetText(true, Color.yellow, false, dmg: strIsMin ? 0 : 1);
        _str--;
        _str = Mathf.Clamp(_str, 1, GM.maxStr);
        _strTxt.text = $"{_str}";
    }

    void Stun(int offset = 0)
    {
        _stunned = true;
        _lastStunned = GM.time + offset;
        _stunSpr.enabled = _stunned;
    }

    void UnStun()
    {
        _stunned = false;
        _stunSpr.enabled = _stunned;
    }

    public void DMGTEMP(int dmg)
    {
        TakeDmg(dmg);
    }

    void TakeDmg(int dmg)
    {
        SetText(true, Color.red, false, dmg);
        _hp -= dmg;
        _hp = Mathf.Clamp(_hp, 0, GM.maxHP);

        _hpHolder.localScale = new Vector3(hpPercent, 1, 1);
    }

    public void SetOpponent(Fighter opp)
    {
        _opp = opp;
        stateData = new StateData(myTurn, this, _opp);
    }

    public void EatWaffle()
    {
        _hp += 7;

        _hp = Mathf.Clamp(_hp, 0, GM.maxHP);

        _hpHolder.localScale = new Vector3(hpPercent, 1, 1);
    }
}

using System;
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

    int _candyX;
    int _candyY;

    public int candyX => _candyX;
    public int candyY => _candyY;

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

    public void UpdateClosestCandy(List<Tile> candies)
    {
        // 
        if (candies.Count == 0)
        {
            _candyX = -GM.mapSize;
            _candyY = -GM.mapSize;
        }
        else
        {
            int id = -1;
            int dist = 1000;

            // 
            for (int i = 0; i < candies.Count; i++)
            {
                int computeD = Map.ManhattanDistance(candies[i].v2Int, expression);

                // 
                if (computeD < dist)
                {
                    id = i;
                    dist = computeD;
                }
            }

            // 
            _candyX = candies[id].x;
            _candyY = candies[id].y;
        }
    }

    public void CheckUp(Map map)
    {
        UpdateClosestCandy(map.candyTiles);

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
        map.SetCamTo(map.camMode, myTurn);
    }

    public void Battle(int time, OutputAttack[] oA, Map map)
    {

        // SAVE ALL OF THESE FOR A COOL REPLAY FEATURE

        //oA1.DEBUGSetVals(0, 1, 0);
        //oA2.DEBUGSetVals(0, 1, 0);

        // reset ran away after every battle
        _ranAway = 0;

        // compare & calculate
        if (oA[0].decision == 0)
        {
            // ATTACK

            if (oA[1].decision == 0)
            {
                // Both take damage
                opp.TakeDmg(str);
                TakeDmg(_opp.str);
                //Debug.Log($"({time}) {obj.name} -> <- {_opp.obj.name} ATT: {obj.name} HP: {hp} - {_opp.obj.name} HP: {_opp.hp}");
            }
            else if (oA[1].decision == 1)
            {
                // oppponent takes a damage for every 3 str you have
                opp.TakeDmg((int)Mathf.Floor(str / 3));
                // take damage & is stunned
                TakeDmg(1);
                //Debug.Log($"({time}) {_opp.obj.name} blocked {obj.name} for {_opp.str}. {obj.name} HP = {hp}");
                Stun();
            }
            else if (oA[1].decision == 2)
            {
                // opponent takes damage x2
                opp.TakeDmg(str * 2);
                //Debug.Log($"({time}) {obj.name} crit {_opp.obj.name} for {str}*2 = HP: {_opp.hp}");
            }
        }
        else if (oA[0].decision == 1)
        {
            // DEFEND

            if (oA[1].decision == 0)
            {
                // you take a damage for every 3 str your opponent has
                TakeDmg((int)Mathf.Floor(opp.str / 3));
                // opponent takes damage
                _opp.TakeDmg(1);
                //Debug.Log($"({time}) {obj.name} blocked {_opp.obj.name} for {str}. {_opp.obj.name} HP = {_opp.hp}");
                _opp.Stun();
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
            }
            else if (oA[1].decision == 2)
            {
                // opponent powers up
                _opp.PowerUp();
            }
        }
        else if (oA[0].decision == 2)
        {
            // TAUNT

            if (oA[1].decision == 0)
            {
                // you take damage x2
                TakeDmg(_opp.str * 2);
            }
            else if (oA[1].decision == 1)
            {
                // you power up
                PowerUp();
            }
            else if (oA[1].decision == 2)
            {
                // both powers down
                PowerDown();
                _opp.PowerDown();
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

    public void ChangeAnimation(string a, bool display = false)
    {
        _anim.SetTrigger(a);

        // 
        if (display)
        {
            _display.enabled = true;
            _display.material = a[0] == 'A' ? FightCTRL.txts[0] : a[0] == 'D' ? FightCTRL.txts[1] : FightCTRL.txts[2];
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
        _str++;
        _str = Mathf.Clamp(_str, 1, GM.maxStr);
        _strTxt.text = $"{_str}";
        SetText(true, Color.yellow, dmg: 1);
    }

    public void PowerDown()
    {
        _powerDown.Play();
        _str--;
        _str = Mathf.Clamp(_str, 1, GM.maxStr);
        _strTxt.text = $"{_str}";
        SetText(true, Color.yellow, false, dmg: 1);
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

    public void EatCandy()
    {
        _hp += 7;

        _hp = Mathf.Clamp(_hp, 0, GM.maxHP);

        _hpHolder.localScale = new Vector3(hpPercent, 1, 1);
    }
}

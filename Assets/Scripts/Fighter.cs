using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Fighter
{
    GameObject _obj;
    public GameObject obj => _obj;

    Fighter _opp;
    public StateData stateData;

    public int x => Mathf.RoundToInt(obj.transform.position.x);
    public int y => Mathf.RoundToInt(obj.transform.position.z);

    public int eX => x + (mapSize / 2);
    public int eY => y + (mapSize / 2);

    int _myTurn;
    public int myTurn => _myTurn;

    int _hp = GM.maxHP;
    public int hp => _hp;

    int _str = 2;
    public int str => _str;

    int _lastStunned = -1;

    public Vector2Int v2Int => new Vector2Int(x, y);
    public Vector2Int expression => new Vector2Int(eX, eY);

    int distToOpp => Map.ManhattanDistance(v2Int, _opp.v2Int);
    bool _stunned;
    public bool stunned => _stunned;

    public bool inAttackRange => distToOpp == 1;

    int mapSize;

    public Fighter(GameObject gO, int myTurn, int mapSize)
    {
        _obj = gO;
        this.mapSize = mapSize;
        _myTurn = myTurn;
    }

    public void CheckUp(int time)
    {
        // 
        if (_stunned && (time > _lastStunned + 1))
        {
            UnStun(time);
        }
    }

    public void MoveTo(int time, Map map, Vector2 where)
    {
        Vector3 next = new Vector3(where.x, .5f, where.y);

        LookAt(next);

        _obj.transform.position = next;
        _opp.LookAtOpponent();

        // 
        if (map.camMode == Map.CamMode.Isometric)
        {
            map.SetCamTo(Map.CamMode.Isometric, myTurn);
        }
    }

    public void Battle(int time, Map map)
    {
        // pass in state, output 1 2 or 3
        OutputAttack oA1 = OutputAttack.CalculateOutput(_stunned, stateData);

        // ask opp for their battle output
        OutputAttack oA2 = OutputAttack.CalculateOutput(_opp._stunned, _opp.stateData);

        // SAVE ALL OF THESE FOR A COOL REPLAY FEATURE

        oA1.DEBUGSetVals(0, 1, 0);
        oA2.DEBUGSetVals(0, 1, 0);

        // compare & calculate
        if (oA1.decision == 0)
        {
            // ATTACK

            if (oA2.decision == 0)
            {
                // Both take damage
                _opp.TakeDmg(str);
                TakeDmg(_opp.str);
                //Debug.Log($"({time}) {obj.name} -> <- {_opp.obj.name} ATT: {obj.name} HP: {hp} - {_opp.obj.name} HP: {_opp.hp}");
            }
            else if (oA2.decision == 1)
            {
                // take damage & is stunned
                TakeDmg(_opp.str);
                //Debug.Log($"({time}) {_opp.obj.name} blocked {obj.name} for {_opp.str}. {obj.name} HP = {hp}");
                Stun(time);
            }
            else if (oA2.decision == 2)
            {
                // opponent takes damage x2
                _opp.TakeDmg(str*2);
                //Debug.Log($"({time}) {obj.name} crit {_opp.obj.name} for {str}*2 = HP: {_opp.hp}");
            }
        }
        else if (oA1.decision == 1)
        {
            // DEFEND

            if (oA2.decision == 0)
            {
                // opponent takes damage
                _opp.TakeDmg(str);
                //Debug.Log($"({time}) {obj.name} blocked {_opp.obj.name} for {str}. {_opp.obj.name} HP = {_opp.hp}");
                _opp.Stun(time);
            }
            else if (oA2.decision == 1)
            {
                // take one step away from each other
                //Debug.Log($"({time}) Both took 1 step away from each other");

                // 
                Vector2Int s1 = v2Int;
                Vector2Int s2 = _opp.v2Int;

                // 
                StepBackwardsFrom(time, map, s2);
                _opp.StepBackwardsFrom(time, map, s1);
            }
            else if (oA2.decision == 2)
            {
                // opponent powers up
                _opp.PowerUp();
                //Debug.Log($"({time}) {_opp.obj.name} powered up +1 = {_opp.str} str");
            }
        }
        else if (oA1.decision == 2)
        {
            // TAUNT

            if (oA2.decision == 0)
            {
                // you take damage x2
                TakeDmg(_opp.str*2);
                //Debug.Log($"({time}) {_opp.obj.name} crit {obj.name} for {_opp.str}*2 = HP: {hp}");
            }
            else if (oA2.decision == 1)
            {
                // you power up
                PowerUp();
                //Debug.Log($"({time}) {obj.name} powered up +1 = {str} str");
            }
            else if (oA2.decision == 2)
            {
                // both powers down
                PowerDown();
                _opp.PowerDown();
                //Debug.Log($"({time}) Both powered down -1. {_opp.obj.name} = {_opp.str} str : {obj.name} = {str} str");
            }
        }
    }

    // 
    void StepBackwardsFrom(int time, Map map, Vector2Int from)
    {
        Vector2Int back = v2Int - (from - v2Int);

        int theX = back.x + map.halfMapSize;
        int theY = back.y + map.halfMapSize;

        bool xIsOkay = theX >= 0 && theX <= map.mapSize-1;
        bool yIsOkay = theY >= 0 && theY <= map.mapSize-1;

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
        _str++;
        _str = Mathf.Clamp(_str, 1, GM.maxStr);
    }

    public void PowerDown()
    {
        _str--;
        _str = Mathf.Clamp(_str, 1, GM.maxStr);
    }

    void Stun(int time)
    {
        _stunned = true;
        _lastStunned = time;
        //Debug.Log($"({time}) {obj.name} is now STUNNED!");
    }

    void UnStun(int time)
    {
        _stunned = false;
        //Debug.Log($"({time}) {obj.name} is now UNSTUNNED!");
    }

    void TakeDmg(int dmg)
    {
        _hp -= dmg;
    }

    public void SetOpponent(Fighter opp)
    {
        _opp = opp;
        stateData = new StateData(myTurn, this, _opp);
    }

    public void Rest(float time)
    {
        _hp++;

        _hp = Mathf.Clamp(_hp, 0, GM.maxHP);
        //Debug.Log($"({time}) {obj.name} has rested to: {hp}!");
    }
}

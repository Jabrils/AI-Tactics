using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct OutputMove
{
    float _distance, _angleX, _angleY;

    public float distance => _distance;
    public float angleX => _angleX;
    public float angleY => _angleY;

    OutputMove(float d, float aX, float aY)
    {
        _distance = d;
        _angleX = aX;
        _angleY = aY;
    }

    /// <summary>
    /// this takes in state data, & spits out a prediction on the best move to make
    /// </summary>
    /// <param name="inp"></param>
    /// <returns></returns>
    public static OutputMove CalculateOutput(Fighter fighter)
    {
        float d = UnityEngine.Random.value;
        float aX = UnityEngine.Random.Range(-1f, 1f);
        float aY = UnityEngine.Random.Range(-1f, 1f);

        // 
        //if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Run)
        //{
        //    d = 1;
        //    aX = UnityEngine.Random.Range(-1f, 1f);
        //    aY = UnityEngine.Random.Range(-1f, 1f);
        //}
        //else if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Pursue)
        //{
        //    d = 0;
        //    aX = UnityEngine.Random.Range(-1f, 1f);
        //    aY = UnityEngine.Random.Range(-1f, 1f);
        //}
        //else if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Random)
        //{
        //    d = UnityEngine.Random.value;
        //    aX = UnityEngine.Random.Range(-1f, 1f);
        //    aY = UnityEngine.Random.Range(-1f, 1f);
        //}

        d = fighter.config.distance == -1 ? d : fighter.config.distance;
        aX = fighter.config.distance == 0 ? aX : fighter.config.angleX;
        aY = fighter.config.distance == 0 ? aY : fighter.config.angleY;

        return new OutputMove(d, aX, aY);
    }
}

public struct OutputToBattle
{
    float _decision;
    public bool toBattle => _decision > .5f;

    OutputToBattle(float d)
    {
        _decision = d;
    }

    public static OutputToBattle CalculateOutput(Fighter fighter)
    {
        float ret = -1;

        //// 
        //if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Run)
        //{
        //    ret = 0;
        //}
        //else if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Pursue)
        //{
        //    ret = 1;
        //}
        //else if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Random)
        //{
        //    ret = UnityEngine.Random.value;
        //}

        ret = fighter.config.toBattle == true ? 1 : 0;

        return new OutputToBattle(ret);
    }
}

public struct OutputAttack
{
    float _attack, _defend, _taunt;
    public float attack => _attack;
    public float defend => _defend;
    public float taunt => _taunt;

    float[] votes => new float[] { attack, defend, taunt };
    public int decision => Decision();
    public string choice => decision == 0 ? "Attack" : decision == 1 ? "Defend" : "Taunt";

    OutputAttack(float a, float d, float t)
    {
        _attack = a;
        _defend = d;
        _taunt = t;
    }

    public void DEBUGSetVals(float a, float d, float t)
    {
        _attack = a;
        _defend = d;
        _taunt = t;
    }

    public static OutputAttack CalculateOutput(bool stunned, Fighter fighter)
    {
        float a = UnityEngine.Random.value;
        float d = UnityEngine.Random.Range(0, 1 - a);
        float t = 1 - (a + d);

        // 
        if (fighter.config.attack || fighter.config.defend || fighter.config.taunt)
        {
            a = fighter.config.attack == true ? 1 : 0;
            d = fighter.config.defend == true ? 1 : 0;
            t = fighter.config.taunt == true ? 1 : 0;
        }

        // MOVE IS STUNNED FUNCT INTO HERE INSTEAD, PASS IN BOOL
        if (stunned)
        {
            // check if defense is higher than taunt
            if (d > t)
            {
                a = 0;
                d = 1;
                t = 0;
            }
            // else that means that taunt is higher than defense, & we can set taunt
            else
            {
                a = 0;
                d = 0;
                t = 1;
            }

        return new OutputAttack(a, d, t);
        }

        // 
        return new OutputAttack(a, d, t);
    }

    public static OutputAttack ForceOutput(int w)
    {
        float[] ret = new float[3];

        ret[w] = 1;

        return new OutputAttack(ret[0], ret[1], ret[2]);
    }

    int Decision()
    {
        int ret = 0;
        float highest = 0;

        for (int i = 0; i < votes.Length; i++)
        {
            if (votes[i] > highest)
            {
                ret = i;
                highest = votes[i];
            }
        }

        return ret;
    }
}

public struct OutputStay
{
    float _stay;
    public bool stay => UnityEngine.Mathf.Round(_stay) == 0;

    public OutputStay(float r)
    {
        _stay = r;
    }

    public static OutputStay Calculate(Fighter fighter)
    {
        float ret = -1;

        if (fighter.isStunned)
        {
            return new OutputStay(0);
        }

        // 
        //if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Run)
        //{
        //    ret = 1;
        //}
        //else if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Pursue)
        //{
        //    ret = 1;
        //}
        //else if (GM.inpType[fighter.myTurn] == FightCTRL.InputType.Random)
        //{
        //    ret = UnityEngine.Random.value;
        //}

        ret = fighter.config.move == true ? 1 : 0;

        return new OutputStay(ret);
    }
}

public struct StateData
{
    public StateData(int turn, Fighter me, Fighter opp)
    {
        this.me = me;
        this.opp = opp;
        theTurn = turn;
    }

    public void Update(int turn)
    {
        theTurn = turn;
    }

    public void PrintState()
    {
        Debug.Log($"dX: {distX}, dY: {distY}\nmyT: {myTurn}, mX: {myX}, mY: {myY}, mH: {myHP}, mS: {myStr}, iS: {iStunned}" +
        $"\noT: {oppTurn}, oX: {oppX}, oY: {oppY}, oH: {oppHP}, oS: {oppStr}, oS: {oppStunned}");
    }

    public Fighter me;
    public Fighter opp;
    int theTurn;

    // 
    // // SHARED
    // 

    // 1- dist x
    public float distX => UnityEngine.Mathf.Abs(me.x - opp.x) / ((float)GM.mapSize - 1);
    // 2- dist y
    public float distY => UnityEngine.Mathf.Abs(me.y - opp.y) / ((float)GM.mapSize - 1);

    // 
    // // MINE
    //

    // 3- myTurn
    public int myTurn => theTurn == me.myTurn ? 1 : 0;
    // 4- myX
    public float myX => (float)me.eX / (float)GM.mapSize;
    // 5- myY
    public float myY => (float)me.eY / (float)GM.mapSize;
    // 6- myHp
    public float myHP => (float)me.hp / (float)GM.maxHP;
    // 7- myStr
    public float myStr => (float)me.str / (float)GM.maxStr;
    // 8- iStunned
    public int iStunned => me.isStunned ? 1 : 0;

    // 
    // // OPPONENT
    // 

    // 9- oppTurn
    public int oppTurn => myTurn == 0 ? 1 : 0;
    // 10- oppX
    public float oppX => (float)opp.eX / (float)GM.mapSize;
    // 11- oppY
    public float oppY => (float)opp.eY / (float)GM.mapSize;
    // 12- oppHp
    public float oppHP => (float)opp.hp / (float)GM.maxHP;
    // 13- oppStr
    public float oppStr => (float)opp.str / (float)GM.maxStr;
    // 14- oppStunned
    public int oppStunned => opp.isStunned ? 1 : 0;
}
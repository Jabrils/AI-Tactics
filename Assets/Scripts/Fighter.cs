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

    public int x => Mathf.RoundToInt(obj.transform.position.x);
    public int y => Mathf.RoundToInt(obj.transform.position.z);

    public int eX => x + (mapSize / 2);
    public int eY => y + (mapSize / 2);

    int _hp = 100;
    public int hp => _hp;

    int _str = 2;

    public Vector2Int v2Int => new Vector2Int(x, y);
    public Vector2Int expression => new Vector2Int(eX, eY);

    int distToOpp => Map.ManhattanDistance(v2Int, _opp.v2Int);

    public bool inAttackRange => distToOpp == 1;

    int mapSize;

    public Fighter(GameObject gO, int mapSize)
    {
        _obj = gO;
        this.mapSize = mapSize;
    }

    public void MoveTo(Vector2 where)
    {
        _obj.transform.position = new Vector3(where.x, 1.5f, where.y);
    }

    public void Battle()
    {
        // pass in state, output 1 2 or 3

        // ask opp for their battle output

        // compare & calculate

        // apply results

        _opp.TakeDmg(_str);
    }

    void TakeDmg(int dmg)
    {
        _hp -= dmg;

        Debug.Log($"{obj.name} - {hp}");
    }

    public void SetOpponent(Fighter opp)
    {
        _opp = opp;
    }
}

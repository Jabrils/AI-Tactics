using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Config
{
    public bool[] neuralNetwork => new bool[] { stayNN.Length > 10, movementNN.Length > 10, battleNN.Length > 10, attackNN.Length > 10 };

    string _stay, _movement, _battle, _attack, _defend, _taunt;
    public float stayTrad => float.Parse(_stay);
    public float movementTrad => float.Parse(_movement);
    public float battleTrad => float.Parse(_battle);
    public float attackTrad => float.Parse(_attack);
    public float defendTrad => float.Parse(_defend);
    public float tauntTrad => float.Parse(_taunt);

    public string[] stayNN => _stay.Split(',');
    public string[] movementNN => _movement.Split(',');
    public string[] battleNN => _battle.Split(',');
    public string[] attackNN => _attack.Split(',');

    public AI_Config(string data)
    {
        string[] split = data.Split(';');

        _stay = split[0];
        _movement = split[1];
        _battle = split[2];

        string[] last = split[3].Split(',');

        // 
        if (last.Length == 3)
        {
            _attack = last[0];
            _defend = last[1];
            _taunt = last[2];
        }
        else
        {
            _attack = split[3];
        }
    }
}

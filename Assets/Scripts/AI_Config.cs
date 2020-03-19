using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AI_Config
{
    public bool[] neuralNetwork => new bool[] { stayNN.Length > 10, movementNN.Length > 10, battleNN.Length > 10, attackNN.Length > 10 };

    public bool usingAttackNN => neuralNetwork[3];

    string _raw, _stay, _movement, _battle, _attack, _defend, _taunt, _filename, _aiName;
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
    float _exploit = 1f;
    public float exploit => _exploit;

    public AI_Config(string fname, string data)
    {
        string[] split = data.Split('\n');
        _aiName = split[0];
        _raw = split[1];

        _filename = fname;

        split = _raw.Split(';');

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

    public void UpdateAttack(float[][] wH, float[][] wO)
    {
        string add = "";

        for (int i = 0; i < wH.Length; i++)
        {
            for (int j = 0; j < wH[i].Length; j++)
            {
                add += $"{wH[i][j]},";
            }
        }

        for (int i = 0; i < wO.Length; i++)
        {
            for (int j = 0; j < wO[i].Length; j++)
            {
                add += $"{wO[i][j]},";
            }
        }

        add = add.Remove(add.Length-1);

        _attack = add;

        SaveIntelligence();
    }

    public void SaveIntelligence()
    {
        string[] save = _raw.Split(';');

        save[3] = _attack;

        string concat0 = $"{_aiName}\n";
        string concat1 = string.Join(";", save);

        // 
        using (StreamWriter sR = new StreamWriter(_filename))
        {
            sR.Write(concat0+concat1);
        }
    }

    public void SetExploit(float e)
    {
        _exploit = e;
    }
}

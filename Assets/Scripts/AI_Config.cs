using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Config
{
    public bool[] neuralNetwork => new bool[] { stay == -1, movement == -1, attack == -1};
    public float stay = -1, movement = -1, battle = -1, attack = -1, defend = -1, taunt = -1;

    public AI_Config(string data)
    {
        string[] split = data.Split(';');

        stay = float.Parse(split[0]);
        movement = float.Parse(split[1]);
        battle = float.Parse(split[2]);

        string[] last = split[3].Split(',');

        attack = float.Parse(last[0]);
        defend = float.Parse(last[1]);
        taunt = float.Parse(last[2]);
    }
}

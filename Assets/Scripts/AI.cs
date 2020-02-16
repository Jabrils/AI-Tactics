using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct OutputMove
{
    public float distance, angleX, angleY;

    OutputMove(float d, float aX, float aY)
    {
        distance = d;
        angleX = aX;
        angleY = aY;
    }

    /// <summary>
    /// this takes in state data, & spits out a prediction on the best move to make
    /// </summary>
    /// <param name="inp"></param>
    /// <returns></returns>
    public static OutputMove CalculateOutput(StateData inp)
    {
        float d = UnityEngine.Random.value;
        float aX = UnityEngine.Random.Range(-1f, 1f);
        float aY = UnityEngine.Random.Range(-1f, 1f);

        return new OutputMove(d, aX, aY);
    }
}

public struct OutputToBattle
{
    public float decision;

    OutputToBattle(float d)
    {
        decision = d;
    }

    public static OutputToBattle CalculateOutput(StateData inp)
    {
        float d = UnityEngine.Random.value;

        return new OutputToBattle(d);
    }
}

public struct StateData
{

}
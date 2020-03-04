using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "A", menuName ="B")]
public class AI_Config : ScriptableObject
{
    public bool move, toBattle;
    public int distance;
    public float angleX, angleY;
    public bool attack, defend, taunt;
}

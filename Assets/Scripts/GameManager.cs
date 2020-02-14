using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class GM
{
    public static int maxMoves = 5;
    public static string lvlExt = ".map";

    public static float XYtoDeg(float x, float y)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }
}

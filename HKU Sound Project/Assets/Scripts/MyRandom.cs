using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRandom
{
    public static int NoRepeatRange(int min, int max, int last)
    {
        int result = Random.Range(min, max);
        if(result == last)
        {
            result = max - last - 1;
        }
        return result;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DifficultyCurve
{
    public float GetDifficulty(int x);
}

public class Difficulty : MonoBehaviour, DifficultyCurve
{
    public float a;
    
    public float GetDifficulty(int x)
    {
        return a * (x + 1);
    }
}

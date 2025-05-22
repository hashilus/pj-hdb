using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Mathh
{
    static System.Random random = new System.Random();

    public static Vector3 GetPosOnCircle(float height, float degree, float radius)
    {
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * degree), height, Mathf.Cos(Mathf.Deg2Rad * degree)) * radius;
    }

    public static T WeightedRandom<T>(IEnumerable<T> values, IEnumerable<int> weights)
    {
        // https://stackoverflow.com/a/1761646

        var sumOfWeight = 0;
        var count = values.Count();
        for (var i = 0; i < count; i++)
        {
            sumOfWeight += weights.ElementAt(i);
        }

        var randomValue = random.Next(sumOfWeight);
        for (var i = 0; i < count; i++)
        {
            var weight = weights.ElementAt(i);
            if (randomValue < weight)
            {
                return values.ElementAt(i);
            }
            randomValue -= weight;
        }

        throw new Exception();
    }

    public static float LinearMap(float value, float s0, float s1, float d0, float d1)
    {
        return d0 + (value - s0) * (d1 - d0) / (s1 - s0);
    }

    public static float LinearClampedMap(float value, float s0, float s1, float d0, float d1)
    {
        if (d0 < d1)
        {
            return Mathf.Clamp(LinearMap(value, s0, s1, d0, d1), d0, d1);
        }
        return Mathf.Clamp(LinearMap(value, s0, s1, d0, d1), d1, d0);
    }
}

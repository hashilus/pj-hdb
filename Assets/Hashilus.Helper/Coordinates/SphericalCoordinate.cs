using UnityEngine;

// https://blog.nobel-joergensen.com/2010/10/22/spherical-coordinates-in-unity/
public class SphericalCoordinate
{
    public float radius;
    public float elevation;
    public float polar;

    public SphericalCoordinate() { }

    public SphericalCoordinate(Vector3 cartesian)
    {
        var x = cartesian.x;
        var y = cartesian.y;
        var z = cartesian.z;

        if (x == 0)
        {
            x = Mathf.Epsilon;
        }

        radius = Mathf.Sqrt(x * x + y * y + z * z);
        elevation = Mathf.Asin(y / radius);
        polar = Mathf.Atan(z / x);

        if (x < 0)
        {
            polar += Mathf.PI;
        }
    }

    public Vector3 ToCartesian()
    {
        var a = radius * Mathf.Cos(elevation);
        return new Vector3(a * Mathf.Cos(polar), radius * Mathf.Sin(elevation), a * Mathf.Sin(polar));
    }
}

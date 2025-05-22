using UnityEngine;

// https://en.wikipedia.org/wiki/Vector_fields_in_cylindrical_and_spherical_coordinates
public class CylindricalYCoordinate
{
    public float radius;
    public float height;
    public float theta;

    public CylindricalYCoordinate()
    {

    }

    public CylindricalYCoordinate(Vector3 cartesian)
    {
        var x = cartesian.x;
        var y = cartesian.y;
        var z = cartesian.z;

        if (x == 0)
        {
            x = Mathf.Epsilon;
        }

        radius = Mathf.Sqrt(x * x + z * z);
        height = y;
        theta = Mathf.Atan(z / x);

        if (x < 0)
        {
            theta += Mathf.PI;
        }
    }

    public Vector3 ToCartesian()
    {
        return new Vector3(radius * Mathf.Cos(theta), height, radius * Mathf.Sin(theta));
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetsManager
{
    public Vector3 getPlanetPositionByName(String planetName)
    {
        var vector = new Vector3();

        vector.x = 0.02f;
        vector.y = 0.03f;
        vector.z = 0.4f;

        return vector;
 
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears
{
    public Car car;
    public int gear;
    public float gearratio;
    public AnimationCurve engineresistance;

    public Gears(Car ncar, int ngear, float ngearratio)
    {
        car = ncar;
        gear = ngear;
        gearratio = ngearratio;
    }
}

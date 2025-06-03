using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : IcarInitializer
{
    //should be worked on, didnt put its values that can be changed in the CarObjects class, so i tweak with this class in the future
    public Car car;
    public int gear;
    public float gearratio;
    public AnimationCurve engineresistance;

    public void Initialize(Car ccar)
    {
        this.car = ccar;
    }

    public Gears(Car ncar, int ngear, float ngearratio)
    {
        car = ncar;
        gear = ngear;
        gearratio = ngearratio;
    }
}

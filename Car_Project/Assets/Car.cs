using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    //statically put
    public float MaxTorque = 656;//Nm max torque out of crankshaft(engine torque)
    public AnimationCurve Torque;// the torque curve coming out of the crankshaft
    public float BrakeTorque = 5000f;
    public float HandBrakeTorque = 3000f;
    public float SteeringMax = 27;
    public float MaxSpeed = 320f;
    public AnimationCurve SteeringCurve;
    public float Tractioncutoff = 0.35f;
    public float Finaldriveaxle = 3.7f;
    public int Drivingwheels = 2;//rwd

    //should get it from the game, even though i statically put it in the game
    public float Mass;// should be 1800
    public float Wheelradius;//0.45m (ig) in the first car
    public float Wheelmass;//36kg in the first car

}

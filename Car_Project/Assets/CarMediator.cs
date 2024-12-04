using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMediator
{
    //mediator between powertowheels/carcontroller and Car

    //need to be put in every car object, because i want to create a Car instance inside each car, while keeping the low coupling between the Car class and the other side of the mediator
    private Car car;

    public void Initialize(Car carInstance)
    {
        car = carInstance;
    }

    public float GetHandBrakeTorque()
    {
        return car.HandBrakeTorque;
    }

    //to fix after
    /*
    public float GetDecelerationFactor()
    {
        return car.decelerationFactor;
    }
    */
    public float GetBrakeTorque()
    {
        return car.BrakeTorque;
    }

    public void SetBrakeTorque(WheelCollider wheel, float torque)
    {
        wheel.brakeTorque = torque;
    }

    public WheelFrictionCurve GetSidewaysFriction(WheelCollider wheel)
    {
        return wheel.sidewaysFriction;
    }

    public void UpdateFriction(WheelCollider wheel, WheelFrictionCurve friction)
    {
        wheel.sidewaysFriction = friction;
    }

    public float GetMaxTorque()
    {
        return car.MaxTorque;
    }

    public float GetTractionCutoff()
    {
        return car.TractionCutoff;
    }

    public float GetWheelRadius()
    {
        return car.WheelRadius;
    }
}

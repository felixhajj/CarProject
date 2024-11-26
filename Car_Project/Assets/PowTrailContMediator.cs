using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowTrailContMediator
{
    //mediator between PowerToWheels/VehiculeTrailEffect and CarController
    private CarController carController;

    public void Initialize(CarController controller)
    {
        carController = controller;
    }

    public WheelCollider GetWheelCollider(string position)
    {
        return position.ToLower() switch
        {
            "rearleft" => carController.rearleft,
            "rearright" => carController.rearright,
            "frontleft" => carController.frontleft,
            "frontright" => carController.frontright,
            _ => throw new ArgumentException("Invalid wheel position")
        };
    }

    public bool IsEngineTurnedOn()
    {
        return carController.engineturnon;
    }

    public void SetBrakeTorque(WheelCollider wheel, float torque)
    {
        wheel.brakeTorque = torque;
    }

    public void UpdateFriction(WheelCollider wheel, WheelFrictionCurve friction)
    {
        wheel.forwardFriction = friction;
    }

    public AudioSource GetEngineAudioSource()
    {
        return carController.engineAudioSource;
    }
    /*
    public float GetBrakeTorque()
    {
        return carController.BrakeTorque;
    }

    // Getter for decelerationFactor
    public float GetDecelerationFactor()
    {
        return carController.decelerationFactor;
    }
    */
    public AudioSource GetTireScreechAudioSource()
    {
        return carController.tirescreechAudioSource;
    }

    public AudioClip GetTireScreechingEndClip()
    {
        return carController.tirescreechingendClip;
    }

    // Access wheel ground states
    public Dictionary<WheelCollider, int> GetWheelGroundStates()
    {
        return carController.wheelGroundStates;
    }

    // Get wheel GameObject references
    public Transform GetWheelTransform(string position)
    {
        return position.ToLower() switch
        {
            "rearleft" => carController.rearleft.transform,
            "rearright" => carController.rearright.transform,
            "frontleft" => carController.frontleft.transform,
            "frontright" => carController.frontright.transform,
            _ => throw new ArgumentException("Invalid wheel position")
        };
    }

}

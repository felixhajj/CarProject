using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class Car
{
    public float MaxTorque { get; set; } // Nm max torque out of crankshaft (engine torque)
    public AnimationCurve Torque { get; set; } // Torque curve coming out of the crankshaft
    public float BrakeTorque { get; set; }
    public float HandBrakeTorque { get; set; }
    public float SteeringMax { get; set; }
    public float MaxSpeed { get; set; }
    public AnimationCurve SteeringCurve { get; set; }
    public float TractionCutoff { get; set; }
    public float FinalDriveAxle { get; set; }
    public int DrivingWheels { get; set; } // Number of driving wheels (e.g., 2 for RWD)

    // Properties to be set dynamically based on the game environment
    public float Mass { get; set; } // Mass of the car in kg
    public float WheelRadius { get; set; } // Radius of the wheel in meters
    public float WheelMass { get; set; } // Mass of the wheel in kg

}

public class CarBuilder
{
    private Car _car= new Car();

    public CarBuilder SetMaxTorque(float maxTorque)
    {
        _car.MaxTorque = maxTorque;
        return this;
    }
    public CarBuilder SetTorque(AnimationCurve torque)
    {
        _car.Torque = torque; 
        return this;
    }
    public CarBuilder SetBrakeTorque(float brakeTorque)
    {
        _car.BrakeTorque = brakeTorque;
        return this;
    }
    public CarBuilder SetHandBrakeTorque(float handbrakeTorque)
    {
        _car.HandBrakeTorque = handbrakeTorque;
        return this;
    }
    public CarBuilder SetSteeringMax(float steeringmax)
    {
        _car.SteeringMax = steeringmax;
        return this;
    }
    public CarBuilder SetMaxSpeed(float maxspeed)
    {
        _car.MaxSpeed = maxspeed;
        return this;
    }
    public CarBuilder SetSteeringCurve(AnimationCurve steeringcurve)
    {
        _car.SteeringCurve = steeringcurve;
        return this;
    }
    public CarBuilder SetTractionCutoff(float tractioncutoff)
    {
        _car.TractionCutoff = tractioncutoff;
        return this;
    }
    public CarBuilder SetFinalDriveAxle(float finaldriveaxle)
    {
        _car.FinalDriveAxle = finaldriveaxle;
        return this;
    }
    public CarBuilder SetDrivingWheels(int drivingwheels)
    {
        _car.DrivingWheels = drivingwheels;
        return this;
    }
    public CarBuilder SetMass(float mass)
    {
        _car.Mass= mass;
        return this;
    }
    public CarBuilder SetWheelRadius(float wheelradius)
    {
        _car.WheelRadius = wheelradius;
        return this;
    }
    public CarBuilder SetWheelMass(float wheelmass)
    {
        _car.WheelMass = wheelmass;
        return this;
    }

    public Car Build()
    {
        return _car;
    }
}

/*

example usage:
// Storing template
Car sportsCarTemplate = new CarBuilder().SetMaxTorque(656).SetBrakeTorque(5000f).Build();
carConfigs["SportsCar"] = sportsCarTemplate;

// Cloning the car
Car newSportsCar = carConfigs["SportsCar"].Clone();

*/
//private static readonly Dictionary<string, Car> carPresets = new Dictionary<string, Car>
//    {
//        { "SportsCar", new PlayerCar { MaxTorque = 656, BrakeTorque = 5000f, HandBrakeTorque = 3000f, SteeringMax = 27, MaxSpeed = 320f, TractionCutoff = 0.35f, FinalDriveAxle = 3.7f, DrivingWheels = 2, Mass = 1800, WheelRadius = 0.34f, WheelMass = 25f }},
//      //{ "SUV", new BotCar { MaxTorque = 400, BrakeTorque = 3500f, HandBrakeTorque = 2000f, SteeringMax = 30, MaxSpeed = 200f, TractionCutoff = 0.4f, FinalDriveAxle = 4.1f, DrivingWheels = 4, Mass = 2200, WheelRadius = 0.38f, WheelMass = 30f }}
//    };
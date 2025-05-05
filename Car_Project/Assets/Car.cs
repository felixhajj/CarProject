using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public interface ICar
{
    //everything data related, that doesnt use directly unity's physics. doesnt have 
    public ICar SetName(string name);
    public ICar SetMaxTorque(float maxTorque);
    public ICar SetTorque(AnimationCurve torque);
    public ICar SetBrakeTorque(float brakeTorque);
    public ICar SetHandBrakeTorque(float handbrakeTorque);
    public ICar SetSteeringMax(float steeringmax);
    public ICar SetMaxSpeed(float maxspeed);
    public ICar SetSteeringCurve(AnimationCurve steeringcurve);
    public ICar SetTractionCutoff(float tractioncutoff);
    public ICar SetFinalDriveAxle(float finaldriveaxle);
    public ICar SetDrivingWheels(float drivingwheels);
    public ICar SetIsTractionControl(bool istractioncontrol);
    public ICar SetMass(float mass);
    public ICar SetWheelRadius(float wheelradius);
    public ICar SetWheelMass(float wheelmass);

    public ICar Clone(string carRole);

}

public class Car : ICar
{
    public string Name { get; set; }
    public float MaxTorque { get; set; } // Nm max torque out of crankshaft (engine torque)
    public AnimationCurve Torque { get; set; } // Torque curve coming out of the crankshaft
    public float BrakeTorque { get; set; }
    public float HandBrakeTorque { get; set; }
    public float SteeringMax { get; set; }
    public float MaxSpeed { get; set; }
    public AnimationCurve SteeringCurve { get; set; }
    public float TractionCutoff { get; set; }
    public float FinalDriveAxle { get; set; }
    public float DrivingWheels { get; set; } // Number of driving wheels (e.g., 2 for RWD)
    public bool IsTractionControl { get; set; }


    // Properties to be set dynamically based on the game environment
    public float Mass { get; set; } // Mass of the car in kg
    public float WheelRadius { get; set; } // Radius of the wheel in meters
    public float WheelMass { get; set; } // Mass of the wheel in kg

    public ICar SetName(string name)
    {
        Name = name;
        return this;
    }
    public ICar SetMaxTorque(float maxTorque)
    {
        MaxTorque = maxTorque;
        return this;
    }
    public ICar SetTorque(AnimationCurve torque)
    {
        Torque = torque;
        return this;
    }
    public ICar SetBrakeTorque(float brakeTorque)
    {
        BrakeTorque = brakeTorque;
        return this;
    }
    public ICar SetHandBrakeTorque(float handbrakeTorque)
    {
        HandBrakeTorque = handbrakeTorque;
        return this;
    }
    public ICar SetSteeringMax(float steeringmax)
    {
        SteeringMax = steeringmax;
        return this;
    }
    public ICar SetMaxSpeed(float maxspeed)
    {
        MaxSpeed = maxspeed;
        return this;
    }
    public ICar SetSteeringCurve(AnimationCurve steeringcurve)
    {
        SteeringCurve = steeringcurve;
        return this;
    }
    public ICar SetTractionCutoff(float tractioncutoff)
    {
        TractionCutoff = tractioncutoff;
        return this;
    }
    public ICar SetFinalDriveAxle(float finaldriveaxle)
    {
        FinalDriveAxle = finaldriveaxle;
        return this;
    }
    public ICar SetDrivingWheels(float drivingwheels)
    {
        DrivingWheels = drivingwheels;
        return this;
    }
    public ICar SetIsTractionControl(bool istractioncontrol)
    {
        IsTractionControl = istractioncontrol; 
        return this;
    }
    public ICar SetMass(float mass)
    {
        Mass = mass;
        return this;
    }
    public ICar SetWheelRadius(float wheelradius)
    {
        WheelRadius = wheelradius;
        return this;
    }
    public ICar SetWheelMass(float wheelmass)
    {
        WheelMass = wheelmass;
        return this;
    }


    public ICar Clone(string carRole)
    {
        switch (carRole)
        {
            case "PlayerCar":
                return new PlayerCar
                {
                    Name = this.Name,
                    MaxTorque = this.MaxTorque,
                    Torque = new AnimationCurve(this.Torque.keys), // Deep copy of AnimationCurve
                    BrakeTorque = this.BrakeTorque,
                    HandBrakeTorque = this.HandBrakeTorque,
                    SteeringMax = this.SteeringMax,
                    MaxSpeed = this.MaxSpeed,
                    SteeringCurve = new AnimationCurve(this.SteeringCurve.keys), // Deep copy of AnimationCurve
                    TractionCutoff = this.TractionCutoff,
                    FinalDriveAxle = this.FinalDriveAxle,
                    DrivingWheels = this.DrivingWheels,
                    Mass = this.Mass,
                    WheelRadius = this.WheelRadius,
                    WheelMass = this.WheelMass,
                };

            case "BotCar":
                return new BotCar
                {
                    Name = this.Name,
                    MaxTorque = this.MaxTorque,
                    Torque = new AnimationCurve(this.Torque.keys), // Deep copy of AnimationCurve
                    BrakeTorque = this.BrakeTorque,
                    HandBrakeTorque = this.HandBrakeTorque,
                    SteeringMax = this.SteeringMax,
                    MaxSpeed = this.MaxSpeed,
                    SteeringCurve = new AnimationCurve(this.SteeringCurve.keys), // Deep copy of AnimationCurve
                    TractionCutoff = this.TractionCutoff,
                    FinalDriveAxle = this.FinalDriveAxle,
                    DrivingWheels = this.DrivingWheels,
                    Mass = this.Mass,
                    WheelRadius = this.WheelRadius,
                    WheelMass = this.WheelMass,
                };

            default:
                throw new ArgumentException("Invalid car type provided: " + carRole);
        }
    }
}

public class PlayerCar : Car
{
    //to fill
}

public class BotCar : Car
{
    // to fill
}

public class Inventory
{
    ICar car;
    public Inventory(ICar carr)
    {
        car = carr;
    }

    public ICar createcar(string carRole)
    {
        return car.Clone(carRole);
    }
}


/*
ICar sportcar = new Car().SetName().SetBrakeTorque();
//inventory made to make cars in general, without specifying them to a certain task. like if they were in the menu, before playing the game

Inventory sportcarinv = new Inventory(sportcar);

//i create clone of cars, which can be either a PlayerCar or BotCar
var sportcar1 = sportcarinv.createcar("PlayerCar");
var sportcar2 = sportcarinv.createcar("BotCar");
*/
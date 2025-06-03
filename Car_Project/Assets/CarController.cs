using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class CarController : MonoBehaviour, IcarInitializer
{
    //everything related to the car and its logic, and not mechanic related,(the wheelgroundstate, centerofmass,etc.. are not considered mechanic).
    private Car car;
    private CarObjects carobjects;

    public bool engineturnon = false;

    //tracking values(which i definitely need also)
    public Dictionary<WheelCollider, int> wheelGroundStates;//if one of those is equal to 0, means the tyre is in the air, if its other, 1 would be for road, 2 for offroad,etc... should work more on it
    public float frontLeftLoad;
    public float frontRightLoad;
    public float rearLeftLoad;
    public float rearRightLoad;
    float wheelCircumference;


    public void Initialize(Car ccar)
    {
        this.car = ccar;
    }

    void Start()
    {
        carobjects= GetComponent<CarObjects>();


        GetComponent<Rigidbody>().centerOfMass = carobjects.centerofmass.transform.localPosition;


        wheelCircumference = 2f * Mathf.PI * car.WheelRadius;

        wheelGroundStates = new Dictionary<WheelCollider, int>
        {
            { carobjects.wheelColliders["FrontLeft"], 0 },
            { carobjects.wheelColliders["FrontRight"], 0 },
            { carobjects.wheelColliders["RearLeft"], 0 },
            { carobjects.wheelColliders["RearRight"], 0 }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //rpm = 1000;
            engineturnon = !engineturnon;
            if (engineturnon)
            {
                // Play engine startup audio
                carobjects.engineAudioSource.clip = carobjects.engineStartupClip;
                carobjects.engineAudioSource.loop = false;
                carobjects.engineAudioSource.Play();
            }
            else
            {
                carobjects.engineAudioSource.Stop();

                // Play engine turnoff audio
                carobjects.engineAudioSource.clip = carobjects.engineShutdownClip;
                carobjects.engineAudioSource.loop = false;
                carobjects.engineAudioSource.Play();

            }
        }

        if (engineturnon && !carobjects.engineAudioSource.isPlaying)
        {

            // Play engine loop
            carobjects.engineAudioSource.clip = carobjects.engineLoopClip;
            carobjects.engineAudioSource.loop = true;
            carobjects.engineAudioSource.Play();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 newRotation = transform.eulerAngles;

            newRotation.z = 0;
            newRotation.x = 0;
            transform.eulerAngles = newRotation;



            Vector3 newPosition = transform.position;
            newPosition.y += 3;
            transform.position = newPosition;


        }
    }
    private void FixedUpdate()
    {
        UpdateWheelGroundState(carobjects.wheelColliders["FrontLeft"]);
        UpdateWheelGroundState(carobjects.wheelColliders["FrontRight"]);
        UpdateWheelGroundState(carobjects.wheelColliders["RearLeft"]);
        UpdateWheelGroundState(carobjects.wheelColliders["RearRight"]);


        frontLeftLoad = GetWheelLoad(carobjects.wheelColliders["FrontLeft"]);
        frontRightLoad = GetWheelLoad(carobjects.wheelColliders["FrontRight"]);
        rearLeftLoad = GetWheelLoad(carobjects.wheelColliders["RearLeft"]);
        rearRightLoad = GetWheelLoad(carobjects.wheelColliders["RearRight"]);

        animatewheels();

    }
    void animatewheels()
    {
        Vector3 wheelposition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        foreach (var pair in carobjects.wheelColliders) // Iterates through all wheel colliders
        {
            pair.Value.GetWorldPose(out wheelposition, out wheelRotation); // Get the position & rotation

            if (carobjects.wheelsToAnimate.ContainsKey(pair.Key + "Wheel")) // Match the correct visual wheel
            {
                carobjects.wheelsToAnimate[pair.Key + "Wheel"].transform.position = wheelposition;
                carobjects.wheelsToAnimate[pair.Key + "Wheel"].transform.rotation = wheelRotation;
            }
        }
    }
    


    private void UpdateWheelGroundState(WheelCollider wheel)
    {
        if (IsWheelOnGround(wheel))
        {
            wheelGroundStates[wheel] = 1; // Wheel is on the ground
        }
        else
        {
            wheelGroundStates[wheel] = 0; // Wheel is off the ground
        }
    }
    private bool IsWheelOnGround(WheelCollider wheel)
    {
        WheelHit hit;
        return wheel.GetGroundHit(out hit);
    }
    private float GetWheelLoad(WheelCollider wheel)
    {
        WheelHit hit;
        if (wheel.GetGroundHit(out hit))
        {
            return hit.force;
        }

        return 0f;
    }

}

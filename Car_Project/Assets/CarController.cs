using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class CarController : MonoBehaviour, IcarInitializer
{
    //everything related to the car and its logic, and not mechanic related,(the wheelgroundstate, centerofmass,etc.. are not considered mechanic).
    //or accessing car parts from different script, you put them here and other scripts use them from here(like wheelcolliders and centerofmass) ?????doesnt make sense as other scripts are accessing car parts like fronttyremarks in vehiculetraukeffect and lights in Lighting Manager
    private Car car;

    public bool engineturnon = false;

    //if one of those is equal to 0, means the tyre is in the air, if its other, 1 would be for road, 2 for offroad,etc... should work more on it
    public Dictionary<WheelCollider, int> wheelGroundStates;

    public Dictionary<string, WheelCollider> wheelColliders = new Dictionary<string, WheelCollider>();


    public Dictionary<string, GameObject> wheelsToAnimate = new Dictionary<string, GameObject>();

    public float frontLeftLoad;
    public float frontRightLoad;
    public float rearLeftLoad;
    public float rearRightLoad;

    public GameObject centerofmass;


    //public TrailRenderer[] fronttyremarks;
    //public TrailRenderer[] reartyremarks;


    public AudioSource engineAudioSource;
    public AudioSource tirescreechAudioSource;
    public AudioClip engineStartupClip;
    public AudioClip engineLoopClip;
    public AudioClip engineShutdownClip;
    public AudioClip tirescreechingClip;
    public AudioClip tirescreechingendClip;

    float wheelCircumference;


    public void Initialize(Car ccar)
    {
        this.car = ccar;
    }

    void Start()
    {
        //Vector3 cardim = cardimensions.bounds.size;

        foreach (WheelCollider wheelcollider in GetComponentsInChildren<WheelCollider>())
        {
            if (wheelcollider == null)
            {
                Debug.LogError($"[CarController] Missing WheelCollider on {gameObject.name}");
                continue;
            }

            wheelColliders[wheelcollider.name] = wheelcollider;
        }

        foreach (Transform wheeltoanimate in GetComponentsInChildren<Transform>())
        {
            if (wheeltoanimate.name.EndsWith("Wheel"))
            {
                if (wheeltoanimate.gameObject == null)
                {
                    Debug.LogError($"[CarController] Missing visual wheel object for {wheeltoanimate.name} in {gameObject.name}");
                    continue;
                }

                wheelsToAnimate[wheeltoanimate.name] = wheeltoanimate.gameObject;
            }
        }


        centerofmass = transform.Find("CenterOfMass")?.gameObject;

        if (centerofmass == null)
        {
            Debug.LogError("Center of Mass not found in " + gameObject.name);
        }

        GetComponent<Rigidbody>().centerOfMass = centerofmass.transform.localPosition;


        wheelCircumference = 2f * Mathf.PI * car.WheelRadius;

        wheelGroundStates = new Dictionary<WheelCollider, int>
        {
            { wheelColliders["FrontLeft"], 0 },
            { wheelColliders["FrontRight"], 0 },
            { wheelColliders["RearLeft"], 0 },
            { wheelColliders["RearRight"], 0 }
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
                engineAudioSource.clip = engineStartupClip;
                engineAudioSource.loop = false;
                engineAudioSource.Play();
            }
            else
            {
                engineAudioSource.Stop();

                // Play engine turnoff audio
                engineAudioSource.clip = engineShutdownClip;
                engineAudioSource.loop = false;
                engineAudioSource.Play();

            }
        }

        if (engineturnon && !engineAudioSource.isPlaying)
        {

            // Play engine loop
            engineAudioSource.clip = engineLoopClip;
            engineAudioSource.loop = true;
            engineAudioSource.Play();

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
        UpdateWheelGroundState(wheelColliders["FrontLeft"]);
        UpdateWheelGroundState(wheelColliders["FrontRight"]);
        UpdateWheelGroundState(wheelColliders["RearLeft"]);
        UpdateWheelGroundState(wheelColliders["RearRight"]);


        frontLeftLoad = GetWheelLoad(wheelColliders["FrontLeft"]);
        frontRightLoad = GetWheelLoad(wheelColliders["FrontRight"]);
        rearLeftLoad = GetWheelLoad(wheelColliders["RearLeft"]);
        rearRightLoad = GetWheelLoad(wheelColliders["RearRight"]);

        animatewheels();

    }
    void animatewheels()
    {
        Vector3 wheelposition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        foreach (var pair in wheelColliders) // Iterates through all wheel colliders
        {
            pair.Value.GetWorldPose(out wheelposition, out wheelRotation); // Get the position & rotation

            if (wheelsToAnimate.ContainsKey(pair.Key + "Wheel")) // Match the correct visual wheel
            {
                wheelsToAnimate[pair.Key + "Wheel"].transform.position = wheelposition;
                wheelsToAnimate[pair.Key + "Wheel"].transform.rotation = wheelRotation;
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

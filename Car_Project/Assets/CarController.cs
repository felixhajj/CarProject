using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    //everything related to the car and its logic, and not mechanic related,(the wheelgroundstate, centerofmass,etc.. are not considered mechanic).
    //also a middleman(using mediator pattern maybe?) between the Car class and other classes that needs it.
    public Car CarData { get; private set; }
    public PowerToWheels powertowheels { get; private set; }

    public bool engineturnon = false;

    //if one of those is equal to 0, means the tyre is in the air, if its other, 1 would be for road, 2 for offroad,etc... should work more on it
    public Dictionary<WheelCollider, int> wheelGroundStates;

    public float frontLeftLoad;
    public float frontRightLoad;
    public float rearLeftLoad;
    public float rearRightLoad;


    public WheelCollider rearright;
    public WheelCollider rearleft;

    public WheelCollider frontleft;
    public WheelCollider frontright;

    public GameObject[] wheels = new GameObject[4];

    public GameObject centerofmass;
    private Rigidbody rigidbody;

    public Rigidbody car;


    public TrailRenderer[] fronttyremarks;
    public TrailRenderer[] reartyremarks;

    public MeshRenderer cardimensions;
    public MeshRenderer wheeldimensions;
    public MeshRenderer housedimensions;

    public AudioSource engineAudioSource;
    public AudioSource tirescreechAudioSource;
    public AudioClip engineStartupClip;
    public AudioClip engineLoopClip;
    public AudioClip engineShutdownClip;
    public AudioClip tirescreechingClip;
    public AudioClip tirescreechingendClip;

    float wheelCircumference;

    void Start()
    {
        ICar sportcar = new Car().SetName("SRT Challenger").SetBrakeTorque(5000);//....continue all
        
        //inventory made to make cars in general, without specifying them to a certain task. like if they were in the menu, before playing the game
        Inventory sportcarinv = new Inventory(sportcar);

        //i create clone of already available cars by using the car's inventory, and assign to be either a PlayerCar or BotCar
        var sportcar1 = sportcarinv.createcar("PlayerCar");
        var sportcar2 = sportcarinv.createcar("BotCar");


        Vector3 cardim = cardimensions.bounds.size;
        Debug.Log("car dimensions (meters): " + cardim);

        Vector3 wheeldim = wheeldimensions.bounds.size;
        Debug.Log("wheel dimensions (meters): " + wheeldim);

        Vector3 housedim = housedimensions.bounds.size;
        Debug.Log("house dimensions (meters): " + housedim);
        rigidbody = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = centerofmass.transform.localPosition;

        wheelCircumference = 2f * Mathf.PI * CarData.WheelRadius;

        wheelGroundStates = new Dictionary<WheelCollider, int>
        {
            { frontleft, 0 },
            { frontright, 0 },
            { rearleft, 0 },
            { rearright, 0 }
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
        UpdateWheelGroundState(frontleft);
        UpdateWheelGroundState(frontright);
        UpdateWheelGroundState(rearleft);
        UpdateWheelGroundState(rearright);


        frontLeftLoad = GetWheelLoad(frontleft);
        frontRightLoad = GetWheelLoad(frontright);
        rearLeftLoad = GetWheelLoad(rearleft);
        rearRightLoad = GetWheelLoad(rearright);

        animatewheels();

    }
    void animatewheels()
    {
        Vector3 wheelposition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                rearleft.GetWorldPose(out wheelposition, out wheelRotation);

            }
            else if (i == 1)
            {
                rearright.GetWorldPose(out wheelposition, out wheelRotation);
            }
            else if (i == 2)
            {
                frontleft.GetWorldPose(out wheelposition, out wheelRotation);
            }
            else if (i == 3)
            {
                frontright.GetWorldPose(out wheelposition, out wheelRotation);
            }
            wheels[i].transform.position = wheelposition;
            wheels[i].transform.rotation = wheelRotation;
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

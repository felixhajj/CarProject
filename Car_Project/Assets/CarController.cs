using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Car CarData { get; private set; }

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
        
    }
}

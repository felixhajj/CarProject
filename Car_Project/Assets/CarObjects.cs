using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CarObjects : MonoBehaviour
{
    //all objects that should be dragged from the car or somewhere else and used in the classes that are inside the car

    //carcontroller
    
    public Dictionary<string, WheelCollider> wheelColliders = new Dictionary<string, WheelCollider>();
    public Dictionary<string, GameObject> wheelsToAnimate = new Dictionary<string, GameObject>();

    public GameObject centerofmass;

    //customizable(from Resources folder)
    public AudioSource engineAudioSource;
    public AudioSource tirescreechAudioSource;
    public AudioClip engineStartupClip;
    public AudioClip engineLoopClip;
    public AudioClip engineShutdownClip;
    public AudioClip tirescreechingClip;
    public AudioClip tirescreechingendClip;



    //in vehiculetraileffect

    //customizable(from Resources folder)
    public TrailRenderer[] fronttyremarks = new TrailRenderer[2];
    public TrailRenderer[] backtyremarks = new TrailRenderer[2];

    public ParticleSystem[] frontwheelsmoke = new ParticleSystem[2];
    public ParticleSystem[] rearwheelsmoke = new ParticleSystem[2];//customizable from the Material inside Renderer(inside the Particlesystem object)



    //cinemachinecameras
    public CinemachineVirtualCamera thirdpersonrear;
    public CinemachineVirtualCamera thirdpersonfront;
    public CinemachineVirtualCamera worldupcamera;
    public CinemachineVirtualCamera firstperson;

    public CinemachineBrain Brain;
    public GameObject worldup;



    //lightingmanager
    public GameObject frontleftlow;
    public GameObject frontrightlow;
    public GameObject highbeamleft;
    public GameObject highbeamright;


    //customizable(from Resources folder)
    public GameObject trailleft;
    public GameObject trailright;

    public TrailRenderer trailLeftRenderer;
    public TrailRenderer trailRightRenderer;


    void Start()
    {
        //carcontroller

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

        engineStartupClip = Resources.Load<AudioClip>("1-Audio/1-engine/2.acceleration");
        engineLoopClip = Resources.Load<AudioClip>("1-Audio/1-engine/3.looped engine");
        engineShutdownClip = Resources.Load<AudioClip>("1-Audio/1-engine/4.engine off");

        tirescreechingClip = Resources.Load<AudioClip>("1-Audio/1-Tires/1.tire_screeching");
        tirescreechingendClip = Resources.Load<AudioClip>("1-Audio/1-Tires/2.tire_screeching_end");


        //vehiculetraileffect
        fronttyremarks[0] = transform.Find("skidmarkfrontleft")?.GetComponent<TrailRenderer>();
        fronttyremarks[1] = transform.Find("skidmarkfrontright")?.GetComponent<TrailRenderer>();

        backtyremarks[0] = transform.Find("skidmarkrearleft")?.GetComponent<TrailRenderer>();
        backtyremarks[1] = transform.Find("skidmarkrearright")?.GetComponent<TrailRenderer>();


        frontwheelsmoke[0] = transform.Find("frontleftsmoke")?.GetComponentInChildren<ParticleSystem>();
        frontwheelsmoke[1] = transform.Find("frontrightsmoke")?.GetComponentInChildren<ParticleSystem>();
        rearwheelsmoke[0] = transform.Find("rearleftsmoke")?.GetComponentInChildren<ParticleSystem>();
        rearwheelsmoke[1] = transform.Find("rearrightsmoke")?.GetComponentInChildren<ParticleSystem>();

        // Optional: stop them safely
        frontwheelsmoke[0]?.Stop();
        frontwheelsmoke[1]?.Stop();
        rearwheelsmoke[0]?.Stop();
        rearwheelsmoke[1]?.Stop();


        //cinemachinecameras
        thirdpersonrear = transform.root.Find("3rd Person rear")?.GetComponent<CinemachineVirtualCamera>();
        thirdpersonfront = transform.root.Find("3rd Person front")?.GetComponent<CinemachineVirtualCamera>();
        worldupcamera = transform.root.Find("worldupcamera")?.GetComponent<CinemachineVirtualCamera>();
        firstperson = transform.root.Find("1st person")?.GetComponent<CinemachineVirtualCamera>();

        Brain = transform.root.Find("1st person")?.GetComponent<CinemachineBrain>();

        worldup = transform.root.Find("Worldup")?.gameObject;

        //lightingmanager
        frontleftlow = transform.Find("leftlowlight")?.gameObject;
        frontrightlow = transform.Find("rightlowlight")?.gameObject;
        highbeamleft = transform.Find("lefthighlight")?.gameObject;
        highbeamright = transform.Find("righthighlight")?.gameObject;

        trailleft = transform.Find("Trailleft")?.gameObject;
        trailright = transform.Find("Trailright")?.gameObject;

        trailLeftRenderer= transform.Find("Trailleft")?.GetComponent<TrailRenderer>();
        trailRightRenderer= transform.Find("Trailright")?.GetComponent<TrailRenderer>();
    }

    void Update()
    {
        
    }
}

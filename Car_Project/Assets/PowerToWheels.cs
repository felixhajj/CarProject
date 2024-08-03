using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//note!!!!!!!!!!! 1kmh is equal to 0.75 in the velocity of unity
public class PowerToWheels : MonoBehaviour
{
    public float Torque = 500;
	public float handBrakeTorque = 2400f;
    public float BrakeTorque = 2400f;
	private float slipangle;
	public float gasinput;
	public float brakeinput;
	public float rearrot;
	public float frontrot;

	float radius;
	float wheelCircumference;



    public float steeringMax = 25;
    public WheelCollider backright;
    public WheelCollider backleft;

    public WheelCollider frontleft;
    public WheelCollider frontright;
	
    public GameObject[] wheels = new GameObject[4];

	public GameObject centerofmass;
	private Rigidbody rigidbody;


    public ParticleSystem frontLeftSmokePrefab;
    public ParticleSystem frontRightSmokePrefab;
    public ParticleSystem rearLeftSmokePrefab;
    public ParticleSystem rearRightSmokePrefab;


    private ParticleSystem frontleftsmoke;
    private ParticleSystem frontrightsmoke;
    private ParticleSystem rearleftsmoke;
    private ParticleSystem rearrightsmoke;

	private bool rearleftsmokePlaying = false;
	private bool rearrightsmokePlaying = false;

	private bool frontleftsmokePlaying = false;
	private bool frontrightsmokePlaying = false;

	public TrailRenderer[] fronttyremarks;
	public TrailRenderer[] backtyremarks;

	bool engineturnon = false;

	public AudioSource engineAudioSource;
	public AudioClip engineStartupClip;
	public AudioClip engineLoopClip;
	public AudioClip engineShutdownClip;
	void Start()
    {
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.centerOfMass = centerofmass.transform.localPosition;

		radius = backright.radius;
        wheelCircumference = 2f * Mathf.PI * radius;
    }

	void Update()
	{
		rearrot = backleft.rpm;
		frontrot = frontleft.rpm;


		if (Input.GetKeyDown(KeyCode.P))
		{
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
		if (engineturnon)
		{
			if (Mathf.Abs(rearrot) < 2000f)
			{
				engineAudioSource.volume = 0.25f + (Mathf.Abs(rearrot) * (0.75f/2000)) ;
				engineAudioSource.pitch = 1 + (Mathf.Abs(rearrot) / 2000f);
			}
			else
			{
				engineAudioSource.volume = 1;
				if(engineAudioSource.pitch >= 2)
				{
					engineAudioSource.pitch -= 0.3f;
				}
				else
				{
					engineAudioSource.pitch += 0.01f;
				}
			}
		}


		if (frontlock() || frontwheelslide())
		{
			foreach(TrailRenderer T in fronttyremarks)
			{
				T.emitting = true;
			}
			if (frontleftsmoke == null)
			{
				frontleftsmoke = Instantiate(frontLeftSmokePrefab, frontleft.transform.position, Quaternion.identity);
			}
			else
			{
				frontleftsmoke.transform.position = frontleft.transform.position;
				if (!frontleftsmokePlaying)
				{
					frontleftsmoke.Play();
					frontleftsmokePlaying = true;
					Debug.Log("Playing front left");
				}
			}

			if (frontrightsmoke == null)
			{
				frontrightsmoke = Instantiate(frontRightSmokePrefab, frontright.transform.position, Quaternion.identity);
			}
			else
			{
				frontrightsmoke.transform.position = frontright.transform.position;
				if (!frontrightsmokePlaying)
				{
					frontrightsmoke.Play();
					frontrightsmokePlaying = true;
					Debug.Log("Playing front right");
				}
			}
		}
		else
		{
			foreach (TrailRenderer T in fronttyremarks)
			{
				T.emitting = false;
			}
			if (frontleftsmokePlaying)
			{
				frontleftsmoke.Stop();
				frontleftsmokePlaying = false;
			}

			if (frontrightsmokePlaying)
			{
				frontrightsmoke.Stop();
				frontrightsmokePlaying = false;
			}
		}


		if (CarSliding() || rearlock() || burnout())
		{
			foreach (TrailRenderer T in backtyremarks)
			{
				T.emitting = true;
			}
			if (rearleftsmoke == null)
			{
				rearleftsmoke = Instantiate(rearLeftSmokePrefab, backleft.transform.position, Quaternion.identity);
			}
			else
			{
				rearleftsmoke.transform.position = backleft.transform.position;
				if (!rearleftsmokePlaying)
				{
					rearleftsmoke.Play();
					rearleftsmokePlaying = true;
				}
			}

			if (rearrightsmoke == null)
			{
				rearrightsmoke = Instantiate(rearRightSmokePrefab, backright.transform.position, Quaternion.identity);
			}
			else
			{
				rearrightsmoke.transform.position = backright.transform.position;
				if (!rearrightsmokePlaying)
				{
					rearrightsmoke.Play();
					rearrightsmokePlaying = true;
				}
			}
		}
		else
		{
			foreach (TrailRenderer T in backtyremarks)
			{
				T.emitting = false;
			}
			if (rearleftsmokePlaying)
			{
				rearleftsmoke.Stop();
				rearleftsmokePlaying = false;
			}

			if (rearrightsmokePlaying)
			{
				rearrightsmoke.Stop();
				rearrightsmokePlaying = false;
			}
		}
	}


    private void FixedUpdate()
    {
        animatewheels();
		
		braking();

		
		steering();

		if (engineturnon == true)
		{
			gas();
		}

		
        Debug.DrawRay(transform.position, transform.position.normalized * 10);
	 	Debug.DrawRay(transform.position, transform.position.normalized * 10, Color.blue);
	 
	}
    public bool CarSliding()
    {
        Vector3 forwardVector = transform.forward;

        Vector3 velocityVector = rigidbody.velocity;

        forwardVector.Normalize();
        velocityVector.Normalize();

        float dotProduct = Vector3.Dot(forwardVector, velocityVector);

        float driftThreshold = 0.7f; 

        return Mathf.Abs(dotProduct) < driftThreshold;
    }

    public bool frontwheelslide()
	{
        Vector3 forwardVector = transform.forward;

        Vector3 velocityVector = rigidbody.velocity;

        forwardVector.Normalize();
        velocityVector.Normalize();

        float dotProduct = Vector3.Dot(forwardVector, velocityVector);

        float driftThreshold = 0.3f;

        return Mathf.Abs(dotProduct) < driftThreshold;
    }
	public bool rearlock()
	{
		if(rearrot==0 && Math.Abs(rigidbody.velocity.magnitude) > 5)
		{
            return true;
		}
		else
		{
			return false;
		}
	}

	public bool frontlock()
	{
        if (frontrot == 0 && Math.Abs(rigidbody.velocity.magnitude) > 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	public bool burnout()
	{
		if(Math.Abs(rearrot) > Math.Abs((frontrot  * 1.7f )))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    void gas()
	{
		float motor = gasinput * Torque * 5;
		backleft.motorTorque = motor;
		backright.motorTorque = motor;
	}
	void braking()
	{
		gasinput = Input.GetAxis("Vertical");
		slipangle = Vector3.Angle(transform.forward, rigidbody.velocity - transform.forward);
		if (slipangle < 120f)
		{
			if (gasinput < 0)
			{
				brakeinput = Math.Abs(gasinput);
				gasinput = 0;
			}
			else
			{
				brakeinput = 0;
			}
		}
		else
		{
			brakeinput = 0;
		}

		if (Input.GetKey(KeyCode.Space))//handbrake
		{
			Debug.Log("handbrake");
			backright.brakeTorque = handBrakeTorque;
			backleft.brakeTorque = handBrakeTorque;
		}
		else
		{
            frontright.brakeTorque = brakeinput * BrakeTorque * 0.7f * 5;
            frontleft.brakeTorque = brakeinput * BrakeTorque * 0.7f * 5;
            backleft.brakeTorque = brakeinput * BrakeTorque * 0.3f * 5;
            backright.brakeTorque = brakeinput * BrakeTorque * 0.3f * 5;
        }


		

	}
	void steering()
	{
		if (Input.GetAxis("Horizontal") != 0)
		{
			frontleft.steerAngle = Input.GetAxis("Horizontal") * steeringMax;
			frontright.steerAngle = Input.GetAxis("Horizontal") * steeringMax;
		}
		else
		{
			frontleft.steerAngle = 0;
			frontright.steerAngle = 0;
		}
	}
    void animatewheels()
    {
        Vector3 wheelposition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
				backleft.GetWorldPose(out wheelposition, out wheelRotation);

			}
            else if(i==1) 
            {
				backright.GetWorldPose(out wheelposition, out wheelRotation);
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

}

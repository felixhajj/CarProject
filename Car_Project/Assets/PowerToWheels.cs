using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//note!!!!!!!!!!! 1kmh is equal to 0.75 in the velocity of unity
public class PowerToWheels : MonoBehaviour
{
    public float Torque = 1000;
	public float handBrakeTorque = 5000f;
    public float BrakeTorque = 1500f;
	private float slipangle;
	public float gasinput;
	public float brakeinput;
	public float rearrot;
	public float frontrot;

	float radius;
	float wheelCircumference;

	//if one of those is equal to 0, means the tyre is in the air, if its other, 1 would be for road, 2 for offroad,etc... should work more on it
	public Dictionary<WheelCollider, int> wheelGroundStates;

	public float steeringMax = 25;
    public WheelCollider rearright;
    public WheelCollider rearleft;

    public WheelCollider frontleft;
    public WheelCollider frontright;
	
    public GameObject[] wheels = new GameObject[4];

	public GameObject centerofmass;
	private Rigidbody rigidbody;

	public Rigidbody car;



	public TrailRenderer[] fronttyremarks;/// <summary>
	/// </summary>
	public TrailRenderer[] reartyremarks;

	bool engineturnon = false;

	public float decelerationFactor = 1000f;
	public float maxBrakeForce = 300f;
	public float minRPM = 100f;

	public AudioSource engineAudioSource;
	public AudioSource tirescreechAudioSource;
	public AudioClip engineStartupClip;
	public AudioClip engineLoopClip;
	public AudioClip engineShutdownClip;
	public AudioClip tirescreechingClip;
	public AudioClip tirescreechingendClip;

	void Start()
    {
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.centerOfMass = centerofmass.transform.localPosition;

		radius = rearright.radius;
        wheelCircumference = 2f * Mathf.PI * radius;

		wheelGroundStates = new Dictionary<WheelCollider, int>
		{
			{ frontleft, 0 },
			{ frontright, 0 },
			{ rearleft, 0 },
			{ rearright, 0 }
		};
	}

	void Update()
	{
			
		UpdateWheelGroundState(frontleft);
		UpdateWheelGroundState(frontright);
		UpdateWheelGroundState(rearleft);
		UpdateWheelGroundState(rearright);

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
		rearrot = rearleft.rpm;
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

		/*if (engineturnon)
		{
			if (Mathf.Abs(rearrot) < 2000f)
			{
				engineAudioSource.volume = 0.25f + (Mathf.Abs(rearrot) * (0.75f / 2000));
				engineAudioSource.pitch = 1 + (Mathf.Abs(rearrot) / 2000f);
			}
			else
			{
				engineAudioSource.volume = 1;
				if (engineAudioSource.pitch >= 2)
				{
					engineAudioSource.pitch -= 0.3f;
				}

				else
				{
					engineAudioSource.pitch += 0.01f;
				}
			}
		}
		*/

		
	}

    private void FixedUpdate()
	{
		gasinput = Input.GetAxis("Vertical");

		//decelerator();

		animatewheels();
		braking();
		steering();
		gas();

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

        float driftThreshold = 0.97f;

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

	//void decelerator()
	//{
		// Apply brake torque to decelerate rear wheels when no gas input
	//	if (Mathf.Abs(rearleft.rpm) > minRPM) // Check if wheels are still rotating
	//	{
	//		rearleft.brakeTorque = decelerationFactor * 0.7f * 5;
	//		rearright.brakeTorque = decelerationFactor * 0.7f * 5;
	//		Debug.Log("rearleft brakeTorque= " + rearleft.brakeTorque);
	//	}
		//else
		//{
		//	rearleft.brakeTorque = 0;  // Stop applying brakes when the car is stopped
		//	rearright.brakeTorque = 0;
		//}
	//}
	void gas()
	{
		if (engineturnon)
		{
			if(gasinput<0)
			{
				gasinput /= 1.5f;
			}
			float motor = gasinput * Torque * 5;
			rearleft.motorTorque = motor;
			rearright.motorTorque = motor;

			enginesound(motor);
		}
		else
		{
			rearleft.motorTorque = 0;
			rearright.motorTorque = 0;
		}
	}
	void braking()
	{
		// Calculate slip angle first
		slipangle = Vector3.Angle(transform.forward, rigidbody.velocity - transform.forward);


		// Determine brake input based on forward or reverse movement
		if (slipangle < 120f && rearrot > minRPM) // Moving forward, apply brakes if needed
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
		else // Moving backward, no braking input applied
		{
			brakeinput = 0;
		}

		// Apply handbrake if the space key is pressed
		if (Input.GetKey(KeyCode.Space))
		{
			rearright.brakeTorque = handBrakeTorque;
			rearleft.brakeTorque = handBrakeTorque;
		}
		else
		{
			if (rearrot >= minRPM)
			{
				frontright.brakeTorque = brakeinput * BrakeTorque * 0.8f * 5;
				frontleft.brakeTorque = brakeinput * BrakeTorque * 0.8f * 5;
				rearleft.brakeTorque = decelerationFactor + brakeinput * BrakeTorque * 0.2f * 5;
				rearright.brakeTorque = decelerationFactor + brakeinput * BrakeTorque * 0.2f * 5;
			}
			else
			{
				// Apply front and rear brake torques, including controlledBrakeTorque
				frontright.brakeTorque = brakeinput * BrakeTorque * 0.8f * 5;
				frontleft.brakeTorque = brakeinput * BrakeTorque * 0.8f * 5;
				rearleft.brakeTorque = brakeinput * BrakeTorque * 0.2f * 5;
				rearright.brakeTorque = brakeinput * BrakeTorque * 0.2f * 5;
			}
			
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
				rearleft.GetWorldPose(out wheelposition, out wheelRotation);

			}
            else if(i==1) 
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

	void enginesound(float motor)
	{

			//motor maximum value is 5000, so i want to divid the motor by a value greater than 7000 \, so i give part of the sound to the rearrot also.
			Debug.Log("motor: " + Mathf.Abs(motor));


			engineAudioSource.volume = 0.25f + (Mathf.Abs(motor) * (0.75f / 9000f));
		if(MathF.Abs(rearrot)<=2000)
		{
			engineAudioSource.volume += (Mathf.Abs(rearrot) / 5000f);
		}
		else
		{
			engineAudioSource.volume += (2000/5000f);
		}


			engineAudioSource.pitch = 1 + (Mathf.Abs(motor) / 9000f);
		if (MathF.Abs(rearrot) <= 2000)
		{
			engineAudioSource.pitch += (Mathf.Abs(rearrot) / 5000f);
		}
		else
		{
			engineAudioSource.pitch += (2000/5000f);
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

	// Method to check if the wheel is on the ground
	private bool IsWheelOnGround(WheelCollider wheel)
	{
		WheelHit hit;
		return wheel.GetGroundHit(out hit);
	}

}

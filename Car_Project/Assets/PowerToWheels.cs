using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//note!!!!!!!!!!! 1kmh is equal to 0.75 in the velocity of unity
public class PowerToWheels : MonoBehaviour
{
	public float speedKMH;

	public float Torque = 700;
	public float handBrakeTorque = 3000f;
	public float BrakeTorque = 5000f;
	private float slipangle;
	public float speed;
	public float gasinput;
	public float steeringInput;
	public float brakeinput;
	public float rearrot;
	public float frontrot;
	public float rearsidewaysfriction;

	float radius;
	float wheelCircumference;

	//if one of those is equal to 0, means the tyre is in the air, if its other, 1 would be for road, 2 for offroad,etc... should work more on it
	public Dictionary<WheelCollider, int> wheelGroundStates;

	public float steeringMax = 27;
	float maxSpeed = 320f;
	public AnimationCurve steeringCurve;
	public WheelCollider rearright;
	public WheelCollider rearleft;

	public WheelCollider frontleft;
	public WheelCollider frontright;

	public GameObject[] wheels = new GameObject[4];


	public float maxsmokesize;
	

	public GameObject centerofmass;
	private Rigidbody rigidbody;

	public Rigidbody car;



	public TrailRenderer[] fronttyremarks;
	public TrailRenderer[] reartyremarks;

	bool engineturnon = false;

	public float decelerationFactor = 300f;
	//public float maxBrakeForce = 200f;
	public float minRPM = 100f;

	public float frontLeftLoad;
	public float frontRightLoad;
	public float rearLeftLoad;
	public float rearRightLoad;

	public float rearrighttorque;
	public float rearlefttorque;

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

		ParticleSystem.MainModule main = rearwheelsmoke[0].main;
		maxsmokesize = main.startSize.constant;



		rearsidewaysfriction = rearleft.sidewaysFriction.stiffness;
		Vector3 cardim = cardimensions.bounds.size;
		Debug.Log("car dimensions (meters): " + cardim);

		Vector3 wheeldim = wheeldimensions.bounds.size;
		Debug.Log("wheel dimensions (meters): " + wheeldim);

		Vector3 housedim = housedimensions.bounds.size;
		Debug.Log("house dimensions (meters): " + housedim);
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
		speed = rigidbody.velocity.magnitude;
		speedKMH = Mathf.RoundToInt(speed) * 3.6f;

		rearlefttorque = rearleft.motorTorque;
		rearrighttorque = rearright.motorTorque;
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


		UpdateWheelGroundState(frontleft);
		UpdateWheelGroundState(frontright);
		UpdateWheelGroundState(rearleft);
		UpdateWheelGroundState(rearright);


		frontLeftLoad = GetWheelLoad(frontleft);
		frontRightLoad = GetWheelLoad(frontright);
		rearLeftLoad = GetWheelLoad(rearleft);
		rearRightLoad = GetWheelLoad(rearright);

		gasinput = Input.GetAxis("Vertical");
		steeringInput = Input.GetAxis("Horizontal");

		animatewheels();
		braking();
		steering();
		gas();

		Debug.DrawRay(transform.position, transform.position.normalized * 10);
		Debug.DrawRay(transform.position, transform.position.normalized * 10, Color.blue);
	}

	public float NormalizeSmokeSize(float value, float minThreshold, float maxThreshold)
	{
		float normalizedValue = Mathf.InverseLerp(minThreshold, maxThreshold, value);
		
		return normalizedValue;
	}
	public float smokeintensity(float maxSmokeSize, float normalizedValue)
	{
		float smokeIntensity = Mathf.Lerp(0, maxSmokeSize, normalizedValue);

		return smokeIntensity;
	}

	public float rearlock()
	{
		float rearsmoke = 0;
		float maxsmoke = 2 * (maxsmokesize) / 3f;
		float slipnormalizedvalue = 1;//cause there is only one threshold, so it is or actived or not
		float speednormalizedvalue;
		float finalnormalizedvalue;

		if (rearrot == 0 && Math.Abs(speed) > 5)
		{
			speednormalizedvalue = NormalizeSmokeSize(speedKMH, 0, maxSpeed);
			finalnormalizedvalue = (slipnormalizedvalue + speednormalizedvalue) / 2f;

			rearsmoke = smokeintensity(maxsmoke, finalnormalizedvalue);
		}
		return rearsmoke;
	}
	public float CarSliding()
	{
		float rearsmoke = 0;
		float maxsmoke = 2 * (maxsmokesize) / 3f;
		float slipnormalizedvalue;
		float speednormalizedvalue;
		float finalnormalizedvalue;

		Vector3 forwardVector = transform.forward;
		Vector3 velocityVector = rigidbody.velocity;

		forwardVector.Normalize();
		velocityVector.Normalize();

		//the lower the number of the dotProduct, the harder the curve is
		float dotProduct = Vector3.Dot(forwardVector, velocityVector);

		float driftThresholdMin = 0.99f;
		float driftThresholdMax = 0.7f;

		if (Mathf.Abs(dotProduct) > driftThresholdMin)
		{
			slipnormalizedvalue = NormalizeSmokeSize(Mathf.Abs(dotProduct), driftThresholdMin, driftThresholdMax);
			speednormalizedvalue = NormalizeSmokeSize(speedKMH, 0, maxSpeed);
			finalnormalizedvalue = (slipnormalizedvalue + speednormalizedvalue) / 2f;

			rearsmoke = smokeintensity(maxsmoke, finalnormalizedvalue);
		}
		return rearsmoke;
	}
	public float burnout()
	{
		float rearsmoke = 0;
		float maxsmoke = 2 * (maxsmokesize) / 3f;
		float slipnormalizedvalue;
		float speednormalizedvalue;
		float finalnormalizedvalue;

		float frontAbs = Math.Abs(frontrot);
		float rearAbs = Math.Abs(rearrot);

		float driftThresholdMin = frontAbs * 3f;
		float driftThresholdMax = frontAbs * 4f;

		if (rearAbs > driftThresholdMin && speedKMH > 4)
		{
			slipnormalizedvalue = NormalizeSmokeSize(rearAbs, driftThresholdMin, driftThresholdMax);
			speednormalizedvalue = NormalizeSmokeSize(speedKMH, 0, maxSpeed);
			finalnormalizedvalue = (slipnormalizedvalue + speednormalizedvalue) / 2f;

			rearsmoke = smokeintensity(maxsmoke, finalnormalizedvalue);
		}
		return rearsmoke;
	}


	public float frontwheelslide()
	{
		float frontsmoke = 0;
		float maxsmoke = 2 * (maxsmokesize) / 3f;
		float slipnormalizedvalue = 1;//cause there is only one threshold, so it is or actived or not
		float speednormalizedvalue;
		float finalnormalizedvalue;

		Vector3 forwardVector = transform.forward;
		Vector3 velocityVector = rigidbody.velocity;

		forwardVector.Normalize();
		velocityVector.Normalize();

		float dotProduct = Vector3.Dot(forwardVector, velocityVector);

		float driftThreshold = 0.3f;

		if (Mathf.Abs(dotProduct) < driftThreshold)
		{
			speednormalizedvalue = NormalizeSmokeSize(speedKMH, 0, maxSpeed);
			finalnormalizedvalue = (slipnormalizedvalue + speednormalizedvalue) / 2f;
		}

		return frontsmoke;
	}
	public float frontlock()
	{
		float frontsmoke = 0;
		float maxsmoke = (maxsmokesize) / 3f;
		float slipnormalizedvalue = 1;//cause there is only one threshold, so it is or actived or not
		float speednormalizedvalue;
		float finalnormalizedvalue;

		if (frontrot == 0 && Math.Abs(speed) > 5)
		{
			speednormalizedvalue = NormalizeSmokeSize(speedKMH, 0, maxSpeed);
			finalnormalizedvalue = (slipnormalizedvalue + speednormalizedvalue) / 2f;

			frontsmoke = smokeintensity(maxsmoke, finalnormalizedvalue);
		}
		return frontsmoke;
		
	}


	void gas()
	{
		if (engineturnon)
		{
			if (gasinput < 0)
			{
				gasinput /= 1.5f;
			}

			float motor = gasinput * Torque;

			/*
			if (Mathf.Abs(rearleft.rpm) >= 5000f)
			{
				motor = 0;
			}
			*/
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
			rearright.brakeTorque = handBrakeTorque / 2;
			rearleft.brakeTorque = handBrakeTorque / 2;
			if(rearrot==0)
			{
				WheelFrictionCurve rearLeftFriction = rearleft.sidewaysFriction;
				rearLeftFriction.stiffness = 0.1f;
				rearleft.sidewaysFriction = rearLeftFriction;

				WheelFrictionCurve rearRightFriction = rearright.sidewaysFriction;
				rearRightFriction.stiffness = 0.1f;
				rearright.sidewaysFriction = rearRightFriction;

			}
			
		}
		else
		{
			//70% front wheel, 30% rear wheels
			if (Math.Abs(rearrot) >= minRPM)
			{
				frontright.brakeTorque = brakeinput * BrakeTorque * 0.35f;
				frontleft.brakeTorque = brakeinput * BrakeTorque * 0.35f;
				rearleft.brakeTorque = (decelerationFactor/2f) + brakeinput * BrakeTorque * 0.15f;
				rearright.brakeTorque = (decelerationFactor/2f) + brakeinput * BrakeTorque * 0.15f;
			}
			else
			{
				// Apply front and rear brake torques, including controlledBrakeTorque
				frontright.brakeTorque = brakeinput * BrakeTorque * 0.35f;
				frontleft.brakeTorque = brakeinput * BrakeTorque * 0.35f;
				rearleft.brakeTorque = brakeinput * BrakeTorque * 0.15f;
				rearright.brakeTorque = brakeinput * BrakeTorque * 0.15f;
			}	

		}
		if (rearrot != 0)
		{
			WheelFrictionCurve rearLeftFriction = rearleft.sidewaysFriction;
			rearLeftFriction.stiffness = rearsidewaysfriction;
			rearleft.sidewaysFriction = rearLeftFriction;

			WheelFrictionCurve rearRightFriction = rearright.sidewaysFriction;
			rearRightFriction.stiffness = rearsidewaysfriction;
			rearright.sidewaysFriction = rearRightFriction;
		}

	}
	void steering()
	{
		float steeringAngle = steeringInput * steeringCurve.Evaluate(speedKMH);
		
		if (Input.GetAxis("Horizontal") != 0)
		{
			frontleft.steerAngle = steeringAngle;
			frontright.steerAngle = steeringAngle;
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

	void enginesound(float motor)
	{

		//motor maximum value is 5000, so i want to divid the motor by a value greater than 7000 \, so i give part of the sound to the rearrot also.
		Debug.Log("motor: " + Mathf.Abs(motor));


		engineAudioSource.volume = 0.25f + (Mathf.Abs(motor) * (0.75f / 9000f));
		if (MathF.Abs(rearrot) <= 2000)
		{
			engineAudioSource.volume += (Mathf.Abs(rearrot) / 5000f);
		}
		else
		{
			engineAudioSource.volume += (2000 / 5000f);
		}


		engineAudioSource.pitch = 1 + (Mathf.Abs(motor) / 9000f);
		if (MathF.Abs(rearrot) <= 2000)
		{
			engineAudioSource.pitch += (Mathf.Abs(rearrot) / 5000f);
		}
		else
		{
			engineAudioSource.pitch += (2000 / 5000f);
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
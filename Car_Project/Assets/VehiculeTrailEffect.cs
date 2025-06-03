using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;


public class VehiculeTrailEffect : MonoBehaviour
{
	private CarObjects carobjects;
    private PowerToWheels powertowheels;
	private CarController carcontroller;

    private Rigidbody rigidbody;

	

	private bool rearleftsmokePlaying = false;
	private bool rearrightsmokePlaying = false;

	private bool frontleftsmokePlaying = false;
	private bool frontrightsmokePlaying = false;

	bool Frontlockstarted = false;


	private bool isReady = true;
	private float tirescreechcooldownTime = 0.25f;
	private float timer;


	
	public float maxsmokesize;
	public float maxsmokespeed = 250f;
	private SmokeDataRear rearsmokeData;
	private SmokeDataFront frontsmokeData;


	// Start is called before the first frame update
	void Start()
	{
		carobjects = GetComponent<CarObjects>();
        powertowheels = GetComponent<PowerToWheels>();
		carcontroller = GetComponent<CarController>();

        ParticleSystem.MainModule main = carobjects.rearwheelsmoke[0].main;
		maxsmokesize = main.startSize.constant;
		Debug.Log(maxsmokesize);


		if (powertowheels == null)
		{
			Debug.LogError("PowerToWheels script not found on the same GameObject!");
		}

		rigidbody = GetComponent<Rigidbody>();

		frontsmokeData = new SmokeDataFront();
		rearsmokeData = new SmokeDataRear();

        
	}

	// Update is called once per frame
	void Update()
	{

		if (rigidbody.velocity.magnitude > 2)
		{
			
			frontsmokeData.Frontwheelslide(transform.forward, rigidbody.velocity,0.3f);
			frontsmokeData.Frontlock(powertowheels.frontrot, powertowheels.speed);
			frontsmokeData.AddSpeed(powertowheels.speedKMH, maxsmokespeed);
			if (frontsmokeData.slipNormalizedValueFrontSlide != 0 || frontsmokeData.slipNormalizedValueFrontLock != 0)
			{
				float frontsmoke = frontsmokeData.CalculateFinalSmokeIntensity(maxsmokesize);
				ParticleSystem.MainModule left = carobjects.frontwheelsmoke[0].main;
				left.startSize = frontsmoke;

				ParticleSystem.MainModule right = carobjects.frontwheelsmoke[1].main;
				right.startSize = frontsmoke;


                //if (!Frontlockstarted)
                //{
                //	tirescreechAudioSource.clip = tirescreechingClip;
                //	tirescreechAudioSource.loop = false;
                //	tirescreechAudioSource.Play();
                //	Frontlockstarted = true;
                //}

                foreach (TrailRenderer T in carobjects.fronttyremarks)
                {
                    T.emitting = true;
                }

                // Handle front-left wheel ground state
                if (carcontroller.wheelGroundStates[carobjects.wheelColliders["FrontLeft"]] == 1)
                {
                    if (carobjects.frontwheelsmoke[0] == null)
                    {
                        carobjects.frontwheelsmoke[0] = Instantiate(carobjects.frontwheelsmoke[0], carobjects.wheelColliders["FrontLeft"].transform.position, Quaternion.identity);
                    }
                    else
                    {
                        carobjects.frontwheelsmoke[0].transform.position = carobjects.wheelColliders["FrontLeft"].transform.position;
                        if (!frontleftsmokePlaying)
                        {
                            carobjects.frontwheelsmoke[0].Play();
                            frontleftsmokePlaying = true;
                        }
                    }
                }

                // Handle front-right wheel ground state
                if (carcontroller.wheelGroundStates[carobjects.wheelColliders["FrontRight"]] == 1)
                {
                    if (carobjects.frontwheelsmoke[1] == null)
                    {
                        carobjects.frontwheelsmoke[1] = Instantiate(carobjects.frontwheelsmoke[1], carobjects.wheelColliders["FrontRight"].transform.position, Quaternion.identity);
                    }
                    else
                    {
                        carobjects.frontwheelsmoke[1].transform.position = carobjects.wheelColliders["FrontRight"].transform.position;
                        if (!frontrightsmokePlaying)
                        {
                            carobjects.frontwheelsmoke[1].Play();
                            frontrightsmokePlaying = true;
                        }
                    }
                }
            }
            else
            {
                // Deactivate the trail effect for the front wheels
                foreach (TrailRenderer T in carobjects.fronttyremarks)
                {
                    T.emitting = false;
                }

                // Stop front-left smoke if it's playing
                if (frontleftsmokePlaying)
                {
                    carobjects.frontwheelsmoke[0].Stop();
                    frontleftsmokePlaying = false;
                }

                // Stop front-right smoke if it's playing
                if (frontrightsmokePlaying)
                {
                    carobjects.frontwheelsmoke[1].Stop();
                    frontrightsmokePlaying = false;
                }

                // Handle tire screech audio when front lock starts
                if (Frontlockstarted)
                {
                    // Stop the tire screech audio
                    carobjects.tirescreechAudioSource.Stop();

                    // Set new audio clip and play it
                    carobjects.tirescreechAudioSource.clip = carobjects.tirescreechingendClip;
                    carobjects.tirescreechAudioSource.loop = false;
                    carobjects.tirescreechAudioSource.Play();

                    // Reset the front lock started flag
                    Frontlockstarted = false;
                }
            }


            rearsmokeData.Rearlock(powertowheels.rearrot, powertowheels.speed);
			rearsmokeData.CarSliding(transform.forward, rigidbody.velocity, 0.99f, 0.7f);
			rearsmokeData.Burnout(powertowheels.frontrot, powertowheels.rearrot);
			rearsmokeData.AddSpeed(powertowheels.speedKMH, maxsmokespeed);

            if (rearsmokeData.slipNormalizedValueRearlock != 0 || ((rearsmokeData.slipNormalizedValueCarSliding != 0) && powertowheels.speedKMH > 15) || rearsmokeData.slipNormalizedValueBurnout != 0)
            {
                // Calculate the final smoke intensity
                float rearsmoke = rearsmokeData.CalculateFinalSmokeIntensity(maxsmokesize);

                // Set the particle system's size for both rear wheels
                ParticleSystem.MainModule left = carobjects.rearwheelsmoke[0].main;
                left.startSize = rearsmoke;

                ParticleSystem.MainModule right = carobjects.rearwheelsmoke[1].main;
                right.startSize = rearsmoke;

                // Activate the trail effect for the rear wheels
                foreach (TrailRenderer T in carobjects.backtyremarks)
                {
                    T.emitting = true;
                }

                // Handle rear-left wheel ground state using the carcontroller
                if (carcontroller.wheelGroundStates[carobjects.wheelColliders["RearLeft"]] == 1)
                {
                    if (carobjects.rearwheelsmoke[0] == null)
                    {
                        carobjects.rearwheelsmoke[0] = Instantiate(carobjects.rearwheelsmoke[0], carobjects.wheelColliders["RearLeft"].transform.position, Quaternion.identity);
                    }
                    else
                    {
                        carobjects.rearwheelsmoke[0].transform.position = carobjects.wheelColliders["RearLeft"].transform.position;
                        if (!rearleftsmokePlaying)
                        {
                            carobjects.rearwheelsmoke[0].Play();
                            rearleftsmokePlaying = true;
                        }
                    }
                }

                // Handle rear-right wheel ground state using the carcontroller
                if (carcontroller.wheelGroundStates[carobjects.wheelColliders["RearRight"]] == 1)
                {
                    if (carobjects.rearwheelsmoke[1] == null)
                    {
                        carobjects.rearwheelsmoke[1] = Instantiate(carobjects.rearwheelsmoke[1], carobjects.wheelColliders["RearRight"].transform.position, Quaternion.identity);
                    }
                    else
                    {
                        carobjects.rearwheelsmoke[1].transform.position = carobjects.wheelColliders["RearRight"].transform.position;
                        if (!rearrightsmokePlaying)
                        {
                            carobjects.rearwheelsmoke[1].Play();
                            rearrightsmokePlaying = true;
                        }
                    }
                }
            }


            else
            {
				foreach (TrailRenderer T in carobjects.backtyremarks)
				{
					T.emitting = false;
				}
				if (rearleftsmokePlaying)
				{
                    carobjects.rearwheelsmoke[0].Stop();
					rearleftsmokePlaying = false;
				}

				if (rearrightsmokePlaying)
				{
                    carobjects.rearwheelsmoke[1].Stop();
					rearrightsmokePlaying = false;
				}
			}
			
			
			
		}
		
		else
		{
			
			foreach (TrailRenderer T in carobjects.fronttyremarks)
			{
				T.emitting = false;
			}
			if (frontleftsmokePlaying)
			{
                carobjects.frontwheelsmoke[0].Stop();
				frontleftsmokePlaying = false;
			}

			if (frontrightsmokePlaying)
			{
                carobjects.frontwheelsmoke[1].Stop();
				frontrightsmokePlaying = false;
			}


			if (rearleftsmokePlaying)
			{
                carobjects.rearwheelsmoke[0].Stop();
				rearleftsmokePlaying = false;
			}

			if (rearrightsmokePlaying)
			{
                carobjects.rearwheelsmoke[1].Stop();
				rearrightsmokePlaying = false;
			}
			
		}
		//if (Input.GetKeyDown(KeyCode.Y))
		//{
		//	Debug.Log("pressed Y");
		//	ParticleSystem.MainModule left = rearwheelsmoke[0].main;
		//	left.startSize = left.startSize.constant - 1;

		//	ParticleSystem.MainModule right = rearwheelsmoke[1].main;
		//	right.startSize = left.startSize.constant - 1;
		//}
		//if (Input.GetKeyDown(KeyCode.U))
		//{
		//	Debug.Log("pressed U");
		//	ParticleSystem.MainModule left = rearwheelsmoke[0].main;
		//	left.startSize = left.startSize.constant + 1;

		//	ParticleSystem.MainModule right = rearwheelsmoke[1].main;
		//	right.startSize = left.startSize.constant + 1;
		//}
		
	}
	public class SmokeDataRear
	{
		public float slipNormalizedValueRearlock;
		public float slipNormalizedValueCarSliding;
		public float slipNormalizedValueBurnout;
		public float speedNormalizedValue;
		public float smokeIntensity;

		// Method to calculate the rearlock slip value (without speed normalization)
		public float Rearlock(float rearRot, float speed)
		{
			if (rearRot == 0 && Mathf.Abs(speed) > 5)
			{
				slipNormalizedValueRearlock = 1; // Fully active
			}
			else
			{
				slipNormalizedValueRearlock = 0; // Not active
			}

			return slipNormalizedValueRearlock;
		}

		public float CarSliding(Vector3 forwardVector, Vector3 velocityVector, float driftThresholdMin, float driftThresholdMax)
		{
			float dotProduct = Vector3.Dot(forwardVector.normalized, velocityVector.normalized);

			slipNormalizedValueCarSliding = Mathf.InverseLerp(driftThresholdMin, driftThresholdMax, Mathf.Abs(dotProduct));

			return slipNormalizedValueCarSliding;
		}

		public float Burnout(float front, float rear)
		{
			float frontAbs = Math.Abs(front);
			float rearAbs = Math.Abs(rear);
			float driftThresholdMin = frontAbs * 3f;
			float driftThresholdMax = frontAbs * 4f;

			if (rearAbs > driftThresholdMin)
			{
				slipNormalizedValueBurnout = Mathf.InverseLerp(driftThresholdMin, driftThresholdMax, rearAbs);
			}
			else
			{
				slipNormalizedValueBurnout = 0;
			}

			return slipNormalizedValueBurnout;
		}

		public float AddSpeed(float speedKMH, float maxSpeed)
		{
			speedNormalizedValue = Mathf.InverseLerp(0, maxSpeed, speedKMH);
			return speedNormalizedValue;
		}

		// Method to calculate the final smoke intensity
		public float CalculateFinalSmokeIntensity(float maxSmokeSize)
		{
			float combinedValue = (slipNormalizedValueRearlock + slipNormalizedValueCarSliding + slipNormalizedValueBurnout + 2 * speedNormalizedValue) / 4f;
			smokeIntensity = Mathf.Lerp(0, maxSmokeSize, combinedValue);

			return smokeIntensity;
		}
	}


	public class SmokeDataFront
	{
		public float slipNormalizedValueFrontSlide;
		public float slipNormalizedValueFrontLock;
		public float speedNormalizedValue;
		public float smokeIntensity;

		public float Frontwheelslide(Vector3 forwardVector, Vector3 velocityVector, float driftThreshold)
		{
			float dotProduct = Vector3.Dot(forwardVector.normalized, velocityVector.normalized);

			if (Mathf.Abs(dotProduct) < driftThreshold)
			{
				slipNormalizedValueFrontSlide = 1;
			}
			else
			{
				slipNormalizedValueFrontSlide = 0;
			}

			return slipNormalizedValueFrontSlide;
		}

		public float Frontlock(float frontRot, float speed)
		{
			if (frontRot == 0 && Mathf.Abs(speed) > 5)
			{
				slipNormalizedValueFrontLock = 1;
			}
			else
			{
				slipNormalizedValueFrontLock = 0;
			}

			return slipNormalizedValueFrontLock;
		}

		public float AddSpeed(float speedKMH, float maxSpeed)
		{
			speedNormalizedValue = Mathf.InverseLerp(0, maxSpeed, speedKMH);
			return speedNormalizedValue;
		}

		public float CalculateFinalSmokeIntensity(float maxSmokeSize)
		{
			float combinedValue = (slipNormalizedValueFrontSlide + slipNormalizedValueFrontLock + 2 * speedNormalizedValue) / 3f;
			smokeIntensity = Mathf.Lerp(0, maxSmokeSize, combinedValue);

			return smokeIntensity;
		}
	}


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VehiculeTrailEffect : MonoBehaviour
{
	private Rigidbody rigidbody;

	public TrailRenderer[] fronttyremarks;
	public TrailRenderer[] backtyremarks;

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

	bool frontlockstarted = false;


	private bool isReady = true;
	private float tirescreechcooldownTime = 0.25f;
	private float timer;

	private PowerToWheels powertowheels;

	public ParticleSystem[] frontwheelsmoke = new ParticleSystem[2];
	public ParticleSystem[] rearwheelsmoke = new ParticleSystem[2]; 
	
	// Start is called before the first frame update
	void Start()
	{
		powertowheels = GetComponent<PowerToWheels>();

		if (powertowheels == null)
		{
			Debug.LogError("PowerToWheels script not found on the same GameObject!");
		}

		rigidbody = GetComponent<Rigidbody>();

		
	}

	// Update is called once per frame
	void Update()
	{

		if (rigidbody.velocity.magnitude > 2)
		{

			if (powertowheels.frontlock() != 0 || powertowheels.frontwheelslide() != 0)
			{

				/*
				if (!frontlockstarted)
				{
					tirescreechAudioSource.clip = tirescreechingClip;
					tirescreechAudioSource.loop = false;
					tirescreechAudioSource.Play();
					frontlockstarted = true;
				}
				*/
				foreach (TrailRenderer T in fronttyremarks)
				{
					T.emitting = true;
				}
				if (powertowheels.wheelGroundStates[powertowheels.frontleft] == 1)/////////////////////////
				{
					if (frontleftsmoke == null)
					{
						frontleftsmoke = Instantiate(frontLeftSmokePrefab, powertowheels.frontleft.transform.position, Quaternion.identity);
					}
					else
					{
						frontleftsmoke.transform.position = powertowheels.frontleft.transform.position;
						if (!frontleftsmokePlaying)
						{
							frontleftsmoke.Play();
							frontleftsmokePlaying = true;
						}
					}
				}
				if (powertowheels.wheelGroundStates[powertowheels.frontright] == 1)//////////////////////
				{
					if (frontrightsmoke == null)
					{
						frontrightsmoke = Instantiate(frontRightSmokePrefab, powertowheels.frontright.transform.position, Quaternion.identity);
					}
					else
					{
						frontrightsmoke.transform.position = powertowheels.frontright.transform.position;
						if (!frontrightsmokePlaying)
						{
							frontrightsmoke.Play();
							frontrightsmokePlaying = true;
						}
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


				if (frontlockstarted)
				{
					powertowheels.tirescreechAudioSource.Stop();

					powertowheels.tirescreechAudioSource.clip = powertowheels.tirescreechingendClip;
					powertowheels.tirescreechAudioSource.loop = false;
					powertowheels.tirescreechAudioSource.Play();
					frontlockstarted = false;
				}

			}

			
			if (powertowheels.CarSliding()!=0 ||  powertowheels.rearlock()!=0  || powertowheels.burnout()!=0)
			{
				
				foreach (TrailRenderer T in backtyremarks)
				{
					T.emitting = true;
				}
				if (powertowheels.wheelGroundStates[powertowheels.rearleft]==1)//////////////////////
				{
					if (rearleftsmoke == null)
					{
						rearleftsmoke = Instantiate(rearLeftSmokePrefab, powertowheels.rearleft.transform.position, Quaternion.identity);
					}
					else
					{
						rearleftsmoke.transform.position = powertowheels.rearleft.transform.position;
						if (!rearleftsmokePlaying)
						{
							rearleftsmoke.Play();
							rearleftsmokePlaying = true;
						}
					}
				}
				if (powertowheels.wheelGroundStates[powertowheels.rearright] == 1)/////////////////////
				{
					if (rearrightsmoke == null)
					{
						rearrightsmoke = Instantiate(rearRightSmokePrefab, powertowheels.rearright.transform.position, Quaternion.identity);
					}
					else
					{
						rearrightsmoke.transform.position = powertowheels.rearright.transform.position;
						if (!rearrightsmokePlaying)
						{
							rearrightsmoke.Play();
							rearrightsmokePlaying = true;
						}
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

	
}


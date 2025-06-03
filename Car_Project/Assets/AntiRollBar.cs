using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour 
{
	CarObjects carobjects;
	CarController carController;

	public float AntiRoll = 3000;
	private Rigidbody car;


	void Start()
	{
		carobjects = GetComponent<CarObjects>();
		carController = GetComponent<CarController>();
		car = GetComponent<Rigidbody>();
        
    }

	void FixedUpdate ()
	{
		WheelHit hit;
		float travelL = 1.0f;
		float travelR = 1.0f;


		bool groundedL = carobjects.wheelColliders["RearLeft"].GetGroundHit (out hit);
		if (groundedL) 
		{
			travelL = (-carobjects.wheelColliders["RearLeft"].transform.InverseTransformPoint (hit.point).y - carobjects.wheelColliders["RearLeft"].radius) / carobjects.wheelColliders["RearLeft"].suspensionDistance;
		}

		bool groundedR = carobjects.wheelColliders["RearRight"].GetGroundHit (out hit);
		if (groundedR) 
		{
			travelR = (-carobjects.wheelColliders["RearRight"].transform.InverseTransformPoint (hit.point).y - carobjects.wheelColliders["RearRight"].radius) / carobjects.wheelColliders["RearRight"].suspensionDistance;
		}

		float antiRollForce = (travelL - travelR) * AntiRoll;

		if (groundedL)
			car.AddForceAtPosition (carobjects.wheelColliders["RearLeft"].transform.up * -antiRollForce, carobjects.wheelColliders["RearLeft"].transform.position);

		if (groundedR)
			car.AddForceAtPosition (carobjects.wheelColliders["RearRight"].transform.up * antiRollForce, carobjects.wheelColliders["RearRight"].transform.position);
	}
}

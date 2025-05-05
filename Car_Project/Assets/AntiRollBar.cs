using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour 
{

	CarController carController;


    public WheelCollider WheelL;
	public WheelCollider WheelR;
	public float AntiRoll = 3000;

	private Rigidbody car;

	void Start(){
		car = GetComponent<Rigidbody> ();
        carController = GetComponent<CarController>();
        if (carController != null)
        {
            WheelL = carController.wheelColliders["FrontLeft"];
            WheelR = carController.wheelColliders["FrontRight"];
        }
        else
        {
            Debug.LogError("[AntiRollBar] CarController not found on " + gameObject.name);
        }
    }

	void FixedUpdate ()
	{
		WheelHit hit;
		float travelL = 1.0f;
		float travelR = 1.0f;


		bool groundedL = WheelL.GetGroundHit (out hit);
		if (groundedL) 
		{
			travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
		}

		bool groundedR = WheelR.GetGroundHit (out hit);
		if (groundedR) 
		{
			travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
		}

		float antiRollForce = (travelL - travelR) * AntiRoll;

		if (groundedL)
			car.AddForceAtPosition (WheelL.transform.up * -antiRollForce, WheelL.transform.position);

		if (groundedR)
			car.AddForceAtPosition (WheelR.transform.up * antiRollForce, WheelR.transform.position);
	}
}

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PowerToWheels : MonoBehaviour
{
    CarController carcontroller;
    public float finalwheeltorque;
    public float speedKMH;

    //public float CurrTorque;
    private float slipangle;
    public float speed;
    public float gasinput;
    public float steeringInput;
    public float brakeinput;
    public float rearrot;
    public float frontrot;
    public float rearsidewaysfriction;
    public bool istractioncontrol = false;

    public AnimationCurve torque;
    public AnimationCurve steeringCurve;
    float wheelCircumference;

    


    //public float rpm;

   

   
    //tyre glitch fix
    private float rearoriginalSlip;

    private float slipIncrement = 0.5f;
    private float glitchThreshold = 100f;
    private float exitThreshold = 100f;
    private Queue<float> rearframeDifferences = new Queue<float>();

    private int maxFrames = 60;
    private bool inGlitchMode = false;
    private float lastRearrot = 0f;
    //



    bool engineturnon = false;

    //public float decelerationFactor = 300f;
    //public float maxBrakeForce = 200f;
    public float minRPM = 100f;

    

    public float rearrighttorque;
    public float rearlefttorque;

   



    void Start()
    {

        rearoriginalSlip = carcontroller.rearleft.forwardFriction.extremumSlip;
        lastRearrot = rearrot;

        rearsidewaysfriction = carcontroller.rearleft.sidewaysFriction.stiffness;

        


}

    void Update()
    {
        speed = GetComponent<Rigidbody>().velocity.magnitude;
        speedKMH = Mathf.RoundToInt(speed) * 3.6f;

        //debugger
        rearlefttorque = carcontroller.rearleft.motorTorque;
        rearrighttorque = carcontroller.rearright.motorTorque;
        //

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

        if (gasinput == 0 || rearrot < 500)
        {
            float rearcurrentDifference = Mathf.Abs(rearrot - lastRearrot);

            if ((rearcurrentDifference > glitchThreshold))
            {
                EnterGlitchMode();

            }

            rearframeDifferences.Enqueue(rearcurrentDifference);
            if (rearframeDifferences.Count > maxFrames)
            {
                rearframeDifferences.Dequeue();
            }

            if ((inGlitchMode) && (rearframeDifferences.Sum() < exitThreshold))
            {
                ExitGlitchMode();
            }
        }
        else
        {
            ExitGlitchMode();
        }

        lastRearrot = rearrot;

        Debug.DrawRay(transform.position, transform.position.normalized * 10);
        Debug.DrawRay(transform.position, transform.position.normalized * 10, Color.blue);
    }




    void EnterGlitchMode()
    {
        if (!inGlitchMode)
        {
            inGlitchMode = true;

            WheelFrictionCurve leftFriction = rearleft.forwardFriction;
            WheelFrictionCurve rightFriction = rearright.forwardFriction;

            leftFriction.extremumSlip += slipIncrement;
            rightFriction.extremumSlip += slipIncrement;

            rearleft.forwardFriction = leftFriction;
            rearright.forwardFriction = rightFriction;

            //WheelFrictionCurve frontLeftFriction = frontleft.forwardFriction;
            //WheelFrictionCurve frontRightFriction = frontright.forwardFriction;

            //frontLeftFriction.extremumSlip += slipIncrement;
            //frontRightFriction.extremumSlip += slipIncrement;

            //frontleft.forwardFriction = frontLeftFriction;
            //frontright.forwardFriction = frontRightFriction;
        }
    }
    void ExitGlitchMode()
    {
        if (inGlitchMode)
        {
            inGlitchMode = false;

            WheelFrictionCurve leftFriction = rearleft.forwardFriction;
            WheelFrictionCurve rightFriction = rearright.forwardFriction;

            leftFriction.extremumSlip = rearoriginalSlip;
            rightFriction.extremumSlip = rearoriginalSlip;

            rearleft.forwardFriction = leftFriction;
            rearright.forwardFriction = rightFriction;

            //WheelFrictionCurve frontLeftFriction = frontleft.forwardFriction;
            //WheelFrictionCurve frontRightFriction = frontright.forwardFriction;

            //frontLeftFriction.extremumSlip = rearoriginalSlip;
            //frontRightFriction.extremumSlip = rearoriginalSlip;

            //frontleft.forwardFriction = frontLeftFriction;
            //frontright.forwardFriction = frontRightFriction;
        }
    }


    void gas()
    {
        if (engineturnon)
        {
            if (gasinput < 0)
            {
                gasinput /= 1.5f;
            }

            //do the rest of the code here
            finalwheeltorque = tractioncontrol(finalwheeltorque);
                    
            enginesound(finalwheeltorque);
        }
        else
        {
            rearleft.motorTorque = 0;
            rearright.motorTorque = 0;
        }
    }
    float gears(float ratio)
    {

        if (rearrot < (1000 / ratio))
        {
        }
        else
        { 
        }

        

    }
    void braking()
    {
        // Calculate slip angle first
        slipangle = Vector3.Angle(transform.forward, GetComponent<Rigidbody>().velocity - transform.forward);


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
            rearright.brakeTorque = HandBrakeTorque / 2;
            rearleft.brakeTorque = HandBrakeTorque / 2;
            if (rearrot == 0)
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
                rearleft.brakeTorque = (decelerationFactor / 2f) + brakeinput * BrakeTorque * 0.15f;
                rearright.brakeTorque = (decelerationFactor / 2f) + brakeinput * BrakeTorque * 0.15f;
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
    float tractioncontrol(float currtorque)
    {
        float threshold = 2f;
        if ((MathF.Abs(rearrot) > MathF.Abs(threshold * frontrot)) && !istractioncontrol)
        {
            currtorque *= tractioncutoff;
            Debug.Log("traction control yes");

            istractioncontrol = true;
        }
        if ((istractioncontrol) && (MathF.Abs(rearrot) <= MathF.Abs(frontrot + (frontrot / 10f))))
        {
            currtorque = MaxTorque;
            istractioncontrol = false;

        }
        return currtorque;
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
        //Debug.Log("motor: " + Mathf.Abs(motor));


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
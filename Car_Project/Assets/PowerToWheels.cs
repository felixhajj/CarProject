using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PowerToWheels : MonoBehaviour
{
    //-the mechanics of the car. everything related to steering, and power translated to the wheels.
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
       
        rearrot = carcontroller.rearleft.rpm;
        frontrot = carcontroller.frontleft.rpm;
    }

    private void FixedUpdate()
    { 
        gasinput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        braking();
        steering();
        gas();

        //tyre stuttering fix
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

            WheelFrictionCurve leftFriction = carcontroller.rearleft.forwardFriction;
            WheelFrictionCurve rightFriction = carcontroller.rearright.forwardFriction;

            leftFriction.extremumSlip += slipIncrement;
            rightFriction.extremumSlip += slipIncrement;

            carcontroller.rearleft.forwardFriction = leftFriction;
            carcontroller.rearright.forwardFriction = rightFriction;

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

            WheelFrictionCurve leftFriction = carcontroller.rearleft.forwardFriction;
            WheelFrictionCurve rightFriction = carcontroller.rearright.forwardFriction;

            leftFriction.extremumSlip = rearoriginalSlip;
            rightFriction.extremumSlip = rearoriginalSlip;

            carcontroller.rearleft.forwardFriction = leftFriction;
            carcontroller.rearright.forwardFriction = rightFriction;

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
        if (carcontroller.engineturnon)
        {
            if (gasinput < 0)
            {
                gasinput /= 1.5f;
            }

            //do the rest of the code here
            finalwheeltorque = tractioncontrol(finalwheeltorque);

            carcontroller.enginesound(finalwheeltorque);
        }
        else
        {
            carcontroller.rearleft.motorTorque = 0;
            carcontroller.rearright.motorTorque = 0;
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
            carcontroller.rearright.brakeTorque = HandBrakeTorque / 2;
            carcontroller.rearleft.brakeTorque = HandBrakeTorque / 2;
            if (rearrot == 0)
            {
                WheelFrictionCurve rearLeftFriction = carcontroller.rearleft.sidewaysFriction;
                rearLeftFriction.stiffness = 0.1f;
                carcontroller.rearleft.sidewaysFriction = rearLeftFriction;

                WheelFrictionCurve rearRightFriction = carcontroller.rearright.sidewaysFriction;
                rearRightFriction.stiffness = 0.1f;
                carcontroller.rearright.sidewaysFriction = rearRightFriction;

            }

        }
        else
        {
            //70% front wheel, 30% rear wheels
            if (Math.Abs(rearrot) >= minRPM)
            {
                carcontroller.frontright.brakeTorque = brakeinput * BrakeTorque * 0.35f;
                carcontroller.frontleft.brakeTorque = brakeinput * BrakeTorque * 0.35f;
                carcontroller.rearleft.brakeTorque = (decelerationFactor / 2f) + brakeinput * BrakeTorque * 0.15f;
                carcontroller.rearright.brakeTorque = (decelerationFactor / 2f) + brakeinput * BrakeTorque * 0.15f;
            }
            else
            {
                // Apply front and rear brake torques, including controlledBrakeTorque
                carcontroller.frontright.brakeTorque = brakeinput * BrakeTorque * 0.35f;
                carcontroller.frontleft.brakeTorque = brakeinput * BrakeTorque * 0.35f;
                carcontroller.rearleft.brakeTorque = brakeinput * BrakeTorque * 0.15f;
                carcontroller.rearright.brakeTorque = brakeinput * BrakeTorque * 0.15f;
            }

        }
        if (rearrot != 0)
        {
            WheelFrictionCurve rearLeftFriction = carcontroller.rearleft.sidewaysFriction;
            rearLeftFriction.stiffness = rearsidewaysfriction;
            carcontroller.rearleft.sidewaysFriction = rearLeftFriction;

            WheelFrictionCurve rearRightFriction = carcontroller.rearright.sidewaysFriction;
            rearRightFriction.stiffness = rearsidewaysfriction;
            carcontroller.rearright.sidewaysFriction = rearRightFriction;
        }

    }
    void steering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speedKMH);

        if (Input.GetAxis("Horizontal") != 0)
        {
            carcontroller.frontleft.steerAngle = steeringAngle;
            carcontroller.frontright.steerAngle = steeringAngle;
        }
        else
        {
            carcontroller.frontleft.steerAngle = 0;
            carcontroller.frontright.steerAngle = 0;
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

    



}
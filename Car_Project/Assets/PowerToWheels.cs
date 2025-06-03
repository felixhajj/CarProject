using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PowerToWheels : MonoBehaviour, IcarInitializer
{
    //-the mechanics of the car. everything related to steering, and power translated to the wheels.

//here, it contains values that are not static, and change overtime. thats why they're here and not 
    private CarObjects carobjects;
    private CarController carcontroller;
    private Car car;



    //tracking values(which i definitely need also)
    public float finalwheeltorque;
    public float speedKMH;
    private float slipangle;
    public float speed;
    public float gasinput;
    public float steeringInput;
    public float brakeinput;
    public float rearrot;
    public float frontrot;
    public float rearsidewayfrictionstiffness;
    public float rearrighttorque;
    public float rearlefttorque;


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

    //other glitch fixes
    public float minRPM = 100f;
    //


    


    public void Initialize(Car ccar)
    {
        this.car = ccar;
    }

    void Start()
    {
        carobjects = GetComponent<CarObjects>();

        var rearLeftFriction = carobjects.wheelColliders["RearLeft"].forwardFriction;
        rearoriginalSlip = rearLeftFriction.extremumSlip;
        lastRearrot = rearrot;

        // Other initialization logic using the mediator
        rearsidewayfrictionstiffness = carobjects.wheelColliders["RearLeft"].sidewaysFriction.stiffness;
    }

    void Update()
    {
        speed = GetComponent<Rigidbody>().velocity.magnitude;
        speedKMH = Mathf.RoundToInt(speed) * 3.6f;

        // Using mediator to get motorTorque and rpm values
        rearlefttorque = carobjects.wheelColliders["RearLeft"].motorTorque;
        rearrighttorque = carobjects.wheelColliders["RearRight"].motorTorque;

        rearrot = carobjects.wheelColliders["RearLeft"].rpm;
        frontrot = carobjects.wheelColliders["FrontLeft"].rpm;
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
            WheelCollider rearLeftWheel = carobjects.wheelColliders["RearLeft"];
            WheelCollider rearRightWheel = carobjects.wheelColliders["RearRight"];

            inGlitchMode = true;

            WheelFrictionCurve leftFriction = rearLeftWheel.forwardFriction;
            WheelFrictionCurve rightFriction = rearRightWheel.forwardFriction;

            leftFriction.extremumSlip += slipIncrement;
            rightFriction.extremumSlip += slipIncrement;


            rearLeftWheel.forwardFriction = leftFriction;
            rearRightWheel.forwardFriction = rightFriction;

        }
    }
    void ExitGlitchMode()
    {
        if (inGlitchMode)
        {
            WheelCollider rearLeftWheel = carobjects.wheelColliders["RearLeft"];
            WheelCollider rearRightWheel = carobjects.wheelColliders["RearRight"];

            inGlitchMode = false;

            WheelFrictionCurve leftFriction = rearLeftWheel.forwardFriction;
            WheelFrictionCurve rightFriction = rearRightWheel.forwardFriction;

            leftFriction.extremumSlip = rearoriginalSlip;
            rightFriction.extremumSlip = rearoriginalSlip;

            rearLeftWheel.forwardFriction = leftFriction;
            rearRightWheel.forwardFriction = rightFriction;

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
            finalwheeltorque = TractionControl(finalwheeltorque);

            /*
             example of adding torque using the mediator
            WheelCollider rearLeft = mediator.GetWheelCollider("rearleft");
            WheelCollider rearRight = mediator.GetWheelCollider("rearright");

            rearLeft.motorTorque = finalwheeltorque;
            rearRight.motorTorque = finalwheeltorque; enginesound(finalwheeltorque);
            */
        }
        else
        {
            WheelCollider rearLeft = carobjects.wheelColliders["RearLeft"];
            WheelCollider rearRight = carobjects.wheelColliders["RearRight"];

            rearLeft.motorTorque = 0;
            rearRight.motorTorque = 0;
        }
    }
    //should use number of driving wheels to divide torque per number of wheels
    /*
    float gears(float ratio)
    {

        if (rearrot < (1000 / ratio))
        {
        }
        else
        { 
        }

        

    }
    */
    void braking()
    {
        WheelCollider rearLeftWheel = carobjects.wheelColliders["RearLeft"];
        WheelCollider rearRightWheel = carobjects.wheelColliders["RearRight"];
        WheelCollider frontLeftWheel = carobjects.wheelColliders["FrontLeft"];
        WheelCollider frontRightWheel = carobjects.wheelColliders["FrontRight"];

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
            float handBrakeTorque = car.HandBrakeTorque;
            rearLeftWheel.brakeTorque = handBrakeTorque / 2;
            rearRightWheel.brakeTorque = handBrakeTorque / 2;


            if (rearrot == 0)
            {
                // when rearwheels locked, they start sliding sideways

                //we use wheelFrictionCurve not because there is a curve, but because wheelcollider stores the friction data like this.
                WheelFrictionCurve rearLeftFriction = rearLeftWheel.sidewaysFriction;
                rearLeftFriction.stiffness = 0.1f;
                rearLeftWheel.sidewaysFriction = rearLeftFriction;


                WheelFrictionCurve rearRightFriction = rearRightWheel.sidewaysFriction;
                rearRightFriction.stiffness = 0.1f;
                rearRightWheel.sidewaysFriction = rearRightFriction;

            }
        }
        else
        {
            float brakeTorque = car.BrakeTorque;
            //to fix later
            //float decelerationFactor = car.GetDecelerationFactor();

            //70% front wheel, 30% rear wheels
            frontRightWheel.brakeTorque = brakeinput * brakeTorque * 0.35f;
            frontLeftWheel.brakeTorque = brakeinput * brakeTorque * 0.35f;
            rearLeftWheel.brakeTorque = brakeinput * brakeTorque * 0.35f;
            rearRightWheel.brakeTorque = brakeinput * brakeTorque * 0.35f;
        }

        if (rearrot != 0)
        {
            WheelFrictionCurve rearLeftFriction = rearLeftWheel.sidewaysFriction;
            rearLeftFriction.stiffness = rearsidewayfrictionstiffness;
            rearLeftWheel.sidewaysFriction = rearLeftFriction;

            WheelFrictionCurve rearRightFriction = rearRightWheel.sidewaysFriction;
            rearRightFriction.stiffness = rearsidewayfrictionstiffness;
            rearRightWheel.sidewaysFriction = rearRightFriction;
        }
    }

    void steering()
    {
        WheelCollider frontLeftWheel = carobjects.wheelColliders["Frontleft"];
        WheelCollider frontRightWheel = carobjects.wheelColliders["FrontRight"];

        float steeringAngle = steeringInput * car.SteeringCurve.Evaluate(speedKMH);

        if (Input.GetAxis("Horizontal") != 0)
        {
            frontLeftWheel.steerAngle = steeringAngle;
            frontRightWheel.steerAngle = steeringAngle;
        }
        else
        {
            frontLeftWheel.steerAngle = 0;
            frontRightWheel.steerAngle = 0;
        }
    }

    float TractionControl(float currTorque)
    {
        float threshold = 2f;
        float tractionCutoff = car.TractionCutoff; // Access via mediator
        float maxTorque = car.MaxTorque; // Access via mediator

        if ((Mathf.Abs(rearrot) > Mathf.Abs(threshold * frontrot)) && !car.IsTractionControl)
        {
            currTorque *= tractionCutoff;
            Debug.Log("traction control yes");
            car.IsTractionControl = true;
        }
        if ((car.IsTractionControl) && (Mathf.Abs(rearrot) <= Mathf.Abs(frontrot + (frontrot / 10f))))
        {
            currTorque = maxTorque;
            car.IsTractionControl = false;
        }

        return currTorque;
    }

    public void enginesound(float motor)
    {
        AudioSource engineAudioSource = carobjects.engineAudioSource; // Get the AudioSource through the mediator
        float rearrot = Mathf.Abs(carobjects.wheelColliders["RearLeft"].rpm); // Access the rearleft wheel's rpm through the mediator

        // motor maximum value is 5000, so divide motor by a value greater than 7000 to give part of the sound to rearrot as well.
        engineAudioSource.volume = 0.25f + (Mathf.Abs(motor) * (0.75f / 9000f));

        if (rearrot <= 2000)
        {
            engineAudioSource.volume += (rearrot / 5000f);
        }
        else
        {
            engineAudioSource.volume += (2000f / 5000f);
        }

        engineAudioSource.pitch = 1 + (Mathf.Abs(motor) / 9000f);

        if (rearrot <= 2000)
        {
            engineAudioSource.pitch += (rearrot / 5000f);
        }
        else
        {
            engineAudioSource.pitch += (2000f / 5000f);
        }
    }


}
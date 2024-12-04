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

    private PowTrailContMediator mediator;
    private CarMediator carMediator;

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



    public void Initialize(PowTrailContMediator mediator)
    {
        this.mediator = mediator;
    }

    public void Initialize(CarMediator mediator)
    {
        this.carMediator = mediator;
    }

    void Start()
    {
        var rearLeftFriction = mediator.GetWheelCollider("rearleft").forwardFriction;
        rearoriginalSlip = rearLeftFriction.extremumSlip;
        lastRearrot = rearrot;

        // Other initialization logic using the mediator
        rearsidewaysfriction = mediator.GetWheelCollider("rearleft").sidewaysFriction.stiffness;
    }

    void Update()
    {
        speed = GetComponent<Rigidbody>().velocity.magnitude;
        speedKMH = Mathf.RoundToInt(speed) * 3.6f;

        // Using mediator to get motorTorque and rpm values
        rearlefttorque = mediator.GetWheelCollider("rearleft").motorTorque;
        rearrighttorque = mediator.GetWheelCollider("rearright").motorTorque;

        rearrot = mediator.GetWheelCollider("rearleft").rpm;
        frontrot = mediator.GetWheelCollider("frontleft").rpm;
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

            WheelFrictionCurve leftFriction = mediator.GetWheelCollider("rearleft").forwardFriction;
            WheelFrictionCurve rightFriction = mediator.GetWheelCollider("rearright").forwardFriction;

            leftFriction.extremumSlip += slipIncrement;
            rightFriction.extremumSlip += slipIncrement;

            mediator.UpdateFriction(mediator.GetWheelCollider("rearleft"), leftFriction);
            mediator.UpdateFriction(mediator.GetWheelCollider("rearright"), rightFriction);

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

            WheelFrictionCurve leftFriction = mediator.GetWheelCollider("rearleft").forwardFriction;
            WheelFrictionCurve rightFriction = mediator.GetWheelCollider("rearright").forwardFriction;

            leftFriction.extremumSlip = rearoriginalSlip;
            rightFriction.extremumSlip = rearoriginalSlip;

            mediator.UpdateFriction(mediator.GetWheelCollider("rearleft"), leftFriction);
            mediator.UpdateFriction(mediator.GetWheelCollider("rearright"), rightFriction);

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
        if (mediator.IsEngineTurnedOn())
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
            WheelCollider rearLeft = mediator.GetWheelCollider("rearleft");
            WheelCollider rearRight = mediator.GetWheelCollider("rearright");

            rearLeft.motorTorque = 0;
            rearRight.motorTorque = 0;
        }
    }
    
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
            float handBrakeTorque = carMediator.GetHandBrakeTorque();
            mediator.SetBrakeTorque(mediator.GetWheelCollider("rearleft"), handBrakeTorque / 2);
            mediator.SetBrakeTorque(mediator.GetWheelCollider("rearright"), handBrakeTorque / 2);

            if (rearrot == 0)
            {
                // Modify friction for handbrake
                WheelFrictionCurve rearLeftFriction = mediator.GetWheelCollider("rearleft").sidewaysFriction;
                rearLeftFriction.stiffness = 0.1f;
                mediator.UpdateFriction(mediator.GetWheelCollider("rearleft"), rearLeftFriction);

                WheelFrictionCurve rearRightFriction = mediator.GetWheelCollider("rearright").sidewaysFriction;
                rearRightFriction.stiffness = 0.1f;
                mediator.UpdateFriction(mediator.GetWheelCollider("rearright"), rearRightFriction);
            }
        }
        else
        {
            float brakeTorque = carMediator.GetBrakeTorque();
            //to fix later
            //float decelerationFactor = carMediator.GetDecelerationFactor();

            //70% front wheel, 30% rear wheels
            if (Math.Abs(rearrot) >= minRPM)
            {
                mediator.SetBrakeTorque(mediator.GetWheelCollider("frontright"), brakeinput * brakeTorque * 0.35f);
                mediator.SetBrakeTorque(mediator.GetWheelCollider("frontleft"), brakeinput * brakeTorque * 0.35f);
                mediator.SetBrakeTorque(mediator.GetWheelCollider("rearleft"), /*(decelerationFactor / 2f)*/ + brakeinput * brakeTorque * 0.15f);
                mediator.SetBrakeTorque(mediator.GetWheelCollider("rearright"),/* (decelerationFactor / 2f)*/ + brakeinput * brakeTorque * 0.15f);
            }
            else
            {
                // Apply front and rear brake torques, including controlledBrakeTorque
                mediator.SetBrakeTorque(mediator.GetWheelCollider("frontright"), brakeinput * brakeTorque * 0.35f);
                mediator.SetBrakeTorque(mediator.GetWheelCollider("frontleft"), brakeinput * brakeTorque * 0.35f);
                mediator.SetBrakeTorque(mediator.GetWheelCollider("rearleft"), brakeinput * brakeTorque * 0.15f);
                mediator.SetBrakeTorque(mediator.GetWheelCollider("rearright"), brakeinput * brakeTorque * 0.15f);
            }
        }

        if (rearrot != 0)
        {
            // Update friction based on the slip angle
            WheelFrictionCurve rearLeftFriction = mediator.GetWheelCollider("rearleft").sidewaysFriction;
            rearLeftFriction.stiffness = rearsidewaysfriction;
            mediator.UpdateFriction(mediator.GetWheelCollider("rearleft"), rearLeftFriction);

            WheelFrictionCurve rearRightFriction = mediator.GetWheelCollider("rearright").sidewaysFriction;
            rearRightFriction.stiffness = rearsidewaysfriction;
            mediator.UpdateFriction(mediator.GetWheelCollider("rearright"), rearRightFriction);
        }
    }

    void steering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speedKMH);

        if (Input.GetAxis("Horizontal") != 0)
        {
            mediator.GetWheelCollider("frontleft").steerAngle = steeringAngle;
            mediator.GetWheelCollider("frontright").steerAngle = steeringAngle;
        }
        else
        {
            mediator.GetWheelCollider("frontleft").steerAngle = 0;
            mediator.GetWheelCollider("frontright").steerAngle = 0;
        }
    }

    float TractionControl(float currTorque)
    {
        float threshold = 2f;
        float tractionCutoff = carMediator.GetTractionCutoff(); // Access via mediator
        float maxTorque = carMediator.GetMaxTorque(); // Access via mediator

        if ((Mathf.Abs(rearrot) > Mathf.Abs(threshold * frontrot)) && !istractioncontrol)
        {
            currTorque *= tractionCutoff;
            Debug.Log("traction control yes");
            istractioncontrol = true;
        }
        if ((istractioncontrol) && (Mathf.Abs(rearrot) <= Mathf.Abs(frontrot + (frontrot / 10f))))
        {
            currTorque = maxTorque;
            istractioncontrol = false;
        }

        return currTorque;
    }

    public void enginesound(float motor)
    {
        AudioSource engineAudioSource = mediator.GetEngineAudioSource(); // Get the AudioSource through the mediator
        float rearrot = Mathf.Abs(mediator.GetWheelCollider("rearleft").rpm); // Access the rearleft wheel's rpm through the mediator

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
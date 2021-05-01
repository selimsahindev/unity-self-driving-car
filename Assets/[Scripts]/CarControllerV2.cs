using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarControllerV2 : MonoBehaviour {
    public const float LANE_WIDTH = 4.38f;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    [SerializeField] private bool autonomousDrivingMode = false;
    [SerializeField] private bool speedLimit = true;
    [SerializeField] private float maxSpeedKMH = 80f;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private Transform centerOfMass;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider; 

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate() {
        if (!autonomousDrivingMode)
            GetInput();

        HandleMotor();
        HandleSteering();
        UpdateWheels();

        //Debug.Log(GetVelocityKMH());
    }

    public void Reset() {
        rb.velocity = Vector3.zero;
        frontLeftWheelCollider.motorTorque = 0f;
        frontRightWheelCollider.motorTorque = 0f;
        rearLeftWheelCollider.motorTorque = 0f;
        rearRightWheelCollider.motorTorque = 0f;
    }

    public void Drive(float throttle, float steering) {
        horizontalInput = steering;
        verticalInput = (rb.velocity.magnitude >= 20f) ? 0f : throttle;
    }

    private void GetInput() {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor() {
        float torque;
        
        if (!speedLimit || (GetVelocityKMH() < maxSpeedKMH))
            torque = verticalInput * motorForce;
        else
            torque = 0f;

        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking() {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering() {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels() {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform) {
        Vector3 pos;
        Quaternion rot; wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private float GetVelocityKMH() {
        // Converting m/s to km/h (1 m/s == 3.6km/h)
        return ((rb.velocity.magnitude * LANE_WIDTH * 3.6f) / 5f);
    }
}

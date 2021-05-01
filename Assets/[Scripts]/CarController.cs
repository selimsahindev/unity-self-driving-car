using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour {
    [Range(-1, 1)]
    public float steering = 0f;
    public float motorTorque = 100f;
    public float maxSteeringAngle = 30f;
    public float handbrakeDriftFactor = 2f;

    // NOT RELATED TO CAR
    public Text handbrakeIndicator;

    public Transform wheelFrontLeft;
    public Transform wheelFrontRight;
    public Transform wheelRearLeft;
    public Transform wheelRearRight;

    public WheelCollider wheelColliderFrontLeft;
    public WheelCollider wheelColliderFrontRight;
    public WheelCollider wheelColliderRearLeft;
    public WheelCollider wheelColliderRearRight;

    private bool handbrakeIsActive = false;
    private float rearSidewaysExtremumSlip;

    private void Start() {
        // Same for both rear wheel colliders
        rearSidewaysExtremumSlip = wheelColliderRearLeft.sidewaysFriction.extremumSlip;

        handbrakeIndicator.enabled = false;
    }

    private void Update() {
        RotateWheels();
    }

    private void FixedUpdate() {
        HandleInputs();
    }

    private void HandleInputs() {
        float handbrake = Input.GetAxis("Jump");

        if (Input.GetKey(KeyCode.Space))
            handbrakeIsActive = true;
        else
            handbrakeIsActive = false;

        handbrakeIndicator.enabled = handbrakeIsActive;

        wheelColliderRearLeft.motorTorque = Input.GetAxis("Vertical") * motorTorque * (handbrakeIsActive ? 0f : 1f);
        wheelColliderRearRight.motorTorque = Input.GetAxis("Vertical") * motorTorque * (handbrakeIsActive ? 0f : 1f);

        wheelColliderFrontLeft.steerAngle = Input.GetAxis("Horizontal") * maxSteeringAngle;
        wheelColliderFrontRight.steerAngle = Input.GetAxis("Horizontal") * maxSteeringAngle;

        // Reduce the friction of the rear wheels as we pull the handbrake
        WheelFrictionCurve sideFrictionRL = wheelColliderRearLeft.sidewaysFriction;
        WheelFrictionCurve sideFrictionRR = wheelColliderRearRight.sidewaysFriction;

        sideFrictionRL.extremumSlip = rearSidewaysExtremumSlip + handbrake * handbrakeDriftFactor;
        wheelColliderRearLeft.sidewaysFriction = sideFrictionRL;
        sideFrictionRR.extremumSlip = rearSidewaysExtremumSlip + handbrake * handbrakeDriftFactor;
        wheelColliderRearRight.sidewaysFriction = sideFrictionRR;
    }

    private void RotateWheels() {
        Vector3 wheelPos = Vector3.zero;
        Quaternion wheelRot = Quaternion.identity;

        wheelColliderFrontLeft.GetWorldPose(out wheelPos, out wheelRot);
        wheelFrontLeft.rotation = wheelRot;

        wheelColliderFrontRight.GetWorldPose(out wheelPos, out wheelRot);
        wheelFrontRight.rotation = wheelRot;

        if (!handbrakeIsActive) {
            wheelColliderRearLeft.GetWorldPose(out wheelPos, out wheelRot);
            wheelRearLeft.rotation = wheelRot;

            wheelColliderRearRight.GetWorldPose(out wheelPos, out wheelRot);
            wheelRearRight.rotation = wheelRot;
        }
    }
}

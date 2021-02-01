using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Private Variables
    [SerializeField] float speed;
    [SerializeField] float rpm;
    const float turnSpeed = 45;
    float horizontalInput;
    float verticalInput;
    float steeringAngle;
    [SerializeField] float maxSteerAngle = 30;
    [SerializeField] float motorForce = 50;
    Rigidbody playerRb;

    [SerializeField] GameObject centerOfMass;
    [SerializeField] TextMeshProUGUI speedometerText;
    [SerializeField] TextMeshProUGUI rpmText;
    [SerializeField] List<WheelCollider> allWheels;
    [SerializeField] int wheelsOnGround;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = centerOfMass.transform.position;
    }

    private void Update()
    {
        GetInput();

        speedometerText.SetText("Speed: " + speed + " km/h");

        rpm = (speed % 30) * 40;
        rpmText.SetText("RPM: " + rpm);
    }


    void FixedUpdate()
    {
        // Move vehicle forward and backward
        if (IsOnGround())
        {
            Steer();
            Accelerate();
            UpdateWheelPoses();
            
            speed = Mathf.RoundToInt(playerRb.velocity.magnitude * 3.6f); // mtp by 3.6f for kmph speed
        }
    }
    private void GetInput()
    {
        // Getting player input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        steeringAngle = maxSteerAngle * horizontalInput;
        frontDriverW.steerAngle = steeringAngle;
        frontPassengerW.steerAngle = steeringAngle;
    }

    private void Accelerate()
    {
        frontDriverW.motorTorque = verticalInput * motorForce;
        frontPassengerW.motorTorque = verticalInput * motorForce;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    bool IsOnGround()
    {
        wheelsOnGround = 0;
        foreach (WheelCollider wheel in allWheels)
        {
            if (wheel.isGrounded)
            {
                wheelsOnGround++;
            }
        }

        if (wheelsOnGround == allWheels.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

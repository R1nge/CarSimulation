using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    public class WheelsController : MonoBehaviour
    {
        [SerializeField] private float accelerationTime;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float distanceToGround;
        [SerializeField] private float motorForce;
        [SerializeField] private float maxSteerAngle;
        [SerializeField] private Transform[] frontWheels, backWheels;
        [SerializeField] private WheelCollider[] frontWheelsColliders, backWheelColliders;
        private float _horizontalInput;
        private float _verticalInput;
        private float _steeringAngle;
        private float _currentTime;
        private float _speedMultiplier = 1f;
        private Rigidbody _rigidbody;

        public void ApplySpeedPowerup(float percent)
        {
            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                var wheel = backWheelColliders[i];
                if (GetCurrentSpeed(wheelRadius: wheel.radius, wheelRpm: wheel.rpm) >= maxSpeed) return;
                _speedMultiplier = GetCurrentSpeed(wheelRadius: wheel.radius, wheelRpm: wheel.rpm) * percent;
                Debug.LogWarning("Speed: " + GetCurrentSpeed(wheel.radius, wheel.rpm));
                AddAcceleration();
            }
        }

        public void DeleteSpeedPowerup()
        {
            _speedMultiplier = 1;
        }

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void Update()
        {
            GetInput();
        }

        private void FixedUpdate()
        {
            Steer();
            Accelerate();
            UpdateWheelPoses();
            IncreaseTime();
            DownForce();
        }
        //TODO: add force to counter side 
        //https://youtu.be/ueEmiDM94IE?t=1225
        //TODO: add acceleration

        private void GetInput()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");
        }

        private void Steer()
        {
            _steeringAngle = maxSteerAngle * _horizontalInput;

            for (int i = 0; i < frontWheels.Length; i++)
            {
                frontWheelsColliders[i].steerAngle = _steeringAngle;
            }
        }

        private void Accelerate()
        {
            //var acceleration = maxSpeed / accelerationTime;
            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                var wheel = backWheelColliders[i];
                if (GetCurrentSpeed(wheelRadius: wheel.radius, wheelRpm: wheel.rpm) >= maxSpeed) return;
                backWheelColliders[i].motorTorque =
                    _verticalInput * motorForce * _speedMultiplier; // * acceleration;
            }
        }

        private void AddAcceleration()
        {
            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                var wheel = backWheelColliders[i];
                if (GetCurrentSpeed(wheelRadius: wheel.radius, wheelRpm: wheel.rpm) >= maxSpeed) return;
                backWheelColliders[i].motorTorque = motorForce * _speedMultiplier; // * acceleration;
            }
        }

        private void IncreaseTime()
        {
            if (_rigidbody.velocity != Vector3.zero)
            {
                _currentTime += Time.fixedDeltaTime;
            }
            else
            {
                _currentTime = 0;
            }
        }

        private void UpdateWheelPoses()
        {
            for (int i = 0; i < frontWheels.Length; i++)
            {
                UpdateWheelPose(frontWheels[i], frontWheelsColliders[i]);
            }

            for (int i = 0; i < backWheels.Length; i++)
            {
                UpdateWheelPose(backWheels[i], backWheelColliders[i]);
            }
        }

        private void UpdateWheelPose(Transform transform, WheelCollider collider)
        {
            collider.GetWorldPose(out var pos, out var quat);
            transform.position = pos;
            transform.rotation = quat;
        }

        private void DownForce()
        {
            if (!IsGrounded()) return;
            _rigidbody.AddForce(Vector3.down * 25f);
        }

        /// <summary>
        /// Return current speed in km/h
        /// </summary>
        private float GetCurrentSpeed(float wheelRadius, float wheelRpm)
        {
            // var circumFerence = 2.0f * 3.14f * wheelRadius;
            // return circumFerence * wheelRpm * 60 / 1000;
            return _rigidbody.velocity.magnitude * 3.6f;
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + new Vector3(0, .1f, 0), -Vector3.up, distanceToGround);
        }
    }
}
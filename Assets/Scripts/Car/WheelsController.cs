using UnityEngine;

namespace Car
{
    public class WheelsController : MonoBehaviour
    {
        [SerializeField] private float accelerationTime;
        [SerializeField] private float maxSpeed, maxBackSpeed;
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
            _speedMultiplier = GetCurrentSpeed() * percent;

            if (_speedMultiplier > 0)
            {
                AddAcceleration();
            }
            else if (_speedMultiplier < 0)
            {
                SetTorque(motorForce * _speedMultiplier);
                SetBreakTorque(motorForce * _speedMultiplier);
            }
        }

        public void DeleteSpeedPowerup()
        {
            SetTorque(0);
            SetBreakTorque(0);
            _speedMultiplier = 1;
        }

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void Update()
        {
            GetInput();
            Brake();
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
            if (_verticalInput > 0)
            {
                if (GetCurrentSpeed() <= maxSpeed)
                {
                    SetTorque(_verticalInput * motorForce * _speedMultiplier);
                }
            }
            else if (_verticalInput < 0)
            {
                if (GetCurrentSpeed() <= maxBackSpeed)
                {
                    SetTorque(_verticalInput * motorForce * _speedMultiplier);
                }
            }
        }

        private void SetTorque(float value)
        {
            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                var wheel = backWheelColliders[i];
                wheel.motorTorque = value;
            }
        }

        private void Brake()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (GetCurrentSpeed() != 0)
                {
                    SetBreakTorque(motorForce * _speedMultiplier);
                }
                else
                {
                    SetBreakTorque(0);
                }

                SetTorque(0);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                SetBreakTorque(0);
            }
        }

        private void AddAcceleration()
        {
            if (GetCurrentSpeed() >= maxSpeed) return;
            SetTorque(motorForce * _speedMultiplier);
        }

        private void SetBreakTorque(float value)
        {
            if (value < 0)
            {
                value *= -1;
            }

            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                var wheel = backWheelColliders[i];
                wheel.brakeTorque = value;
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
        private float GetCurrentSpeed() => _rigidbody.velocity.magnitude * 3.6f;

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + new Vector3(0, .1f, 0), -Vector3.up, distanceToGround);
        }
    }
}
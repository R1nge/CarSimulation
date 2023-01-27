using UnityEngine;

namespace Car
{
    public class WheelsController : MonoBehaviour
    {
        [SerializeField] private Transform centerOfMass;
        [SerializeField] private float accelerationTime, timeToStop;
        [SerializeField] private float maxSpeed, maxBackSpeed;
        [SerializeField] private float distanceToGround;
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
            _speedMultiplier = GetCurrentSpeedKmh() * percent;

            if (_speedMultiplier > 0)
            {
                if (GetCurrentSpeedKmh() >= maxSpeed) return;
                SetTorque(CalculateAcceleration(maxSpeed) * _speedMultiplier);
            }
            else if (_speedMultiplier < 0)
            {
                SetTorque(CalculateAcceleration(maxSpeed) * _speedMultiplier);
            }
        }

        public void DeleteSpeedPowerup()
        {
            SetTorque(0);

            _speedMultiplier = 1;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.centerOfMass = centerOfMass.localPosition;
        }

        private void Update()
        {
            GetInput();
            Brake();
            Debug.LogWarning(GetCurrentSpeedKmh());
        }

        private void FixedUpdate()
        {
            Steer();
            Accelerate();
            UpdateWheelPoses();
            IncreaseTime();
            //DownForce();
            //print(GetCurrentSpeedKmh());
        }

        private float CalculateAcceleration(float max)
        {
            var current = GetCurrentSpeedMs();
            var accelerationRate = (max - current) / accelerationTime;
            print(accelerationRate);
            return accelerationRate * _rigidbody.mass;
        }

        private float CalculateBreakTorque()
        {
            var momentum = _rigidbody.mass * maxSpeed * 3600 * 1000; //GetCurrentSpeedMs();
            var torque = momentum / timeToStop;
            var force = torque * backWheelColliders[0].forwardFriction.stiffness * 2;
            return force * 2;
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
                if (GetCurrentSpeedKmh() <= maxSpeed)
                {
                    SetTorque(_verticalInput * CalculateAcceleration(maxSpeed) * _speedMultiplier);
                }
            }
            else if (_verticalInput < 0)
            {
                if (GetCurrentSpeedKmh() <= maxBackSpeed)
                {
                    SetTorque(_verticalInput * CalculateAcceleration(maxBackSpeed) * _speedMultiplier);
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
            if (Input.GetKey(KeyCode.Space))
            {
                if (GetCurrentSpeedKmh() >= 0.1f)
                {
                    SetDumpingRate(CalculateBreakTorque());
                }
                else
                {
                    SetDumpingRate(3f);
                }

                SetTorque(0);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                SetDumpingRate(3f);
            }
        }

        private void SetDumpingRate(float value)
        {
            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                backWheelColliders[i].wheelDampingRate = value;
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

        private float GetCurrentSpeedKmh() => _rigidbody.velocity.magnitude * 3.6f;

        private float GetCurrentSpeedMs() => _rigidbody.velocity.magnitude;

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + new Vector3(0, .1f, 0), -Vector3.up, distanceToGround);
        }
    }
}
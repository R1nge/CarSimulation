using UnityEngine;

namespace Car
{
    public class WheelsController : MonoBehaviour
    {
        [SerializeField] private float motorForce = 50;
        [SerializeField] private float maxSteerAngle = 30;
        [SerializeField] private Transform[] frontWheels, backWheels;
        [SerializeField] private WheelCollider[] frontWheelsColliders, backWheelColliders;
        private float _horizontalInput;
        private float _verticalInput;
        private float _steeringAngle;


        private void FixedUpdate()
        {
            GetInput();
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }

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
            for (int i = 0; i < backWheelColliders.Length; i++)
            {
                backWheelColliders[i].motorTorque = _verticalInput * motorForce;
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
                UpdateWheelPose(frontWheels[i], frontWheelsColliders[i]);
            }
        }

        private void UpdateWheelPose(Transform transform, WheelCollider collider)
        {
            collider.GetWorldPose(out var pos, out var quat);
            transform.position = pos;
            transform.rotation = quat;
        }
    }
}
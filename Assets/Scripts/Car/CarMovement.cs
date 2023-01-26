using UnityEngine;

namespace Car
{
    public class CarMovement : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float accelerationTime;
        
        private WheelsController _wheelsController;
        private Vector3 _moveDirection;
        private float _time;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _wheelsController = GetComponent<WheelsController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void IncreaseTime()
        {
            if (_rigidbody.velocity.magnitude == 0 && _time >= .1f)
            {
                ResetTime();
            }
            else
            {
                _time += Time.fixedDeltaTime;
            }
        }

        private void Rotate(Vector3 dir)
        {
        }

        

        private void ResetTime()
        {
            _time = 0;
        }
    }
}
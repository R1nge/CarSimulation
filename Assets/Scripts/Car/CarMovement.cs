using System;
using UnityEngine;

namespace Car
{
    public class CarMovement : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float accelerationTime;
        [SerializeField] private float distanceToGround;
        private Vector3 _moveDirection;
        private float _time;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (IsGrounded())
            {
                GetInput();
            }
            else
            {
                ResetTime();
            }

            IncreaseTime();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void GetInput()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _moveDirection = transform.forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _moveDirection = -transform.forward;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Rotate(-transform.right);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Rotate(transform.right);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                Brake();
            }
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

        private void Brake()
        {
            _rigidbody.AddForce(-_moveDirection * LerpSpeed(_time / accelerationTime, 0));
        }

        private void Move()
        {
            if (_rigidbody.velocity.magnitude >= maxSpeed) return;
            _rigidbody.AddForce(_moveDirection * LerpSpeed(0, maxSpeed));
        }

        private float LerpSpeed(float from, float to) => Mathf.Lerp(from, to, _time / accelerationTime);

        private void Rotate(Vector3 dir)
        {
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + new Vector3(0, .1f, 0), -Vector3.up, distanceToGround);
        }

        private void ResetTime()
        {
            _time = 0;
        }
    }
}
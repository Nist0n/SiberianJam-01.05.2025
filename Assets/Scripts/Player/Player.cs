using System.Collections;
using Settings.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 5;
        [SerializeField] private float mouseSensitivity = 10;
        [SerializeField] private float jumpForce = 5;
        [SerializeField] private float sprintMultiplier = 2;
        // [SerializeField] private AudioSource steps;
        // [SerializeField] private AudioSource run;

        [SerializeField] private float distanceToGround = 1.1f;
        
        private const float Gravity = -9.81f;

        private CharacterController _characterController;
        private Vector3 _velocity;
        private Transform _cameraTransform;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;

        private float _rotationX;
        private float _moveSpeed;

        private bool _grounded = true;
        
        private void Start()
        {
            if (PlayerPrefs.HasKey("Sensitivity"))
            {
                mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");
            }
            

            _characterController = GetComponent<CharacterController>();

            _cameraTransform = Camera.main.transform;

            _moveAction = InputSystem.actions.FindAction("Move");
            _lookAction = InputSystem.actions.FindAction("Look");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _sprintAction = InputSystem.actions.FindAction("Sprint");
        }

        private void Update()
        {
            Look();
            Move();
        }

        private void Look()
        {
            Vector2 lookValue = _lookAction.ReadValue<Vector2>() * (mouseSensitivity * Time.deltaTime);

            _rotationX -= lookValue.y;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            _cameraTransform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

            transform.Rotate(Vector3.up, lookValue.x);
        }

        private void Move()
        {
            Vector2 moveValue = _moveAction.ReadValue<Vector2>();
            if (_sprintAction.IsPressed() && moveValue is { y: > 0, x: 0 })
            {
                _moveSpeed = sprintMultiplier * walkSpeed;
                // run.enabled = true;
                // steps.enabled = false;
            }
            else
            {
                _moveSpeed = walkSpeed;
                // run.enabled = false;
                // steps.enabled = true;
            }

            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            if (moveValue is { x: 0, y: 0 })
            {
                // run.enabled = false;
                // steps.enabled = false;
            }

            Vector3 move = transform.right * moveValue.x + transform.forward * moveValue.y;

            _characterController.Move(move * (_moveSpeed * Time.deltaTime));

            _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround);

            if (_jumpAction.IsPressed() && _grounded)
            {
                _grounded = false;
                // AudioManager.instance.PlaySfx("PlayerJump");
                _velocity.y = jumpForce;
            }

            _velocity.y += Gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}
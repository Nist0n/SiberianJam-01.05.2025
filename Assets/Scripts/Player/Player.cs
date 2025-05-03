using System;
using System.Collections;
using Environment;
using Settings.Audio;
using Static_Classes;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        [SerializeField] private float interactDistance = 1f;

        private FaderExample _fader;
        
        private const float Gravity = -9.81f;

        private CharacterController _characterController;
        private Vector3 _velocity;
        private Transform _cameraTransform;
        private InteractiveObject _currentTarget;

        #region InputActions

            private InputAction _moveAction;
            private InputAction _lookAction;
            private InputAction _jumpAction;
            private InputAction _sprintAction;
            private InputAction _interactAction;
            private InputAction _closeAction;
                
        #endregion
        
        private float _rotationX;
        private float _moveSpeed;

        private bool _grounded = true;
        private bool _isInteracting;
        
        private Camera _mainCamera;

        private bool _canInteractWithChest = true;
        
        private void OnEnable()
        {
            GameEvents.ChestOpened += ChestOpened;
        }

        private void OnDisable()
        {
            GameEvents.ChestOpened -= ChestOpened;
        }

        private void Start()
        {
            _fader = FindAnyObjectByType<FaderExample>();
            
            if (PlayerPrefs.HasKey("Sensitivity"))
            {
                mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");
            }
            
            _characterController = GetComponent<CharacterController>();

            _mainCamera = Camera.main;
            _cameraTransform = _mainCamera.transform;

            _moveAction = InputSystem.actions.FindAction("Move");
            _lookAction = InputSystem.actions.FindAction("Look");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _sprintAction = InputSystem.actions.FindAction("Sprint");
            _interactAction = InputSystem.actions.FindAction("Interact");
            _closeAction = InputSystem.actions.FindAction("Close");
        }

        private void Update()
        {
            ControlInput();
            
            if (_isInteracting)
            {
                return;
            }
            
            Look();
            Move();
        }

        private void FixedUpdate()
        {
            if (!_isInteracting)
            {
                FindClosestInteractable();
                UpdateHintUI();
            }
        }

        private void ChestOpened()
        {
            _canInteractWithChest = false;
            StartCoroutine(StopInteracting());
        }
        
        private void ControlInput()
        {
            if (_interactAction.WasPressedThisFrame())
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = _mainCamera.ScreenPointToRay(screenCenter);
                if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
                {
                    GameObject hitGameObject = hit.collider.gameObject;
                    if (hitGameObject.CompareTag("Chest"))
                    {
                        if (!_canInteractWithChest)
                        {
                            return;
                        }
                        
                        Chest chest = hitGameObject.GetComponent<Chest>();
                        _isInteracting = chest.InteractWithLock();
                    }

                    if (hitGameObject.CompareTag("Car"))
                    {
                        // play sound, cutscene
                        if (SceneManager.GetActiveScene().name.Equals("Home"))
                        {
                            _fader.LoadScene("Island");
                        }
                        else
                        {
                            if (!_canInteractWithChest)
                            {
                                _fader.LoadScene("Runner");
                            }
                            else
                            {
                                // проиграть голосовуху
                            }
                        }
                    }
                }
            }

            if (_closeAction.WasPressedThisFrame())
            {
                if (_isInteracting)
                {
                    Chest chest = FindAnyObjectByType<Chest>();
                    chest.CloseLock();
                    _isInteracting = false;
                }
            }
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
        
        private void FindClosestInteractable()
        {
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (_currentTarget)
                {
                    return;
                }
                
                InteractiveObject obj = hit.collider.gameObject.GetComponent<InteractiveObject>();
                if (obj)
                {
                    _currentTarget = obj;
                    return;
                }
            }

            if (_currentTarget)
            {
                _currentTarget.HideNotification();
            }
            _currentTarget = null;
        }

        private void UpdateHintUI()
        {
            Debug.Log("UpdateHintUI");
            if (_currentTarget)
            {
                Debug.Log("show notification");
                _currentTarget.ShowNotification();
            }
        }

        private IEnumerator StopInteracting()
        {
            yield return new WaitForSeconds(2f);
            _isInteracting = false;
        }
    }
}
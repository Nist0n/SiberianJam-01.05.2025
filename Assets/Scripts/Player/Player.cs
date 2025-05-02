using Environment;
using Static_Classes;
using TMPro;
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
        [SerializeField] private float interactDistance = 1f;

        [SerializeField] private GameObject inputLock;
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TMP_Text hintText;
        
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
        private bool _isInteracting = false;
        
        private Camera _mainCamera;

        private bool _canInteractWithChest = true;

        private bool _cursorShown;
        
        private void OnEnable()
        {
            GameEvents.ChestOpened += ChestOpened;
            GameEvents.ActivateCursor += CursorStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.ChestOpened -= ChestOpened;
            GameEvents.ActivateCursor -= CursorStateChanged;
        }

        private void Start()
        {
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
            ControlCursor();
            Interact();
            if (_canInteractWithChest && !_isInteracting)
            {
                FindClosestInteractable();
                UpdateHintUI();
            }
            else
            {
                hintPanel.SetActive(false);
            }
            
            if (inputLock.activeInHierarchy)
            {
                return;
            }
            
            Look();
            Move();
        }

        private void ChestOpened()
        {
            _canInteractWithChest = false;
        }

        private void ControlCursor()
        {
            if (_cursorShown)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void CursorStateChanged(bool activated)
        {
            _cursorShown = activated;
        }
        
        private void Interact()
        {
            if (!_canInteractWithChest)
            {
                return;
            }
            
            if (_interactAction.WasPressedThisFrame())
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = _mainCamera.ScreenPointToRay(screenCenter);
                if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
                {
                    if (hit.collider.gameObject.CompareTag("Chest"))
                    {
                        inputLock.SetActive(!inputLock.activeSelf);
                        GameEvents.ActivateCursor?.Invoke(inputLock.activeSelf);
                        _isInteracting = inputLock.activeSelf;
                    }
                }
            }

            if (_closeAction.WasPressedThisFrame())
            {
                if (Time.timeScale == 0)
                {
                    return;
                }
                
                GameEvents.ActivateCursor?.Invoke(false);
                inputLock.SetActive(false);
                _isInteracting = false;
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
                InteractiveObject obj = hit.collider.GetComponent<InteractiveObject>();
                if (obj)
                {
                    _currentTarget = obj;
                    return;
                }
            }

            _currentTarget = null;
        }

        private void UpdateHintUI()
        {
            if (_currentTarget)
            {
                hintPanel.SetActive(true);
                hintText.text = _currentTarget.InteractionText;
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(_currentTarget.transform.position);
                hintPanel.transform.position = screenPos + new Vector3(0, 50, 0);
            }
            else
            {
                hintPanel.SetActive(false);
            }
        }
    }
}
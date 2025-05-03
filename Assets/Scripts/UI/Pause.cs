using System;
using Static_Classes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private GameObject pauseUI;
        
        private InputAction _pauseAction;

        private FaderExample _fader;

        private bool _isInteracting;

        private static Pause _instance;

        private bool _cursorShown;
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            GameEvents.Interacting += Interacting;
            GameEvents.ActivateCursor += CursorStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.Interacting -= Interacting;
            GameEvents.ActivateCursor -= CursorStateChanged;
        }

        private void ControlCursor()
        {
            if (SceneManager.GetActiveScene().name.Equals("Duck Hunt"))
            {
                return;
            }
            
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
        
        private void Interacting(bool obj)
        {
            _isInteracting = obj;
        }

        private void Start()
        {
            continueButton.onClick.AddListener(Continue);
            exitButton.onClick.AddListener(ExitToMenu);

            _pauseAction = InputSystem.actions.FindAction("Close");
            _fader = FindAnyObjectByType<FaderExample>();
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                return;
            }
            
            ControlCursor();
            if (_pauseAction.triggered)
            {
                if (_isInteracting)
                {
                    return;
                }
                
                TogglePause(!pauseUI.activeInHierarchy);
            }
        }

        private void TogglePause(bool paused)
        {
            Time.timeScale = paused ? 0 : 1;
            GameEvents.ActivateCursor?.Invoke(paused);
            pauseUI.SetActive(paused);
        }
        
        private void Continue()
        {
            Debug.Log("Continue");
            TogglePause(false);
        }

        private void ExitToMenu()
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
            _fader.LoadScene("MainMenu");
        }
    }
}
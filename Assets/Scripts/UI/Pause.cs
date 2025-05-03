using System;
using Static_Classes;
using UnityEngine;
using UnityEngine.InputSystem;
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
        }

        private void OnDisable()
        {
            GameEvents.Interacting -= Interacting;
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
            if (_pauseAction.triggered)
            {
                if (_isInteracting)
                {
                    Debug.Log("interacting");
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
            _fader.LoadScene("MainMenu");
        }
    }
}
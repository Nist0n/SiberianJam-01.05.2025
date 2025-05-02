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
        [SerializeField] private GameObject lockUI;
        
        private InputAction _pauseAction;

        [SerializeField] private FaderExample fader;
        
        private void Start()
        {
            continueButton.onClick.AddListener(Continue);
            exitButton.onClick.AddListener(ExitToMenu);

            _pauseAction = InputSystem.actions.FindAction("Close");
        }

        private void Update()
        {
            if (_pauseAction.triggered)
            {
                if (lockUI.activeInHierarchy)
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
            fader.LoadScene("MainMenu");
        }
    }
}
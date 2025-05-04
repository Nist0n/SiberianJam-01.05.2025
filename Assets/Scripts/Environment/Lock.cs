using System;
using System.Collections;
using System.Collections.Generic;
using Settings.Audio;
using Static_Classes;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Environment
{
    public class Lock : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lockInput;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Button submitButton;
        
        private InputAction _enterKeyAction;

        private string _secretCode;

        private bool _canInput = true;
        
        private AcornSpawner _acornSpawner;
        
        private void Start()
        {
            _enterKeyAction = InputSystem.actions.FindAction("DigitInput");
            _acornSpawner = FindAnyObjectByType<AcornSpawner>();
            List<int> acornCounts = _acornSpawner.acornCounts;
            _secretCode = $"{acornCounts[0]}{acornCounts[1]}{acornCounts[2]}{acornCounts[3]}";
            Debug.Log(_secretCode);
        }

        private void Update()
        {
            if (!_canInput)
            {
                return;
            }

            submitButton.interactable = lockInput.text.Length == 4;
            
            if (_enterKeyAction.WasPressedThisFrame())
            {
                AudioManager.instance.PlaySfx("Print the password");
                InputControl inputControl = _enterKeyAction.activeControl;
                string character = inputControl.displayName;
                Debug.Log(character);
                if ("1234567890".Contains(character))
                {
                    if (lockInput.text.Length < 4)
                    {
                        lockInput.text += character;
                    }
                }
                else // backspace
                {
                    if (lockInput.text.Length > 0)
                    {
                        lockInput.text = lockInput.text.Remove(lockInput.text.Length - 1);
                    }
                }
            }
        }

        public void OnKeyEnter()
        {
            bool result = lockInput.text.Equals(_secretCode);
            if (result)
            {
                GameEvents.ChestOpened?.Invoke();
                AudioManager.instance.PlaySfx("Correct password");
                StartCoroutine(OnLockOpened());
                Debug.Log("Key is right!");
            }

            StartCoroutine(SetResultText(result));
        }

        private IEnumerator OnLockOpened()
        {
            _canInput = false;
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
            GameEvents.ActivateCursor?.Invoke(false);
            // катсцена или анимация
        }
        
        private IEnumerator SetResultText(bool result)
        {
            if (result)
            {
                resultText.text = "Верно!";
                GameEvents.Interacting?.Invoke(false);
            }
            else
            {
                resultText.text = "Неверно!";
                AudioManager.instance.PlaySfx("Incorrect password");
                AudioManager.instance.PlayRandomVoiceLine(0);
            }
            
            yield return new WaitForSeconds(2f);
            resultText.text = "";
        }
    }
}
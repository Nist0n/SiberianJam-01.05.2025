using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Environment
{
    public class Lock : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lockInput;

        private InputAction _enterKeyAction;

        private void Start()
        {
            _enterKeyAction = InputSystem.actions.FindAction("DigitInput");
            lockInput.onValueChanged.AddListener(delegate { OnKeyEnter(); });
        }

        private void Update()
        {
            if (_enterKeyAction.triggered)
            {
                InputControl inputControl = _enterKeyAction.activeControl;
                string character = inputControl.displayName;
                Debug.Log(character);
                if ("1234567890".Contains(character))
                {
                    lockInput.text += character;
                }
                else  // backspace
                {
                    if (lockInput.text.Length > 0)
                    {
                        lockInput.text = lockInput.text.Remove(lockInput.text.Length - 1);
                    }
                }
            }
        }

        private void OnKeyEnter()
        {
            if (lockInput.text == "1234")
            {
                Debug.Log("Key is right!");
            }
        }
    }
}
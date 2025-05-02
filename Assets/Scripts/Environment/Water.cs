using System;
using UI;
using UnityEngine;

namespace Environment
{
    public class Water : MonoBehaviour
    {
        private FaderExample _fader;

        private void Start()
        {
            _fader = FindAnyObjectByType<FaderExample>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered water " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                _fader.LoadScene("Island");
            }
        }
    }
}
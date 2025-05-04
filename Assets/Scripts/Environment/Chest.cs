using System;
using System.Collections;
using Static_Classes;
using UnityEngine;

namespace Environment
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private GameObject inputLock;
        [SerializeField] private GameObject rabbit;
        
        private Animator _animator;

        private void OnEnable()
        {
            GameEvents.ChestOpened += ChestOpened;
        }

        private void OnDisable()
        {
            GameEvents.ChestOpened -= ChestOpened;
        }

        private void ChestOpened()
        {
            StartCoroutine(AnimateChest());
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public bool InteractWithLock()
        {
            inputLock.SetActive(!inputLock.activeSelf);
            bool isInteracting = inputLock.activeSelf;
            GameEvents.ActivateCursor?.Invoke(inputLock.activeSelf);
            GameEvents.Interacting?.Invoke(isInteracting);

            return isInteracting;
        }

        public void CloseLock()
        {
            inputLock.SetActive(false);
            GameEvents.ActivateCursor?.Invoke(false);
            StartCoroutine(StopInteracting());
        }
        
        private IEnumerator AnimateChest()
        {
            yield return new WaitForSeconds(2f);
            _animator.Play("open");
            rabbit.SetActive(true);
            yield return new WaitForSeconds(15f);
            rabbit.SetActive(false);
        }

        private IEnumerator StopInteracting()
        {
            yield return null;
            GameEvents.Interacting?.Invoke(false);
        }
    }
}
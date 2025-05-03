using Static_Classes;
using UnityEngine;

namespace Environment
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private GameObject inputLock;
        
        public bool InteractWithLock()
        {
            inputLock.SetActive(!inputLock.activeSelf);
            bool isInteracting = inputLock.activeSelf;
            GameEvents.ActivateCursor?.Invoke(inputLock.activeSelf);
            
            // if (inputLock.activeInHierarchy)
            // {
            //     inputLock.SetActive(false);
            //     
            // }
            // else
            // {
            //     inputLock.SetActive(true);
            //     GameEvents.ActivateCursor?.Invoke(true); 
            //     isInteracting = true;
            // }

            return isInteracting;
        }

        public void CloseLock()
        {
            inputLock.SetActive(false);
            GameEvents.ActivateCursor?.Invoke(true);
        }
    }
}
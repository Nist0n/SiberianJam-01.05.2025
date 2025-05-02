using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private string interactionText = "Взаимодействовать";
        [SerializeField] private GameObject hintPrefab;
        [SerializeField] private Vector3 hintOffset = new Vector3(0, 3f, 0);

        private GameObject _hintInstance;

        private void Start()
        {
            if (hintPrefab != null)
            {
                _hintInstance = Instantiate(hintPrefab, transform);
                _hintInstance.transform.localPosition = hintOffset;
                _hintInstance.SetActive(false);
            
                TMP_Text text = _hintInstance.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = interactionText;
            }
        }

        public void ShowNotification()
        {
            _hintInstance.SetActive(true);
        }
        
        public void HideNotification()
        {
            _hintInstance.SetActive(false);
        }
    }
}
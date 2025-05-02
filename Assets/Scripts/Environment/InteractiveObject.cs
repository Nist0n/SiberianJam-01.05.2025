using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Environment
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private string interactionText = "Взаимодействовать"; // Текст подсказки
        
        public string InteractionText => interactionText;
    }
}
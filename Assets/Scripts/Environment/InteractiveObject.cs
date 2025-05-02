using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Environment
{
    public class InteractiveObject : MonoBehaviour
    {
        [Header("Notification Settings")]
        [SerializeField] private string interactionText = "Взаимодействовать";
        [SerializeField] private GameObject hintPrefab;
        [SerializeField] private Vector3 hintOffset = new Vector3(0, 3f, 0);
        [SerializeField] private float appearDuration = 0.5f;
        [SerializeField] private float disappearDuration = 0.3f;
        [SerializeField] private float bounceStrength = 0.2f;

        private GameObject _hintInstance;
        private Vector3 _originalScale;
        private Sequence _animationSequence;

        private void Start()
        {
            if (hintPrefab != null)
            {
                _hintInstance = Instantiate(hintPrefab, transform);
                _hintInstance.transform.localPosition = hintOffset;
                _originalScale = _hintInstance.transform.localScale;
                _hintInstance.transform.localScale = Vector3.zero;
                _hintInstance.SetActive(false);
            
                TMP_Text text = _hintInstance.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = interactionText;
            }
        }

        public void ShowNotification()
        {
            if (!_hintInstance || _hintInstance.activeSelf) return;
            
            _animationSequence?.Kill();

            _hintInstance.SetActive(true);
            
            _animationSequence = DOTween.Sequence()
                .Append(_hintInstance.transform.DOScale(_originalScale * 1.1f, appearDuration * 0.5f).SetEase(Ease.OutBack))
                .Append(_hintInstance.transform.DOScale(_originalScale * 0.95f, appearDuration * 0.2f))
                .Append(_hintInstance.transform.DOScale(_originalScale, appearDuration * 0.3f))
                .OnKill(() => _hintInstance.transform.localScale = _originalScale);
        }
        
        public void HideNotification()
        {
            if (!_hintInstance || !_hintInstance.activeSelf) return;
            
            _animationSequence?.Kill();
            
            _animationSequence = DOTween.Sequence()
                .Append(_hintInstance.transform.DOScale(_originalScale * (1f + bounceStrength), disappearDuration * 0.3f).SetEase(Ease.OutQuad))
                .Append(_hintInstance.transform.DOScale(Vector3.zero, disappearDuration * 0.7f).SetEase(Ease.InBack))
                .OnComplete(() => _hintInstance.SetActive(false));
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
        }
    }
}
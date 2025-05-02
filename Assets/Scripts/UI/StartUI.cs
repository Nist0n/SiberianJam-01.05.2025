using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class StartUI : MonoBehaviour
    {
        [SerializeField] private string interactionText = "Взаимодействовать";
        [SerializeField] private float appearDuration = 0.5f;
        [SerializeField] private float disappearDuration = 0.3f;
        [SerializeField] private float bounceStrength = 0.2f;
        [SerializeField] private GameObject hintInstance;
        
        private Vector3 _originalScale;
        private Sequence _animationSequence;
    
        private void Start()
        {
            _originalScale = hintInstance.transform.localScale;
            hintInstance.transform.localScale = Vector3.zero;
            TMP_Text text = hintInstance.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = interactionText;
            StartCoroutine(Show());
        }

        private IEnumerator Show()
        {
            yield return new WaitForSeconds(2f);
            ShowNotification();
            // yield return new WaitForSeconds(.);
            // HideNotification();
        }
    
        private void ShowNotification()
        {
            if (!hintInstance || hintInstance.activeSelf) return;
            
            _animationSequence?.Kill();

            hintInstance.SetActive(true);
            
            _animationSequence = DOTween.Sequence()
                .Append(hintInstance.transform.DOScale(_originalScale * 1.1f, appearDuration * 0.5f).SetEase(Ease.OutBack))
                .Append(hintInstance.transform.DOScale(_originalScale * 0.95f, appearDuration * 0.2f))
                .Append(hintInstance.transform.DOScale(_originalScale, appearDuration * 0.3f))
                .OnKill(() => hintInstance.transform.localScale = _originalScale);
        }
        
        private void HideNotification()
        {
            if (!hintInstance || !hintInstance.activeSelf) return;
            
            _animationSequence?.Kill();
            
            _animationSequence = DOTween.Sequence()
                .Append(hintInstance.transform.DOScale(_originalScale * (1f + bounceStrength), disappearDuration * 0.3f).SetEase(Ease.OutQuad))
                .Append(hintInstance.transform.DOScale(Vector3.zero, disappearDuration * 0.7f).SetEase(Ease.InBack))
                .OnComplete(() => hintInstance.SetActive(false));
        }
    }
}

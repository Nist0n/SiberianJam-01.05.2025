using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Settings.Audio;
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

        [SerializeField] private float timeBetweenPhrases = 5f;
        
        [SerializeField] private List<string> phrases;
        
        private Vector3 _originalScale;
        private Sequence _animationSequence;

        private TMP_Text _text;
    
        private void Start()
        {
            _originalScale = hintInstance.transform.localScale;
            hintInstance.transform.localScale = Vector3.zero;
            _text = hintInstance.GetComponentInChildren<TMP_Text>();
            if (_text != null) _text.text = interactionText;
            StartCoroutine(Show());
        }

        private IEnumerator Show()
        {
            yield return new WaitForSeconds(2f);
            ShowNotification();
            StartCoroutine(ShowSeriesOfText(phrases));
            yield return new WaitForSeconds(30.5f);
            HideNotification();
            AudioManager.instance.PlaySfx("Ну и дела");
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

        private IEnumerator ShowSeriesOfText(List<string> series)
        {
            foreach (var phrase in series)
            {
                _text.text = phrase;
                yield return new WaitForSeconds(timeBetweenPhrases);
            }
        }
    }
}

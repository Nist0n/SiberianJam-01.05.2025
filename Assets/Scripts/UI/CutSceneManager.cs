using System.Collections;
using System.Collections.Generic;
using Settings.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class CutSceneManager : MonoBehaviour
    {
        [System.Serializable]
        public class CutsceneSlide
        {
            public GameObject ImagePrefab;
            [TextArea(3, 10)]
            public string Text;
            public float Duration = 5f; 
            public float TextSpeed = 0.05f; 
            public AudioClip Clip;
        }

        public List<CutsceneSlide> Slides;
        public Transform ImageContainer;
        public TextMeshProUGUI DisplayText;
        public Button SkipButton;

        private bool _isPlaying;
        private Coroutine _cutsceneRoutine;
        private FaderExample _fader;
        private GameObject _currentImageInstance;

        void Start()
        {
            _fader = FindFirstObjectByType<FaderExample>();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SkipButton.onClick.AddListener(SkipCutscene);
            StartCutscene();
        }

        void StartCutscene()
        {
            _isPlaying = true;
            _cutsceneRoutine = StartCoroutine(PlayCutscene());
        }

        IEnumerator PlayCutscene()
        {
            foreach (var slide in Slides)
            {
                if (_currentImageInstance)
                {
                    Destroy(_currentImageInstance);
                }
                
                if (slide.ImagePrefab)
                {
                    _currentImageInstance = Instantiate(slide.ImagePrefab, ImageContainer);
                }

                DisplayText.text = "";

                if (slide.Clip)
                {
                    AudioManager.instance.PlaySfx(slide.Clip.name);
                }
                
                float timePerChar = slide.TextSpeed;
                float timeElapsed = 0;
                int visibleChars = 0;

                while (visibleChars < slide.Text.Length && _isPlaying)
                {
                    timeElapsed += Time.deltaTime;
                    visibleChars = Mathf.FloorToInt(timeElapsed / timePerChar);
                    visibleChars = Mathf.Clamp(visibleChars, 0, slide.Text.Length);
                    DisplayText.text = slide.Text.Substring(0, visibleChars);
                    yield return null;
                }
                
                float remainingTime = slide.Duration - (visibleChars * timePerChar);
                if (remainingTime > 0 && _isPlaying)
                    yield return new WaitForSeconds(remainingTime);
            }

            EndCutscene();
        }

        void SkipCutscene()
        {
            if (!_isPlaying) return;
            _isPlaying = false;
            StopCoroutine(_cutsceneRoutine);
            EndCutscene();
        }

        void EndCutscene()
        {
            if (_currentImageInstance)
            {
                Destroy(_currentImageInstance);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            AudioManager.instance.sfxSource.Stop();
            gameObject.SetActive(false);
            if (_fader)
            {
                _fader.LoadScene("Home");
            }
            else
            {
                SceneManager.LoadScene("Home");  
            }
        }
    }
}
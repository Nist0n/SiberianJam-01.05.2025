using Settings.Audio;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        [SerializeField] private GameObject controlButtons;
        [SerializeField] private GameObject settingsUI;
        [SerializeField] private GameObject cutSceneUI;
        [SerializeField] private GameObject mainMenu;

        private FaderExample _fader;
        
        private void Awake()
        {
            playButton.onClick.AddListener(PlayGame);
            settingsButton.onClick.AddListener(ToggleSettings);
            exitButton.onClick.AddListener(ExitGame);
            backButton.onClick.AddListener(ToggleSettings);
        }

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(PlayMenuMusic());
            AudioManager.instance.PlayAmbient("AmbientMenu");
            settingsUI.SetActive(false);
            _fader = FindAnyObjectByType<FaderExample>();
        }

        private void PlayGame()
        {
            AudioManager.instance.PlaySfx("UI buttons");
            AudioManager.instance.musicSource.Stop();
            // Добавить метод для замедленного приближения камеры к автомату (корутину мб)
            cutSceneUI.SetActive(true);
            mainMenu.SetActive(false);
            controlButtons.SetActive(false);
            AudioManager.instance.PlayAmbient("Ambient");
        }

        private void ToggleSettings()
        {
            AudioManager.instance.PlaySfx("UI buttons");
            controlButtons.SetActive(!controlButtons.activeSelf);
            settingsUI.SetActive(!settingsUI.activeSelf);
            
            if (settingsUI.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(backButton.gameObject);    
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(playButton.gameObject);
            }
        }

        private void ExitGame()
        {
            AudioManager.instance.PlaySfx("UI buttons");
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private IEnumerator PlayMenuMusic()
        {
            AudioManager.instance.PlayMusic("MainMenu");
            float clipLength = AudioManager.instance.GetMusicClipLength("MainMenu");
            yield return new WaitForSeconds(clipLength);
            AudioManager.instance.PlayMusic("MainMenuLoop");
        }
    }
}

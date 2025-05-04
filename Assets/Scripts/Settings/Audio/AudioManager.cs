using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Settings.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField] public AudioSource sfxSource;
        [SerializeField] public AudioSource musicSource;
        [SerializeField] public AudioSource voiceSource;
        [SerializeField] public AudioSource ambientSource;
        [SerializeField] private List<Sound> music, sounds, ambient;
        [SerializeField] private AudioResource musicAudioRandomController;

        [SerializeField] private List<AudioResource> voiceLineContainers;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void PlaySfx(string soundName)
        {
            Sound s = sounds.Find(x => x.name == soundName);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            
            sfxSource.PlayOneShot(s.clip);
        }

        public void PlayMusic(string musicName)
        {
            Sound s = music.Find(x => x.name == musicName);

            if (s == null)
            {
                Debug.LogWarning("Music: " + musicName + " not found!");
                return;
            }
            
            musicSource.clip = s.clip;
            musicSource.Play();
        }

        public void PlayAmbient(string ambientName)
        {
            Sound s = ambient.Find(x => x.name == ambientName);

            if (s == null)
            {
                Debug.LogWarning("Ambient: " + ambientName + " not found!");
                return;
            }

            ambientSource.clip = s.clip;
            ambientSource.Play();
        }
        
        public void StartMusicShuffle()
        {
            musicSource.resource = musicAudioRandomController;
            musicSource.Play();
        }

        public float GetMusicClipLength(string musicName)
        {
            Sound s = music.Find(x => x.name == musicName);

            if (s == null)
            {
                Debug.LogWarning("Music: " + musicName + " not found!");
            }

            return s.clip.length;
        }

        /// <summary>
        /// Changes antialiasing parameter in camera rendering settings.
        /// </summary>
        /// <param name="index">
        ///     Номер набора реплик. 0 - Реплики при неправильном вводе пароля сундука. 1 - Реплики при игре в догонялки зайца.
        /// </param>
        public void PlayRandomVoiceLine(int index)
        {
            voiceSource.resource = voiceLineContainers[index];
            voiceSource.Play();
        }
    }
}

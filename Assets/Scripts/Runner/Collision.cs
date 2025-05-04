using System;
using Settings.Audio;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runner
{
    public class Collision : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Obstacle")
            {
                AudioManager.instance.PlaySfx("Car crash");
                SceneManager.LoadScene("Runner"); //game over and reload scene
            }
        }
    }
}

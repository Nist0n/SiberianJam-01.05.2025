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
                SceneManager.LoadScene("Runner");//game over and reload scene
            }
        }
    }
}

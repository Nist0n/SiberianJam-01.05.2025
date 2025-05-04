using UnityEngine;

namespace Runner
{
    public class Movement : MonoBehaviour
    {
        private Transform _player;
        private bool _lane1 = false, _lane2 = true, _lane3 = false;
        private bool _up = false;

        [SerializeField] private GameObject car;

        private Animator _carAnimator;
        
        private void Start()
        {
            _player = GetComponent<Transform>();
            _carAnimator = car.GetComponent<Animator>();
            _carAnimator.Play("leftback");
            _carAnimator.Play("rightback");
            _carAnimator.Play("frontright");
            _carAnimator.Play("frontleft");
        }

        private void Update()
        {
            HandleLaneSwitching();
        
            HandleJump();
        
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * 5.0f);
        }

        private void HandleLaneSwitching()
        {
            if (SwipeManager.swipeRight && _lane3 == false && _lane1 == true)
            {
                _lane2 = true;
                _lane1 = false;
                _lane3 = false;
            }
            else if (SwipeManager.swipeLeft && _lane2 == true && _player.position.z <= 0.2f)
            {
                _lane1 = true;
                _lane2 = false;
                _lane3 = false;
            }
            else if (SwipeManager.swipeRight && _lane2 == true && _player.position.z >= -0.2f)
            {
                _lane3 = true;
                _lane1 = false;
                _lane2 = false;
            }
            else if (SwipeManager.swipeLeft && _lane1 == false && _lane3 == true)
            {
                _lane2 = true;
                _lane1 = false;
                _lane3 = false;
            }
        
            if (_lane3 && _player.position.z < 1.1f)
            {
                _player.position += new Vector3(0, 0, 10.5f * Time.deltaTime);
            }
            else if (_lane1 && _player.position.z > -1.1f)
            {
                _player.position += new Vector3(0, 0, -10.5f * Time.deltaTime);
            }
            else if (_lane2 && _player.position.z <= -0.1f)
            {
                _player.position += new Vector3(0, 0, 10.5f * Time.deltaTime);
            }
            else if (_lane2 && _player.position.z >= 0.1f)
            {
                _player.position += new Vector3(0, 0, -10.5f * Time.deltaTime);
            }
        }

        private void HandleJump()
        {
            if (SwipeManager.swipeUp && _player.position.y <= 0f && _up == false)
            {
                _up = true;
            }

            if (_up && _player.position.y <= 1.6f)
            {
                _player.position += new Vector3(0, 5.0f * Time.deltaTime, 0);
            }
            else if (_player.position.y > 0f)
            {
                _up = false;
                _player.position += new Vector3(0, -5.0f * Time.deltaTime, 0);
            }
        }
    }
}
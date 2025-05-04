using System;
using System.Collections;
using Static_Classes;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Duck
{
    public class Duck : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxMoveDistance = 10f;
        [SerializeField] private int health = 5;

        [SerializeField] private GameObject eggPrefab;
        
        [SerializeField] private Slider healthbar;

        private FaderExample _fader;
        
        private bool _reachedGoal = true;
        private Vector3 _goal;

        private readonly float _screenZMax = 85;
        private readonly float _screenZMin = -90;
        private readonly float _screenYMax = 90;
        private readonly float _screenYMin = 20;
        
        private void OnEnable()
        {
            GameEvents.DuckDefeated += DuckDefeated;
        }

        private void OnDisable()
        {
            GameEvents.DuckDefeated -= DuckDefeated;
        }

        private void DuckDefeated()
        {
            StartCoroutine(DuckDied());
        }

        private void Start()
        {
            healthbar.maxValue = health;
            healthbar.value = health;

            _fader = FindAnyObjectByType<FaderExample>();
        }

        private void Update()
        {
            if (health <= 0)
            {
                return;
            }
            
            if (_reachedGoal)
            {
                float randomY = Random.Range(-maxMoveDistance, maxMoveDistance) + transform.position.y;
                float randomZ = Random.Range(-maxMoveDistance, maxMoveDistance) + transform.position.z;
                float newY = Mathf.Clamp(randomY, _screenYMin, _screenYMax);
                float newZ = Mathf.Clamp(randomZ, _screenZMin, _screenZMax);
                _goal = new Vector3(0, newY, newZ);
                _reachedGoal = false;
            }
            
            if (Vector3.SqrMagnitude(_goal - transform.position) < 0.01f)
            {
                _reachedGoal = true;
            }
            else
            {
                // float angle = Vector3.Angle(transform.position, _goal);
                // transform.Rotate(0, -angle, 0);
                transform.position += (_goal - transform.position).normalized * (Time.deltaTime * speed);
            }
        }

        public void ReceiveDamage(int damage)
        {
            health -= damage;
            healthbar.value = health;
            if (health <= 0)
            {
                GameEvents.DuckDefeated?.Invoke();
            }
        }

        private IEnumerator DuckDied()
        {
            // possibly wait for death animation
            yield return new WaitForSeconds(1f);
            Instantiate(eggPrefab, transform.position, Quaternion.identity);
            healthbar.gameObject.SetActive(false);
            Destroy(gameObject);
            
            _fader.LoadScene("MainMenu");
        }
    }
}
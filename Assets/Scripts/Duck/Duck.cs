using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Duck
{
    public class Duck : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxMoveDistance = 10f;
        [SerializeField] private int health = 5;
        
        private bool _reachedGoal = true;
        private Vector3 _goal;

        private float _screenZMax = 85;
        private float _screenZMin = -90;
        private float _screenYMax = 90;
        private float _screenYMin = 20;
        
        private Camera _mainCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
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
                transform.position += (_goal - transform.position).normalized * (Time.deltaTime * speed);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Serialization;

namespace Runner
{
    public class TargetObject : MonoBehaviour
    {
        [SerializeField] private float startDistance = 50f;
        [SerializeField] private float speed = 5f;    
        [SerializeField] private float minDistance = 1f;   // Дистанция "победы"

        private Transform _player;
        private bool _isReached = false;
        private float _currentDistance;

        public float DistanceToPlayer => _currentDistance;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = new Vector3(
                _player.position.x + startDistance,
                _player.position.y,
                0
            );
        }

        private void Update()
        {
            if (_isReached) return;
        
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        
            _currentDistance = Vector3.Distance(
                new Vector3(transform.position.x, 0, 0),
                new Vector3(_player.position.x, 0, 0)
            );

            if (_currentDistance <= minDistance)
            {
                _isReached = true;
                Debug.Log("Цель достигнута");
            }
        }
    }
}
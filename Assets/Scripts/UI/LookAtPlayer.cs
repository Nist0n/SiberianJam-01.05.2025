using Settings.Audio;
using UnityEngine;

namespace UI
{
    public class LookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool useMainCameraAsTarget;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private bool lockXAxis = false;
        [SerializeField] private bool lockYAxis = false;
        [SerializeField] private bool lockZAxis = false;
        [SerializeField] private Vector3 rotationOffset;

        private void Start()
        {
            AudioManager.instance.PlaySfx("Help tip");
            if (!target)
            {
                if (useMainCameraAsTarget && Camera.main)
                {
                    target = Camera.main.transform;
                }
                else
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player) target = player.transform;
                }
            }
        }

        private void Update()
        {
            if (!target) return;
        
            Vector3 direction = target.position - transform.position;
        
            Quaternion targetRotation = Quaternion.LookRotation(direction);
        
            Vector3 euler = targetRotation.eulerAngles;
            if (lockXAxis) euler.x = transform.rotation.eulerAngles.x;
            if (lockYAxis) euler.y = transform.rotation.eulerAngles.y;
            if (lockZAxis) euler.z = transform.rotation.eulerAngles.z;
            targetRotation = Quaternion.Euler(euler);
        
            targetRotation *= Quaternion.Euler(rotationOffset);
        
            if (rotationSpeed > 0)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }
}
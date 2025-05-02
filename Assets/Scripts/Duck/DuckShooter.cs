using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Duck
{
    public class DuckShooter : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float shotDelay = 0.5f;
        [SerializeField] private float cooldown = 1f;

        [SerializeField] private Image shootCdImage;
        [SerializeField] private GameObject crosshairImage;
        [SerializeField] private GameObject crosshairHitImage;
        
        private InputAction _shootAction;

        private float _shotTimer;
        private bool _canShoot = true;
        
        private void Start()
        {
            _shootAction = InputSystem.actions.FindAction("Attack");
            Cursor.visible = false;
        }

        private void Update()
        {
            crosshairImage.transform.position = Mouse.current.position.ReadValue();
            crosshairHitImage.transform.position = Mouse.current.position.ReadValue();
            if (_shotTimer > cooldown)
            {
                _canShoot = true;
                _shotTimer = 0f;
            }
            
            if (_canShoot)
            {
                if (_shootAction.WasPressedThisFrame())
                {
                    StartCoroutine(Shoot());
                }
            }
            else
            {
                _shotTimer += Time.deltaTime;
                var fillPercentage = (cooldown - _shotTimer) / cooldown;
                shootCdImage.fillAmount = fillPercentage;
            }
        }

        private IEnumerator Shoot()
        {
            _canShoot = false;
            Debug.Log("Shot");
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            yield return new WaitForSeconds(shotDelay);
            crosshairHitImage.SetActive(true);
            crosshairImage.SetActive(false);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Duck"))
                {
                    Debug.Log("Duck hit");
                    Duck duck = hit.collider.gameObject.GetComponent<Duck>();
                    duck.ReceiveDamage(damage);
                }
            }

            yield return new WaitForSeconds(0.2f);
            crosshairHitImage.SetActive(false);
            crosshairImage.SetActive(true);
        }
    }
}
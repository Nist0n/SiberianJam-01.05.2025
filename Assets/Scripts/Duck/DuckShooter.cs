using System.Collections;
using System.Collections.Generic;
using Settings.Audio;
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
        [SerializeField] private GameObject fireImageGameObject;

        [SerializeField] private List<Sprite> fireSprites;
        
        
        private InputAction _shootAction;

        private float _shotTimer;
        private bool _canShoot = true;

        private bool _shooting;

        private Image _fireImage;
        
        private void Start()
        {
            AudioManager.instance.PlaySfx("Quack");
            AudioManager.instance.PlaySfx("Надо сбить");
            AudioManager.instance.PlayAmbient("Start a vehicle");
            _shootAction = InputSystem.actions.FindAction("Attack");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
            
            _fireImage = fireImageGameObject.GetComponent<Image>();
        }

        private void Update()
        {
            if (!_shooting)
            {
                crosshairImage.transform.position = Mouse.current.position.ReadValue();
                            crosshairHitImage.transform.position = Mouse.current.position.ReadValue();
            }
            
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
            _shooting = true;
            Debug.Log("Shot");
            AudioManager.instance.PlaySfx("Plasma");
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            yield return new WaitForSeconds(shotDelay);
            StartCoroutine(PlayShotAnimation());
            
            Mouse.current.WarpCursorPosition(crosshairImage.transform.position);
            _shooting = false;
            crosshairHitImage.SetActive(true);
            crosshairImage.SetActive(false);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Duck"))
                {
                    Debug.Log("Duck hit");
                    AudioManager.instance.PlaySfx("Есть попал");
                    AudioManager.instance.PlaySfx("Quack");
                    Duck duck = hit.collider.gameObject.GetComponent<Duck>();
                    duck.ReceiveDamage(damage);
                }
            }

            yield return new WaitForSeconds(0.2f);
            crosshairHitImage.SetActive(false);
            crosshairImage.SetActive(true);
        }

        private IEnumerator PlayShotAnimation()
        {
            fireImageGameObject.SetActive(true);
            fireImageGameObject.transform.position = crosshairImage.transform.position;
            foreach (var sprite in fireSprites)
            {
                _fireImage.sprite = sprite;
                yield return new WaitForSeconds(0.07f);
            }
            _fireImage.sprite = fireSprites[0];
            fireImageGameObject.SetActive(false);
        }
    }
}
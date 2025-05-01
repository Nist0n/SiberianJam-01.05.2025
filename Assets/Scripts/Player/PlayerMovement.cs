using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody _rb;
    private Vector3 _vector3;
    private AudioSource _audio;
    private float _speed = 8;
    private float _rotationSpeed = 500;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        _vector3.x = Input.GetAxis("Horizontal");
        _vector3.z = Input.GetAxis("Vertical");

        _rb.MovePosition(_rb.position - _vector3 * _speed * Time.deltaTime);
        
        SetRunningAnim();
        
        Vector3 movement = new Vector3(-_vector3.x, 0, -_vector3.z);
        movement.Normalize();

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }

    }

    private void SetRunningAnim()
    {
        if (_vector3.x == 0 && _vector3.z == 0)
        {
            _anim.SetBool("isRunning", false);
        }
        else
        {
            _anim.SetBool("isRunning", true);
        }
    }

    public void AudioGo()
    {
        _audio.PlayOneShot(_audio.clip);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterAnimationManager : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();

        _animator.SetFloat("dirY", -1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = _rigidBody.velocity;
        bool idle = delta.magnitude < 0.01f;

        if (!idle)
        {
            _animator.SetFloat("dirX", delta.x);
            _animator.SetFloat("dirY", delta.y);
        }
        _animator.SetBool("idle", idle);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationManager : MonoBehaviour
{
    private Animator _animator;

    private Vector3 _position;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;

        Vector3 delta = newPos - _position;
        delta *= 100.0f;
        bool idle = delta.magnitude < 0.001f;

        if (!idle)
        {
            _animator.SetFloat("dirX", delta.x);
            _animator.SetFloat("dirY", delta.y);
        }
        _animator.SetBool("idle", idle);

        _position = newPos;
    }
}

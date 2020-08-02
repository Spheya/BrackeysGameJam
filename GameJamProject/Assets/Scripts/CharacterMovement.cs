using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.28f;

    private Rigidbody2D _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Use this to move the character
    public void Move(Vector2 inputVector)
    {
        Vector2 movementVector = new Vector2(inputVector.x, inputVector.y) * moveSpeed;
        _rigidBody.velocity = movementVector;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y/10.0f);
    }
}

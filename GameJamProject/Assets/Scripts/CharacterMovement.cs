using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.28f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Use this to move the character
    public void Move(Vector2 inputVector)
    {
        Vector3 movementVector = new Vector3(inputVector.x, inputVector.y, 0.0f) * moveSpeed * Time.deltaTime;
        transform.Translate(movementVector.x, movementVector.y, 0.0f);
    }
}

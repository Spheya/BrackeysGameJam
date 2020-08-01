using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class Player : MonoBehaviour
{
    CharacterMovement character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Joystick X");
        inputVector.y = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Joystick Y");

        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
        character.Move(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Unsafe but this is only for testing!
            GetComponent<Recording>().Play();
        }
    }
}

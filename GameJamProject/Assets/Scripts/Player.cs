using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Recording))]
public class Player : MonoBehaviour
{
    CharacterMovement character;
    static bool activeRecording = false;

    [Range(0.0f, 100.0f)]
    public float health = 100.0f;

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
            Die();
        }
    }

    public void Die()
    {
        if (!activeRecording)
        {
            Recording recording = GetComponent<Recording>();
            Vector2 origin = recording.GetStartPosition();
            
            Recording[] recordings = FindObjectsOfType<Recording>();
            
            Instantiate(gameObject, new Vector3(origin.x, origin.y, 0.0f), Quaternion.identity);
            for (int i = 0; i < recordings.Length; i++)
            {
                recordings[i].Play();
            }

            activeRecording = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        activeRecording = false;
    }
}

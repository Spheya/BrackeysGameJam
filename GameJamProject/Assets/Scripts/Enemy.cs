using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Recording))]
public class Enemy : MonoBehaviour
{
    GameObject targetCharacter;

    CharacterMovement character;

    [Range(0.0f, 100.0f)]
    public float health = 100.0f;
    private float speed = 1.0f;
    [Range(0.0f, 10.0f)]
    public float maxSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterMovement>();

        if (!targetCharacter)
        {
            FindPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetCharacter)
        {
            FindPlayer();
        }

        if (targetCharacter)
        {
            Vector2 inputVector = (targetCharacter.transform.position - transform.position).normalized * speed;

            character.Move(inputVector);
        }
    }

    private void FindPlayer()
    {
        GameObject[] targetCharacters = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = float.MaxValue;
        Debug.Log("Found " + targetCharacters.Length + " player character(s)");
        for (int i = 0; i < targetCharacters.Length; i++)
        {
            float magnitude = (transform.position - targetCharacters[i].transform.position).magnitude;
            if (magnitude < minDistance)
            {
                minDistance = magnitude;
                targetCharacter = targetCharacters[i];
            }
        }
    }

    public void Die()
    {
        
    }
}

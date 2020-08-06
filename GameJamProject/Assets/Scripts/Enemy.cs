using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Recording))]
public class Enemy : MonoBehaviour
{
    GameObject targetCharacter;

    CharacterMovement character;

    Vector2 velocity = Vector2.zero;
    const float maxTurnVelocity = 0.2f;
    float turnCooldownTimer = 0.0f;
    const float turnCooldown = 0.8f;

    [Range(0.0f, 100.0f)]
    public float health = 100.0f;
    private float speed = 0.0f;
    [Range(0.0f, 10.0f)]
    public float maxSpeed = 5.0f;
    [Range(0.0f, 20.0f)]
    public float acceleration = 10.0f;

    public GameObject explosion;

    public bool doUpdate = true;

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
        if (doUpdate)
        {
            if (!targetCharacter || !targetCharacter.GetComponent<Recording>().Alive)
            {
                FindPlayer();
            }

            if (targetCharacter)
            {
                Vector2 inputVector = (targetCharacter.transform.position - transform.position).normalized;

                // true if enemy wants to go up/down more than left/right
                bool hInputAxis = Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.y);
                 // true if enemy is currently going up/down more than left/right
                bool hVelocityAxis = Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y);

                if (turnCooldownTimer < turnCooldown)
                {
                    turnCooldownTimer += Time.deltaTime;
                }

                if ((hInputAxis == hVelocityAxis) || (turnCooldownTimer < turnCooldown))
                {
                    // Enemy wants to travel along the same axis it's already on
                    if (hVelocityAxis)
                    {
                        // moving horizontally
                        velocity.x += acceleration * Mathf.Sign(inputVector.x) * Time.deltaTime;
                    }
                    else
                    {
                        // moving vertically
                        velocity.y += acceleration * Mathf.Sign(inputVector.y) * Time.deltaTime;
                    }
                }
                else
                {
                    // Enemy wants to change axis (decelerate current axis)
                    if (hVelocityAxis)
                    {
                        // moving horizontally
                        velocity.x -= acceleration * Mathf.Sign(velocity.x) * Time.deltaTime;
                        if (Mathf.Abs(velocity.x) < maxTurnVelocity)
                        {
                            velocity.y += acceleration * Mathf.Sign(inputVector.y) * Time.deltaTime;
                            velocity.x = 0;
                        }
                    }
                    else
                    {
                        // moving vertically
                        velocity.y -= acceleration * Mathf.Sign(velocity.y) * Time.deltaTime;
                        if (Mathf.Abs(velocity.y) < maxTurnVelocity)
                        {
                            velocity.x += acceleration * Mathf.Sign(inputVector.x) * Time.deltaTime;
                            velocity.y = 0;
                        }
                    }
                }

                if (velocity.magnitude > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }
                character.Move(velocity);
            }
        }
        else
        {
            character.Move(new Vector2(0.0f, 0.0f));
        }
    }

    private void LateUpdate()
    {
        // I should get the enemy to collide with two specific colliders, but idk how to do that tbh. so this code came to be

        if (transform.position.y > 4.0f)
            transform.position = new Vector3(transform.position.x, 4.0f, transform.position.y);

        if (transform.position.y < -3.55f)
            transform.position = new Vector3(transform.position.x, -3.55f, transform.position.y);
    }

    private void FindPlayer()
    {
        GameObject[] targetCharacters = GameObject.FindGameObjectsWithTag("Player").Where(x => x.GetComponent<Recording>().Alive).ToArray();
        float minDistance = float.MaxValue;
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
        var effect = Instantiate(explosion);
        effect.transform.position = new Vector3(transform.position.x, transform.position.y, 5.0f);
        GetComponent<Recording>().Die();
        doUpdate = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0.0f)
            Die();
    }
}

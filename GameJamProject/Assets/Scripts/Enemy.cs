using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                Vector2 inputVector = (targetCharacter.transform.position - transform.position).normalized * speed;

                character.Move(inputVector);
            }
        }
        else
        {
            character.Move(new Vector2(0.0f, 0.0f));
        }
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

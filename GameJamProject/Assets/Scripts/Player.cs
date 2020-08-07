using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Recording))]
public class Player : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    CharacterMovement character;
    static bool activeRecording = false;

    [Range(0.0f, 100.0f)]
    public float health = 100.0f;
    [Range(0.0f, 4.0f)]
    public float invincibilityTime = 1.5f;
    private float invincibleTimer = float.MaxValue;

    HealthBarMask healthBar;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar = FindObjectOfType<HealthBarMask>();
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
        
        if (invincibleTimer < invincibilityTime)
        {
            invincibleTimer += Time.deltaTime;

            spriteRenderer.enabled = ((int)(invincibleTimer * 8)) % 2 > 0;

            if (invincibleTimer >= invincibilityTime)
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    public void Die()
    {
        if (!rewinding)
        {
            if (FindObjectsOfType<Player>().Where(x => x.GetComponent<Recording>().Alive).Count() < 2)
            {
                rewinding = true;
                FindObjectOfType<RewindPostProcessing>().Play();
                StartCoroutine(RewindAfterCooldown());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private bool rewinding = false;
    private IEnumerator RewindAfterCooldown()
    {
        yield return new WaitForSeconds(0.2f);

        Recording recording = GetComponent<Recording>();
        Vector2 origin = recording.GetStartPosition();
        health = recording.GetStartHealth();

        spriteRenderer.enabled = true;

        float newTime = Mathf.Max(recording.Timer - Recording.maxTime, 0.0f);
        float rewindAmount = recording.Timer - newTime;

        Recording[] recordings = FindObjectsOfType<Recording>();

        var obj = Instantiate(gameObject, new Vector3(origin.x, origin.y, 0.0f), Quaternion.identity);
        obj.name = "Player";
        name = "Player(Clone)";
        for (int i = 0; i < recordings.Length; i++)
        {
            recordings[i].Play(recordings[i].Timer - rewindAmount);
        }

        healthBar.Player = obj.GetComponent<Player>();

        activeRecording = true;

        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.45f);
        GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1.0f, 1.0f, 1.0f, 0.45f);

        Destroy(FindObjectOfType<RewindTimer>()?.gameObject);

        rewinding = false;
    }

    private void OnDestroy()
    {
        activeRecording = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (invincibleTimer > invincibilityTime)
        {
            if (other.gameObject.tag == "Enemy")
            {
                TakeDamage(17.0f);
            }
        }
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Took damage - Health : " + health);
        if (health < 0)
        {
            Die();
        }
        invincibleTimer = 0;
    }
}

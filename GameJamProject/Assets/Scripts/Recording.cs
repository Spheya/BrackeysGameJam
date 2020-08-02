using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recording : MonoBehaviour
{
    bool recording = true;

    [SerializeField]
    bool destroyOnFinish = true;

    const float maxTime = 10.0f; // seconds between the start and end of the recording
    const float precision = 1.0f / 30.0f; // time between recorded positions
    float positionTimer = 0.0f;
    float totalTimer = 0.0f;
    Queue<Vector2> recordingPositions = new Queue<Vector2>();
    Queue<Vector3> recordingBullets = new Queue<Vector3>(); // (x, y) for position, I'm using z as time (so I don't need another queue lol)

    Vector2 currentPosition;
    Vector2 nextPosition;
    Vector3 currentGun;

    Gun gun;

    // Start is called before the first frame update
    void Start()
    {
        recordingPositions.Enqueue(new Vector2(transform.position.x, transform.position.y));
        gun = transform.GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        positionTimer += Time.deltaTime;
        totalTimer += Time.deltaTime;

        if (recording)
        {
            if (positionTimer > precision)
            {
                positionTimer -= precision;
                recordingPositions.Enqueue(new Vector2(transform.position.x, transform.position.y));

                // Don't let the queue get longer than it needs to be! We don't need to record the entire game, only the last few seconds
                if ((float)recordingPositions.Count * precision > maxTime)
                {
                    recordingPositions.Dequeue();
                }
            }

            // This is to trim bullets that were fired over 10 seconds ago
            if (recordingBullets.Count > 0)
            {
                if (recordingBullets.Peek().z < totalTimer - maxTime)
                {
                    recordingBullets.Dequeue();
                }
            }
        }
        else
        {
            // play the recording

            if (positionTimer > precision)
            {
                positionTimer -= precision;

                if (recordingPositions.Count > 0)
                {
                    currentPosition = nextPosition;
                    nextPosition = recordingPositions.Dequeue();
                }
                else
                {
                    if (destroyOnFinish)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        recording = true;

                        Enemy enemy = GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.enabled = true;
                        }
                    }
                }
            }

            float time;
            Vector2 lerpPosition;

            if (gun)
            {
                if (recordingBullets.Count > 0)
                {
                    time = Mathf.InverseLerp(currentGun.z, recordingBullets.Peek().z, totalTimer);
                    Vector3 nextGun = recordingBullets.Peek();

                    Vector3 offset = Vector3.Lerp(currentGun, nextGun, time);
                    offset.z = offset.y / 100.0f;
                    gun.transform.position = gun.Parent.transform.position + offset;

                    // Rotate towards that direction and flip if the gun appears upside down
                    float rotation = (Mathf.Atan2(offset.y, offset.x) * 180.0f / Mathf.PI + 180.0f) % 360.0f;
                    gun.transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
                    gun.transform.localScale = new Vector3(1.0f, (rotation < 270.0f && rotation > 90.0f) ? -1.0f : 1.0f, 1.0f);

                    if (totalTimer > nextGun.z)
                    {
                        gun.Shoot();
                        currentGun = recordingBullets.Dequeue();
                        Debug.Log(currentGun);
                    }
                }
            }

            time = positionTimer / precision; // time normalised between 0 and 1, for interpolation
            lerpPosition = Vector2.Lerp(currentPosition, nextPosition, time);
            transform.position = new Vector3(lerpPosition.x, lerpPosition.y, 0.0f);
        }
    }

    public void Play()
    {
        recordingPositions.Enqueue(new Vector2(transform.position.x, transform.position.y));

        recording = false;
        positionTimer = 0.0f;
        totalTimer -= maxTime;

        Player player = GetComponent<Player>();
        if (player != null)
        {
            player.enabled = false;
        }
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.enabled = false;
        }
        if (gun != null)
        {
            gun.DoUpdate = false;
        }

        currentPosition = recordingPositions.Dequeue();
        nextPosition = recordingPositions.Dequeue();

        currentGun = new Vector3(1.0f, 0.0f, 0.0f);
    }

    public Vector2 GetStartPosition()
    {
        if (recordingPositions.Count > 0)
        {
            return recordingPositions.Peek();
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void RecordBullet(Vector2 direction)
    {
        recordingBullets.Enqueue(new Vector3(direction.x, direction.y, totalTimer));
    }
}

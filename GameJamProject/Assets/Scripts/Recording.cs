using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recording : MonoBehaviour
{
    bool recording = true;

    const float maxTime = 10.0f; // seconds between the start and end of the recording
    const float precision = 1.0f / 30.0f; // time between recorded positions
    float positionTimer = 0.0f;
    float totalTimer = 0.0f;
    Queue<Vector2> recordingPositions = new Queue<Vector2>();
    Queue<Vector3> recordingBullets = new Queue<Vector3>(); // (x, y) for position, I'm using z as time (so I don't need another queue lol)

    Vector2 currentPosition;
    Vector2 nextPosition;

    // Start is called before the first frame update
    void Start()
    {
        recordingPositions.Enqueue(new Vector2(transform.position.x, transform.position.y));
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
                    Destroy(gameObject);
                }
            }

            float time = positionTimer / precision; // time normalised between 0 and 1, for interpolation
            Vector2 lerpPosition = Vector2.Lerp(currentPosition, nextPosition, time);
            transform.position = new Vector3(lerpPosition.x, lerpPosition.y, 0.0f);
        }
    }

    public void Play()
    {
        recordingPositions.Enqueue(new Vector2(transform.position.x, transform.position.y));

        recording = false;
        positionTimer = 0.0f;
        totalTimer = 0.0f;

        Player player = GetComponent<Player>();
        if (player != null)
        {
            player.enabled = false;
        }

        currentPosition = recordingPositions.Dequeue();
        nextPosition = recordingPositions.Dequeue();
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Recording : MonoBehaviour
{
    bool playing = false;

    [SerializeField]
    bool destroyOnFinish = true;

    [SerializeField]
    float precision = 1.0f / 30.0f;
    public const float maxTime = 10.0f;

    [SerializeField]
    bool alive = true;
    public bool Alive { get => alive;
        set => alive = value; }

    RecordSample shotThisSample = null;

    // A simple class to make storing samples easier
    private class RecordSample
    {
        public float Time { get; }

        public Vector2 Position { get; }
        public Vector2 GunDirection { get; }
        public bool Shoot { get; }
        public float Health { get; }

        public RecordSample(float time, Vector2 position, float health)
        {
            Time = time;
            Position = position;
            GunDirection = Vector2.right;
            Shoot = false;
            Health = health;
        }

        public RecordSample(float time, Vector2 position, Vector2 gunDirection, bool shoot, float health)
        {
            Time = time;
            Position = position;
            GunDirection = gunDirection;
            Shoot = shoot;
            Health = health;
        }

        public static RecordSample Lerp(RecordSample a, RecordSample b, float mix) => new RecordSample(
                Mathf.Lerp(a.Time, b.Time, mix),
                Vector2.Lerp(a.Position, b.Position, mix),
                Vector2.Lerp(a.GunDirection, b.GunDirection, mix),
                false,
                Mathf.Lerp(a.Health, b.Health, mix)
        );
    }

    List<RecordSample> samples = new List<RecordSample>();

    Gun gun;

    float timer;
    public float Timer => timer;

    void Start()
    {
        timer = 0.0f;
        RecordNewSample();

        gun = GetComponentInChildren<Gun>();
    }

    int shotIndex = -1;

    private RecordSample InterpolateSampleAt(float time)
    {
        if (time <= samples[0].Time)
            return null;

        RecordSample previousSample = null;
        for(int i = 0; i < samples.Count; i++)
        {
            var sample = samples[i];
            if(sample.Time > time)
            {
                if(previousSample.Shoot && shotIndex != i-1)
                {
                    shotIndex = i-1;
                    return previousSample;
                }else
                {
                    shotIndex = -1;
                }

                float mix = (time - previousSample.Time) / (sample.Time - previousSample.Time);
                return RecordSample.Lerp(previousSample, sample, mix);
            }
            previousSample = samples[i];
        }
        return null;
    }

    private void ApplySample(RecordSample sample)
    {
        transform.position = sample.Position;

        if (gun)
        {
            gun.AimAt(sample.GunDirection);

            if (sample.Shoot && sample != shotThisSample)
            {
                gun.Shoot();
                shotThisSample = sample;
            }
        }
    }

    private void RecordNewSample()
    {
        float health = 100;
        var enemy = GetComponent<Enemy>();
        if (enemy)
            health = enemy.health;

        if (gun)
        {
            samples.Add(new RecordSample(timer, transform.position, gun.transform.position - transform.position, false, health));
        }
        else
        {
            samples.Add(new RecordSample(timer, transform.position, health));
        }
    }

    private void CullOldSamples()
    {
        while (samples.Count > 0 && samples[0].Time < timer - maxTime)
            samples.RemoveAt(0);
    }

    void Update()
    {
        // Keep the timer up to date
        timer += Time.deltaTime;

        // Don't let the list get longer than it needs to be! We don't need to record the entire game, only the last few seconds
        CullOldSamples();

        // If this object is dead and no samples are stored anymore, there's no use respawning it, as there wont be any data to display
        if (!Alive && samples.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        if (timer < 0.0f)
        {
            // This object was rewinded to a point before its creation
            Die();
        }
        else
        {
            var sample = InterpolateSampleAt(timer);

            if(sample == null)
            {
                if (Alive)
                {
                    if (playing)
                    {
                        if (destroyOnFinish)
                        {
                            Die();
                        }
                        else
                        {
                            playing = false;

                            Enemy enemy = GetComponent<Enemy>();
                            if (enemy != null)
                            {
                                enemy.doUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        // No sample found at the current time, so we are recording
                        var previousSample = samples[samples.Count - 1];
                        if (timer - previousSample.Time >= precision)
                        {
                            // The duration of a sample has passed since the previous sample, so we need to make a new one
                            RecordNewSample();
                        }
                    }
                }
            }
            else
            {
                // A sample was stored for the current time, so we are playing back
                if (!Alive)
                    Resurrect();

                ApplySample(sample);
            }
        }
    }

    public void Play() => Play(timer - maxTime);

    public void Play(float newTime)
    {
        // Play back
        //if (!playing)
        //    RecordNewSample();
        timer = newTime;

        playing = true;

        // Make sure other components dont try to take control
        // eliminate the middle class
        Player player = GetComponent<Player>();
        if (player != null)
        {
            player.enabled = false;
        }
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.doUpdate = false;
            if(samples.Count > 0)
                enemy.health = samples[0].Health;
        }
        if (gun != null)
        {
            gun.DoUpdate = false;
        }

        if (!Alive)
          Resurrect();
    }

    public Vector2 GetStartPosition()
    {
        return samples[0].Position;
    }

    public void RecordBullet(Vector2 direction)
    {
        samples.Add(new RecordSample(timer, transform.position, direction, true, 100.0f));
    }

    public void Die()
    {
        // Dying, but this may get rolled back

        // Get rid of all the samples that exist after the point of dying
        for (int i = 0; i < samples.Count; i++)
        {
            if (samples[i].Time >= timer)
            {
                samples.RemoveAt(i);
                --i;
            }
        }

        // Record a sample at the moment this object dies for that frame perfect consistency
        RecordNewSample();

        Alive = false;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.enabled = false;
        }
    }

    private void Resurrect()
    {
        Alive = true;

        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.enabled = true;
            enemy.doUpdate = false;
            enemy.health = 100.0f;
        }

        // Dying, but in reverse
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            collider.enabled = true;
        }
    }
}

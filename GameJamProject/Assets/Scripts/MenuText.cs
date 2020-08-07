using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class MenuText : MonoBehaviour
{
    public float frequency = 2.0f;
    public float baseAlpha = 0.2f;

    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Min(Mathf.Cos(Time.time * frequency * Mathf.PI * 2.0f) * 0.5f  + 0.5f + baseAlpha, 1.0f));

        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("SampleScene");
    }
}

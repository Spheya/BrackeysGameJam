using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindPostProcessing : MonoBehaviour
{
    public Material postProcessingMaterial;

    public float animationDuration = 1.0f;

    private float t = 1.0f;


    private void Update()
    {
        t += Time.deltaTime / animationDuration;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        postProcessingMaterial.SetFloat("_T", t);
        postProcessingMaterial.SetFloat("_Aspect", (float)Screen.width / Screen.height);

        Graphics.Blit(source, destination, postProcessingMaterial);
    }

    public void Play()
    {
        t = 0.0f;
    }
}

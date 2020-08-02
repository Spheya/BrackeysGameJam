using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time;

    void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

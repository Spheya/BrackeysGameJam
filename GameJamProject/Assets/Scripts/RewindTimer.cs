using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindTimer : MonoBehaviour
{
    public void OnTimerComplete()
    {
        FindObjectOfType<Player>().Die();
    }
}

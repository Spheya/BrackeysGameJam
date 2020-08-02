using System.Collections;
using UnityEngine;

class RewindTimerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _timer;
    public GameObject Timer { get => _timer; set => _timer = value; }

    [SerializeField]
    private float _interval = 20.0f;
    public float Interval { get => _interval; set => _interval = value; }

    [SerializeField]
    private float _initialCooldown = 2.0f;
    public float InitialCooldown { get => _initialCooldown; set => _initialCooldown = value; }

    private IEnumerator _spawner;

    private void Awake()
    {
        _spawner = Spawner();
    }

    private void OnEnable()
    {
        StartCoroutine(_spawner);
    }

    private void OnDisable()
    {
        StopCoroutine(_spawner);
    }

    public IEnumerator Spawner()
    {
        yield return new WaitForSeconds(InitialCooldown);
        while (true)
        {
            Instantiate(Timer);
            yield return new WaitForSeconds(Interval);
        }
    }
}

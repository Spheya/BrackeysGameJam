using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Enemy _prototype;
    public Enemy Prototype { get => _prototype; set => _prototype = value; }

    [SerializeField]
    private float _initialCooldown = 5.0f;

    [SerializeField]
    private float _bonusCooldown = 1.0f;

    [SerializeField]
    private float _randomBonusCooldown = 1.0f;

    [SerializeField]
    private float _cooldownGrowthFactor = 0.8f;

    [SerializeField]
    private int _maxSpawns = 8;

    [SerializeField]
    private int _minSpawns = 3;

    [SerializeField]
    private int _maxEnemies = 100;

    private int _spawns;

    private IEnumerator _spawningCoroutine;

    private void Awake()
    {
        _spawningCoroutine = SpawningCoroutine();
    }

    private void OnEnable()
    {
        StartCoroutine(_spawningCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(_spawningCoroutine);
    }

    private IEnumerator SpawningCoroutine()
    {
        while(true)
        {
            if (FindObjectsOfType<Enemy>().Length > _maxEnemies)
                yield return new WaitForSeconds(5.0f);

            var bounds = GetComponent<BoxCollider2D>().bounds;
            int spawnCount = Random.Range(_minSpawns, _maxSpawns);
            for(int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnLocation = bounds.min + new Vector3(Random.value * bounds.size.x, Random.value * bounds.size.y, Random.value * bounds.size.z);
                Instantiate(_prototype, spawnLocation, Quaternion.identity);
            }

            ++_spawns;
            yield return new WaitForSeconds(
                Mathf.Pow(_cooldownGrowthFactor, _spawns)
                + _bonusCooldown 
                + _randomBonusCooldown * Random.value
            );
        }
    }
}

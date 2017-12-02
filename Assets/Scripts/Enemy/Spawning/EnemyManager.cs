using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    //TODO: inject enemyspawner


    [SerializeField] private List<SpawnableEnemy> _enemies;
    [SerializeField] private float _difficulty;

    private float _totalChance;
    private EnemySpawner _spawner;

	void Start ()
	{
	    _spawner = GetComponent<EnemySpawner>();

	    /*this.UpdateAsObservable()
            .Delay(TimeSpan.FromSeconds(_difficulty))
            */ //Call the timer every _difficulty seconds

    }

    void Update()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        _totalChance = 0;

        foreach (var enemy in _enemies)
        {
            _totalChance += enemy._spawnChance;
        }

        float result = Random.Range(0, _totalChance);
        float r = result;

        foreach (var enemy in _enemies)
        {
            r -= enemy._spawnChance;

            if (r < 0)
            {
                _spawner.SpawnEnemy(enemy._prefab);
                break;
            }
        }
    }
}

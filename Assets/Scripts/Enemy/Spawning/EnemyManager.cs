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

    public float Difficulty;
    [SerializeField] private List<SpawnableEnemy> _enemies;
    
    private float _totalChance;
    private EnemySpawner _spawner;
    private DateTimeOffset _lastAdded;

	void Start ()
	{
	    _spawner = GetComponent<EnemySpawner>();

	    _lastAdded = DateTimeOffset.Now;

	    this.UpdateAsObservable()
	        .Timestamp()
	        .Where(x => x.Timestamp >= _lastAdded.AddSeconds(Difficulty))
	        .Subscribe(x =>
	        {
	            SpawnEnemy();
	            _lastAdded = x.Timestamp;
	        });
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

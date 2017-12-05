using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    //TODO: inject enemyspawner

    public float Difficulty;
    public List<SpawnableEnemy> Enemies;
    
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

    public void EnableEnemy(string enemyName)
    {
        var enemy = Enemies.First(x => x.Prefab.name.ToUpper() == enemyName.ToUpper());
        enemy.Spawnable = true;
        enemy.SpawnChance = 5;
    }

    public SpawnableEnemy GetEnemy(string enemyName)
    {
        return Enemies.First(x => x.Prefab.name.ToUpper() == enemyName.ToUpper());
    }

    private void SpawnEnemy()
    {
        _totalChance = 0;

        foreach (var enemy in Enemies)
        {
            if (enemy.Spawnable)
            {
                _totalChance += enemy.SpawnChance;
                enemy.SpawnChance++;
            }
        }

        float result = Random.Range(0, _totalChance);
        float r = result;

        foreach (var enemy in Enemies)
        {
            r -= enemy.SpawnChance;

            if (r < 0)
            {
                _spawner.SpawnEnemy(enemy.Prefab);
                break;
            }
        }
    }
}

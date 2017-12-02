using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private int _spawnRadius;
    [SerializeField] private GameObject _enemyPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		SpawnEnemy(_enemyPrefab);
	}

    private void SpawnEnemy(GameObject EnemyPrefabToSpawn)
    {
        var enemy = PrefabUtility.InstantiatePrefab(EnemyPrefabToSpawn) as GameObject;

        var degrees = Random.Range(0, 361);

        var x = _spawnRadius * Mathf.Cos(degrees * Mathf.Deg2Rad);
        if (Mathf.Abs(x) < 0.01f)
            x = 0;

        var y = _spawnRadius * Mathf.Sin(degrees * Mathf.Deg2Rad);
        if (Mathf.Abs(y) < 0.01f)
            y = 0;

        enemy.transform.position = new Vector3(x, 0, y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private int _spawnRadius;

    public void SpawnEnemy(GameObject EnemyPrefabToSpawn)
    {
        var degrees = Random.Range(0, 361);

        var x = _spawnRadius * Mathf.Cos(degrees * Mathf.Deg2Rad);
        if (Mathf.Abs(x) < 0.01f)
            x = 0;

        var y = _spawnRadius * Mathf.Sin(degrees * Mathf.Deg2Rad);
        if (Mathf.Abs(y) < 0.01f)
            y = 0;

        var enemy = Instantiate(EnemyPrefabToSpawn, new Vector3(x, 0, y), Quaternion.identity);
    }
}

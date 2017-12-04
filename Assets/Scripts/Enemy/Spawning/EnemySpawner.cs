using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    private AIBase.Factory _factory;
    [SerializeField] private int _spawnRadius;

    [Inject]
    private void Construct(AIBase.Factory aiFactory)
    {
        _factory = aiFactory;
    }

    public void SpawnEnemy(GameObject enemyPrefabToSpawn)
    {
        var degrees = Random.Range(0, 361);

        var x = _spawnRadius * Mathf.Cos(degrees * Mathf.Deg2Rad);
        if (Mathf.Abs(x) < 0.01f)
            x = 0;

        var y = _spawnRadius * Mathf.Sin(degrees * Mathf.Deg2Rad);
        if (Mathf.Abs(y) < 0.01f)
            y = 0;
        _factory.TargetPrefab = enemyPrefabToSpawn;
        AIBase result = _factory.Create();
        result.transform.position = new Vector3(x, 0, y);
        result.transform.rotation = Quaternion.identity;
    }
}

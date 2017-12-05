using System;
using UnityEngine;

[Serializable]
public class SpawnableEnemy
{
    public bool Spawnable = false;
    public GameObject Prefab;
    public float SpawnChance = 0;
}
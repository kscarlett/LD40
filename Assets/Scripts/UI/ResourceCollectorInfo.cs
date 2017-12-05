using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ResourceCollectorInfo
{
    public string Name;
    public float Cost;
    public float GoldPerSecond;
    public ulong UnlockThreshhold;
    public ulong EnemyUnlockThreshold;
    public string EnemyName;
    public string UnlockMessage;
    public string EnemyUnlockMessage;
    public bool HasRun = false;
}


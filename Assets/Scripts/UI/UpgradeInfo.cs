using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class UpgradeInfo
{
    public string Name;
    public float Cost;
    public ulong UnlockThreshhold;
    public string UnlockMessage;
    public bool HasRun = false;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugAIMover : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _nav;

    void Start()
    {
        _nav.SetDestination(new Vector3(60, 0, 60));
    }
}

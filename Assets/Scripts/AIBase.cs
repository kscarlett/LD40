using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Collider))]
public class AIBase : MonoBehaviour
{
    [SerializeField] private bool _animate;

    //TODO: inject
    private Transform _targetTransform;

    private NavMeshAgent _nav;
    private Animator _anim;
    private DateTimeOffset _lastAdded;

    void Start () {
	    if (_animate)
            _anim = GetComponentInChildren<Animator>();

	    _nav = GetComponent<NavMeshAgent>();
	    _targetTransform = GameObject.FindGameObjectWithTag("Player").transform;

	    this.UpdateAsObservable()
	        .Timestamp()
            .Where(x => !(_animate && _anim.GetBool("Attacking")))
	        .Where(x => x.Timestamp >= _lastAdded.AddSeconds(0.2))
	        .Subscribe(x =>
	        {
	            Pathfind();
	            _lastAdded = x.Timestamp;
	        });
    }

    private void Pathfind()
    {
        _nav.SetDestination(_targetTransform.position);
    }
}

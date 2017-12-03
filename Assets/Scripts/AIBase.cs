using System.Collections;
using System.Collections.Generic;
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

	// Use this for initialization
	void Start () {
	    if (_animate)
            _anim = GetComponentInChildren<Animator>();

	    _nav = GetComponent<NavMeshAgent>();
	    _targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
	    if (!(_animate && _anim.GetBool("Attacking")))
        {
            _nav.SetDestination(_targetTransform.position);
        }
	}
}

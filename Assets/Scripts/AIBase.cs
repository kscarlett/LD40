using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavMeshAgent), typeof(Collider))]
public class AIBase : MonoBehaviour, IDamageable
{
    [SerializeField] private bool _animate;

    //TODO: inject
    private CastleBehaviour _castle;
    private Transform _targetTransform;

    private GameObject _currentEnemy;
    private NavMeshAgent _nav;
    private Animator _anim;
    private DateTimeOffset _lastAdded;
    public bool IsEnemy;
    public int Damage;
    private ReactiveProperty<int> _health;
    public int StartingHealth;

    [Inject]
    private void Construct(CastleBehaviour castle)
    {
        _castle = castle;
        _targetTransform = _castle.transform;
    }

    void Start()
    {
        _health = new ReactiveProperty<int>(StartingHealth);
        if (_animate)
            _anim = GetComponentInChildren<Animator>();

        _nav = GetComponent<NavMeshAgent>();

        _health.AsObservable().Where(i => i <= 0).Subscribe(x => Destroy(gameObject));

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

    void OnCollisionEnter(Collision coll)
    {
        IDamageable damageable = coll.transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (coll.transform.tag == "Player" || coll.transform.tag == "PlayerSoldier" || coll.transform.tag == "Enemy")
            {
                if ((IsEnemy && coll.transform.tag != "Enemy") || coll.transform.tag == "Enemy")
                {
                    if (_currentEnemy == null)
                    {
                        _anim.SetBool("Attacking", true);
                        _currentEnemy = coll.gameObject;
                    }
                }
            }
        }
    }

    public void PrepareAnimationCancel()
    {
        StartCoroutine(CheckForCancel());
    }

    private IEnumerator CheckForCancel()
    {
        while (_anim.GetBool("Attacking"))
        {
            if (_currentEnemy == null)
            {
                _anim.SetBool("Attacking", false);
            }
            yield return new WaitForSeconds(0.01f);
        }

    }

    public void ApplyDamage()
    {
        if (_currentEnemy != null)
        {
            _currentEnemy.GetComponent<IDamageable>().TakeDamage(Damage);
            StopCoroutine("CheckForCancel");
        }
    }
}

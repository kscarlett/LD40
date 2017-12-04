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
    }

    void Start()
    {
        if (IsEnemy)
            _targetTransform = _castle.transform;
        _health = new ReactiveProperty<int>(StartingHealth);
        if (_animate)
            _anim = GetComponentInChildren<Animator>();

        _nav = GetComponent<NavMeshAgent>();

        _health.AsObservable().Where(i => i <= 0).Subscribe(x => Destroy(gameObject));

        this.UpdateAsObservable()
            .Timestamp()
            .Where(x => !(_animate && _anim.GetBool("Attacking")))
            .Where(x => x.Timestamp >= _lastAdded.AddSeconds(0.2))
            .Where(x => IsEnemy)
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
            if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("PlayerSoldier") || coll.transform.CompareTag("Enemy"))
            {
                if ((IsEnemy && !coll.transform.CompareTag("Enemy")) || coll.transform.CompareTag("Enemy") )
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

    public void SetTargetTransform(Transform target)
    {
        _targetTransform = target;
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

    public void TakeDamage(int damage)
    {
        _health.Value -= damage;
    }

    public class Factory : IFactory<AIBase>
    {
        private readonly DiContainer _container;
        public GameObject TargetPrefab;

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public AIBase Create()
        {
            var obj = Instantiate(TargetPrefab);
            _container.InjectGameObjectForComponent<AIBase>(obj);
            return obj.GetComponent<AIBase>();
        }
    }
}

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
    public ReactiveProperty<bool> UnderAttack { get; set; }
    [SerializeField] private bool _animate;

    private CastleBehaviour _castle;
    private Transform _targetTransform;

    public GameObject CurrentEnemy;
    private NavMeshAgent _nav;
    private Animator _anim;
    private DateTimeOffset _lastAdded;
    public bool IsEnemy;
    public int Damage;
    private ReactiveProperty<int> _health;
    public int StartingHealth;
    public Transform EnemyTransform;

    [Inject]
    private void Construct(CastleBehaviour castle)
    {
        _castle = castle;
    }

    void Start()
    {
        UnderAttack = new ReactiveProperty<bool>();
        _lastAdded = DateTimeOffset.Now;

        if (IsEnemy)
        {
            _targetTransform = _castle.transform;
        }
        else
        {
            _targetTransform = EnemyTransform;
        }

        this.UpdateAsObservable().Where(x => CurrentEnemy != null && _targetTransform != CurrentEnemy.transform).Subscribe(
            x =>
            {
                _targetTransform = CurrentEnemy.transform;
                _anim.SetBool("Attacking", true);
                GetComponent<AudioSource>().Play();
            });

        this.UpdateAsObservable().Where(x => CurrentEnemy == null && _targetTransform != _castle.transform && IsEnemy).Subscribe(
            x =>
            {
                _targetTransform = CurrentEnemy.transform;
                GetComponent<AudioSource>().Stop();
            });

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

        StartCoroutine(LateStart());
    }

    private void Pathfind()
    {
        if (!(Vector3.Distance(transform.position, _targetTransform.position) <= 1))
        {
            _nav.SetDestination(_targetTransform.position);
        }
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        if (!IsEnemy)
        {
            _targetTransform = EnemyTransform;
        }
    }

    //void OnCollisionEnter(Collision coll)
    //{
    //    IDamageable damageable = coll.transform.GetComponent<IDamageable>();
    //    Debug.Log("Collision");
    //    if (damageable != null)
    //    {
    //        Debug.Log("Damageable not null");
    //        if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("PlayerSoldier") || coll.transform.CompareTag("Enemy"))
    //        {
    //            Debug.Log("First Test");
    //            if (/*(IsEnemy && !coll.transform.CompareTag("Enemy")) || coll.transform.CompareTag("Enemy")*/ !tag.Equals(coll.transform.tag)) //This one fails
    //            {
    //                Debug.Log("Second test");
    //                if (_currentEnemy == null)
    //                {
    //                    Debug.Log("CurrentEnemy not null");
    //                    _anim.SetBool("Attacking", true);
    //                    _currentEnemy = coll.gameObject;
    //                }
    //            }
    //        }
    //    }
    //}

    void OnTriggerEnter(Collider coll)
    {
        IDamageable damageable = coll.transform.GetComponent<IDamageable>();
        Debug.Log("Collision");
        if (damageable != null)
        {
            Debug.Log("Damageable not null");
            if (coll.transform.CompareTag("Player") || coll.transform.CompareTag("PlayerSoldier") || coll.transform.CompareTag("Enemy"))
            {
                Debug.Log("First Test");
                if (!CompareTag(coll.tag)) //This one fails
                {
                    Debug.Log("Second test");
                    if (CurrentEnemy == null)
                    {
                        Debug.Log("CurrentEnemy not null");
                        _anim.SetBool("Attacking", true);
                        GetComponent<AudioSource>().Play();
                        CurrentEnemy = coll.gameObject;
                        if (damageable is AIBase)
                        {
                            ((AIBase) damageable).CurrentEnemy = gameObject;
                        }
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
            if (CurrentEnemy == null)
            {
                _anim.SetBool("Attacking", false);
            }
            yield return new WaitForSeconds(0.01f);
        }

    }

    public void ApplyDamage()
    {
        if (CurrentEnemy != null)
        {
            CurrentEnemy.GetComponent<IDamageable>().TakeDamage(Damage);
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
        public Vector3 SpawnPos;
        public Quaternion Rotation;

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public AIBase Create()
        {
            var obj = Instantiate(TargetPrefab, SpawnPos, Rotation);
            _container.InjectGameObjectForComponent<AIBase>(obj);
            return obj.GetComponent<AIBase>();
        }
    }
}

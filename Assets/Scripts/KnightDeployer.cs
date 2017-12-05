using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class KnightDeployer : MonoBehaviour
{
    [SerializeField] private GameObject _knightPrefab;
    private Ray _ray;
    private RaycastHit _hit;
    private Camera _camera;
    private Transform _castleTransform;
    private CastleBehaviour _castle;
    private AIBase.Factory _factory;

    [Inject]
    private void Construct(Transform castleTransform, CastleBehaviour castle, AIBase.Factory factory)
    {
        _castleTransform = castleTransform;
        _castle = castle;
        _factory = factory;
    }

    void Start () {
		_camera = Camera.main;
	}
	
	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
	        _ray = _camera.ScreenPointToRay(Input.mousePosition);
	        if (Physics.Raycast(_ray, out _hit))
	        {
	            if (_hit.collider.CompareTag("Enemy"))
	            {
	                SpawnKnight(_hit.transform);
	            }
	        }
	    }
	}

    private void SpawnKnight(Transform target)
    {
        Vector2 spawnPoint = GetCirclePoint(target, 10);
        if (_castle.PayGold(2))
        {
            //var knight = Instantiate(_knightPrefab, new Vector3(spawnPoint.x, 0, spawnPoint.y), Quaternion.identity);
            _factory.Rotation = Quaternion.identity;
            _factory.SpawnPos = new Vector3(spawnPoint.x, 0, spawnPoint.y);
            _factory.TargetPrefab = _knightPrefab;
            var knight = _factory.Create();
            knight.GetComponent<AIBase>().EnemyTransform = target;
        }
    }

    private Vector2 GetCirclePoint(Transform target, int radius)
    {
        var heading = target.position - _castleTransform.position;
        var normalizedDirection = heading / heading.magnitude;
        var scaledDirection = normalizedDirection * radius;
        
        return new Vector2(scaledDirection.x, scaledDirection.z);
    }
}

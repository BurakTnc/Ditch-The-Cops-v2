using System;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Managers;
using UnityEngine;

namespace _YabuGames.Scripts.Objects
{
    public class MissileScript : MonoBehaviour
    {
        [SerializeField] private Vector3 spawnOffset;
        [SerializeField] private float speed;
        [SerializeField] private float scanningRange;
        [SerializeField] private float explosionRange;
        [SerializeField] private LayerMask scanningLayer;
        [SerializeField] private AudioClip explosionSound;
        
        private Transform _player;
        private bool _hasTarget;
        private Transform _target;
        private Collider[] _scannedColliders = new Collider[10];
        private Collider[] _explodedColliders = new Collider[20];
        private float _distanceReference = 10000;

        private void Awake()
        {
            _player = GameObject.Find("Player").transform;
        }

        private void Start()
        {
            transform.position = _player.position + spawnOffset;
        }

        private void Update()
        {
            Chase();
            ScanTheArea();
        }

        private void Chase()
        {
            if (_hasTarget && !_target)
                _hasTarget = false;
            transform.position = Vector3.Lerp(transform.position, !_hasTarget ? _player.position+Vector3.up*50 : _target.position,
                speed * Time.deltaTime);
        }

        private void ScanTheArea()
        {
            if(_hasTarget)
                return;
            
            _scannedColliders = Physics.OverlapSphere(_player.position, scanningRange, scanningLayer);
            
            if(_scannedColliders.Length<1)
                return;
            
            foreach (var t in _scannedColliders)
            {
                var police = t.transform;
                var distance = Vector3.Distance(transform.position, police.position);

                if ((!(distance < _distanceReference)))
                    continue;
                _distanceReference = distance;
                _target = police;

            }

            _hasTarget = true;
        }

        private void Explode()
        {
            //police.MissileExplosion(transform.position,50);
            ShakeManager.Instance.ShakeCamera(false);
            PoolManager.Instance.GetExplosionParticle(transform.position);
            if (explosionSound)
            {
                AudioSource.PlayClipAtPoint(explosionSound,_player.position);
            }
            
            _explodedColliders = Physics.OverlapSphere(transform.position, explosionRange, scanningLayer);
            
            if(_explodedColliders.Length<1)
                return;
            
            foreach (var coll in _explodedColliders)
            {
                coll.GetComponent<PoliceCarController>().MissileExplosion(transform.position,100);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PoliceCarController police))
            {
                Explode();
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color=Color.red;
            Gizmos.DrawSphere(transform.position, scanningRange);
        }
    }
}
using System;
using _YabuGames.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.GlobalIllumination;

namespace _YabuGames.Scripts.Controllers
{
    public class TurretController : MonoBehaviour
    {
        [SerializeField] private bool hasCannon;
        [SerializeField] private GameObject turret;
        [SerializeField] private Transform shootingPos;
        [SerializeField] private float shootPeriod;
        [SerializeField] private float shootingRange;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject shootingEffect;

            private bool _isShooting;
            private Transform _player;
            private float _shootDelayer;
            private AudioSource _source;
            private bool _isEliminated;

            private void Awake()
            {
                _player = GameObject.Find("Player").transform;
                _source = GetComponent<AudioSource>();
                _shootDelayer = shootPeriod;
            }

            private void Update()
            {
                if(_isEliminated)
                    return;
                
                LookToPlayer();
                if (!hasCannon)
                    return;
                ScanTheArea();
                BeginShoot();
            }

            private void ScanTheArea()
            {
                var colliders = new Collider[1];
               var i= Physics.OverlapSphereNonAlloc(transform.parent.position, shootingRange, colliders,layerMask);

               _isShooting = i>0;

            }

            private void BeginShoot()
            {
                if (_isShooting)
                {
                    _shootDelayer -= Time.deltaTime;
                    _shootDelayer = Mathf.Clamp(_shootDelayer, 0, shootPeriod);
                }

                var canShoot = _isShooting && _shootDelayer <= 0;
                
                if(!canShoot)
                    return;
                Shoot();
                

                void Shoot()
                {
                    if(!hasCannon)
                        return;
                    
                    _source.Play();
                    _shootDelayer += shootPeriod;
                    var targetPos = _player.position;
                    var distance = Vector3.Distance(_player.position, transform.position);
                    var cannon = Instantiate(Resources.Load<GameObject>("Spawnables/Cannon"));
                    cannon.transform.position = shootingPos.position;
                    cannon.transform.DOJump(targetPos, .2f, 1, distance/70).OnComplete(Explode);

                    void Explode()
                    {
                        var explosionColliders = new Collider[1];
                        
                        PoolManager.Instance.GetHitParticle(cannon.transform.position);

                        explosionColliders = Physics.OverlapSphere(cannon.transform.position, 3, layerMask);
                        Destroy(cannon);
                        
                        if(explosionColliders.Length<1)
                            return;
                        explosionColliders[0].GetComponent<PlayerPhysicsController>().CannonExplosion(transform.position);

                    }

                }
            }

            private void LookToPlayer()
            {
                turret.transform.LookAt(_player,Vector3.up);
            }

            private void OnDrawGizmos()
            {
                Gizmos.color=Color.red;
                Gizmos.DrawWireSphere(transform.position,shootingRange);
                
            }

            public void Eliminate()
            {
                _isEliminated = true;
                if(!shootingEffect)
                    return;
                shootingEffect.SetActive(false);
            }
        }
}
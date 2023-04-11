using System;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class TurretController : MonoBehaviour
        {
            [SerializeField] private bool hasCannon;
            [SerializeField] private GameObject turret;
            [SerializeField] private Transform shootingPos;
            [SerializeField] private GameObject shootParticle;
            [SerializeField] private float shootPeriod;
            [SerializeField] private float shootingRange;
            [SerializeField] private LayerMask layerMask;

            private bool _isShooting;
            private Transform _player;
            private float _shootDelayer;

            private void Awake()
            {
                _player = GameObject.Find("Player").transform;
                _shootDelayer = shootPeriod;
            }

            private void Update()
            {
                LookToPlayer();
                if(!hasCannon)
                    return;
                ScanTheArea();
                BeginShoot();
            }

            private void ScanTheArea()
            {
                var colliders = new Collider[30];
               var i= Physics.OverlapSphereNonAlloc(transform.root.position, shootingRange, colliders,layerMask);

               if (i>0)
               {
                   _isShooting = true;
               }

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
                    Debug.Log("shoot");
                    shootParticle.SetActive(true);
                    
                    if(!hasCannon)
                        return;
                    _shootDelayer += shootPeriod;
                    var targetPos = _player.position;
                    var cannon = Instantiate(Resources.Load<GameObject>("Spawnables/Cannon"));
                    cannon.transform.position = shootingPos.position;
                    cannon.transform.DOJump(targetPos, .2f, 1, 1);

                }
            }

            private void LookToPlayer()
            {
                turret.transform.LookAt(_player,Vector3.up);
            }

            private void OnDrawGizmos()
            {
                Gizmos.color=Color.red;
                Gizmos.DrawSphere(transform.position,shootingRange);
            }
        }
}
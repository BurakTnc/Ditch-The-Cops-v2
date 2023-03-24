using System;
using _YabuGames.Scripts.Managers;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        [Header("Car Crash Physics")] 
        [SerializeField] private float explosionRadius;
        [SerializeField] private float explosionForce;
        [SerializeField] private float upwardsModifier;
        [SerializeField] private ForceMode forceMode;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = transform.root.GetComponent<Rigidbody>();
        }


        public void PoliceCollision(Vector3 impactPoint)
        {
            _rb.AddExplosionForce(explosionForce, impactPoint, explosionRadius, upwardsModifier, forceMode);
            PoolManager.Instance.GetHitParticle(impactPoint);
        }


    }
    
}
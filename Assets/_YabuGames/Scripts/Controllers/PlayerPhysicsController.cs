using System;
using System.Security.Cryptography.X509Certificates;
using _YabuGames.Scripts.Managers;
using UnityEditor;
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
        [Header("Obstacle Crash Physics")] 
        [SerializeField] private float obstacleExplosionRadius;
        [SerializeField] private float obstacleExplosionForce;
        [SerializeField] private float obstacleUpwardsModifier;
        [SerializeField] private ForceMode obstacleForceMode;

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

        public void ObstacleCollision(Vector3 impactPoint,Rigidbody obstacleRb)
        {
            obstacleRb.useGravity = true;
            obstacleRb.constraints = RigidbodyConstraints.None;
            obstacleRb.AddExplosionForce(obstacleExplosionForce, impactPoint, obstacleExplosionRadius, obstacleUpwardsModifier,
                obstacleForceMode);
            PoolManager.Instance.GetHitParticle(impactPoint);
            Destroy(obstacleRb.gameObject,3);
        }

        public void Eliminate()
        {
            _rb.AddExplosionForce(explosionForce+300, transform.position, explosionRadius+2, upwardsModifier+2, forceMode);
            PoolManager.Instance.GetSmokeParticle(transform.position);
        }


    }
    
}
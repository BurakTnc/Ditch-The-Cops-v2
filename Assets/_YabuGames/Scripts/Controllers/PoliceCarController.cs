using System;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PoliceCarController : MonoBehaviour
    {
        [SerializeField] private float explosionForce, explosionRadius, upwardsModifier;
        [SerializeField] private ForceMode forceMode;
        
        private Rigidbody _rb;
        private PoliceAIController _aiController;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _aiController = GetComponent<PoliceAIController>();
        }

        public void Eliminate(Vector3 impactPoint)
        {
            _rb.AddExplosionForce(explosionForce,impactPoint,explosionRadius,upwardsModifier,forceMode);
            _aiController.StopChasing();
        }
    }
}

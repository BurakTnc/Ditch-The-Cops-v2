using System;
using _YabuGames.Scripts.Managers;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class PoliceCarController : MonoBehaviour
    {
        [Header("Physics")] 
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionRadius;
        [SerializeField] private float upwardsModifier;
        [SerializeField] private ForceMode forceMode;

        [Header("Sound Effects")] 
        [SerializeField] private AudioClip explosionSound;

        private BoxCollider _collider;
        private AudioSource _source;
        private Rigidbody _rb;
        private PoliceAIController _aiController;
        private bool _isEliminated;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _aiController = GetComponent<PoliceAIController>();
            _source = GetComponent<AudioSource>();
            _collider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            _source.Play();
        }

        public void Eliminate(Vector3 impactPoint)
        {
            if(_isEliminated)
                return;
            _isEliminated = true;
            PoolManager.Instance.GetEliminatedParticle(transform.position+Vector3.up*3);
            Invoke(nameof(SetSmokeParticle),3);
            _rb.AddExplosionForce(explosionForce,impactPoint,explosionRadius,upwardsModifier,forceMode);
            _aiController.StopChasing();
            _source.DOPitch(0, 2f).SetEase(Ease.InSine);
            if (!explosionSound)
            {
                Debug.LogWarning("There Is No Explosion Sound Attached!");
                return;
            }
            AudioSource.PlayClipAtPoint(explosionSound, transform.position + Vector3.up);
            
        }

        private void SetSmokeParticle()
        {
            PoolManager.Instance.GetSmokeParticle(transform.position);
        }

        private void OnBecameInvisible()
        {
            _collider.enabled = false;
            _source.Pause();
            if (_isEliminated)
            {
                Destroy(gameObject);
            }
        }

        private void OnBecameVisible()
        {
            _collider.enabled = true;
            _source.Play();
        }
    }
}

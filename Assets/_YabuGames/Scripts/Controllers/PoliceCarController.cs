using System;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class PoliceCarController : MonoBehaviour
    {
        [Header("Explosion Physics")]
        [SerializeField] private float explosionForce, explosionRadius, upwardsModifier;
        [SerializeField] private ForceMode forceMode;

        [Header("Sound Effects")] 
        [SerializeField] private AudioClip explosionSound;

        private AudioSource _source;
        private Rigidbody _rb;
        private PoliceAIController _aiController;
        private bool _isEliminated;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _aiController = GetComponent<PoliceAIController>();
            _source = GetComponent<AudioSource>();
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
    }
}

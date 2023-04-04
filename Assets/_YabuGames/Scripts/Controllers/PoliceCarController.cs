using System;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class PoliceCarController : MonoBehaviour
    {
        [HideInInspector] public int damage;
        
        [Header("Physics")] 
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionRadius;
        [SerializeField] private float upwardsModifier;
        [SerializeField] private ForceMode forceMode;

        [Header("Sound Effects")] 
        [SerializeField] private AudioClip explosionSound;
        [Space]
        [SerializeField] private PoliceSpecs specs;

        [Space]
        private BoxCollider _collider;
        private AudioSource _source;
        private Rigidbody _rb;
        private PoliceAIController _aiController;
        private bool _isEliminated;
        private NavMeshAgent _agent;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _aiController = GetComponent<PoliceAIController>();
            _source = GetComponent<AudioSource>();
            _collider = GetComponent<BoxCollider>();
            _agent = GetComponent<NavMeshAgent>();
        }

        #region Subscribtions

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            LevelSignals.Instance.OnSkillPanel += Mute;
            CoreGameSignals.Instance.OnLevelFail += LevelEnd;
            CoreGameSignals.Instance.OnLevelWin += LevelEnd;
        }

        private void UnSubscribe()
        {
            LevelSignals.Instance.OnSkillPanel -= Mute;
            CoreGameSignals.Instance.OnLevelFail -= LevelEnd;
            CoreGameSignals.Instance.OnLevelWin -= LevelEnd;
        }
        #endregion

        private void Start()
        {
           // _source.Play();
            damage = specs.damage;
        }

        private void LevelEnd()
        {
            Mute(true);
        }
        private void Mute(bool isMuted)
        {
            if (isMuted)
            {
                _source.Stop();
            }
            else
            {
                _source.Play();
            }
        }

        public void Eliminate(Vector3 impactPoint)
        {
            if(_isEliminated)
                return;
            HapticManager.Instance.PlayRigidHaptic();
            ShakeManager.Instance.ShakeCamera(true);
            LevelSignals.Instance.OnPoliceEliminated?.Invoke();
            _isEliminated = true;
            PoolManager.Instance.GetEliminatedParticle(transform.position+Vector3.up*3);
            Invoke(nameof(SetSmokeParticle),2);
            _rb.AddExplosionForce(explosionForce,impactPoint,explosionRadius,upwardsModifier,forceMode);
            _aiController.StopChasing();
            _source.DOPitch(0, 2f).SetEase(Ease.InSine);
            if (!explosionSound)
            {
                Debug.LogWarning("There Is No Explosion Sound Attached!");
                return;
            }
            AudioSource.PlayClipAtPoint(explosionSound, transform.position + Vector3.up);
            Destroy(gameObject,5);
            
        }

        public void MissileExplosion(Vector3 impactPoint,float radius)
        {
            if(_isEliminated)
                return;
            HapticManager.Instance.PlayRigidHaptic();
            LevelSignals.Instance.OnPoliceEliminated?.Invoke();
            _isEliminated = true;
            PoolManager.Instance.GetEliminatedParticle(transform.position+Vector3.up*3);
            Invoke(nameof(SetSmokeParticle),2);
            _rb.AddExplosionForce(explosionForce+50,impactPoint,radius,upwardsModifier+2,forceMode);
            _aiController.StopChasing();
            _source.DOPitch(0, 2f).SetEase(Ease.InSine);
            if (!explosionSound)
            {
                Debug.LogWarning("There Is No Explosion Sound Attached!");
                return;
            }
            AudioSource.PlayClipAtPoint(explosionSound, transform.position + Vector3.up);
            Destroy(gameObject,5);
        }

        private void SetSmokeParticle()
        {
            PoolManager.Instance.GetSmokeParticle(transform.position);
        }

        private void OnBecameInvisible()
        {
            _agent.speed *= 1.4f;
            _collider.enabled = false;
            Mute(true);
            if (_isEliminated)
            {
                Destroy(gameObject);
            }
        }

        private void OnBecameVisible()
        {
            _agent.speed /= 1.4f;
            _collider.enabled = true;
            Mute(false);
        }
    }
}

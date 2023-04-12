using System;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

namespace _YabuGames.Scripts.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class PoliceCarController : MonoBehaviour
    {
        [HideInInspector] public bool onOil;
        [HideInInspector] public bool isArmored;
        
        [SerializeField] private TurretController turretController;
        
        [Header("Physics")] 
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionRadius;
        [SerializeField] private float upwardsModifier;
        [SerializeField] private ForceMode forceMode;

        [Header("Sound Effects")] 
        [SerializeField] private AudioClip explosionSound;
        [Space]
        [SerializeField] private PoliceSpecs specs;
        [SerializeField] private GameObject siren;

        [Space]
        private int _damage;
        private BoxCollider _collider;
        private AudioSource _source;
        private Rigidbody _rb;
        private PoliceAIController _aiController;
        private bool _isEliminated;
        private NavMeshAgent _agent;
        private Vector3 _onOilHeading;
        private bool _hasSiren;

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
            LevelSignals.Instance.OnPlayerDestroyed += Stop;
            LevelSignals.Instance.OnRevive += Continue;
            CoreGameSignals.Instance.OnLevelFail += LevelEnd;
            CoreGameSignals.Instance.OnLevelWin += LevelEnd;
        }

        private void UnSubscribe()
        {
            LevelSignals.Instance.OnPlayerDestroyed -= Stop;
            LevelSignals.Instance.OnRevive -= Continue;
            LevelSignals.Instance.OnSkillPanel -= Mute;
            CoreGameSignals.Instance.OnLevelFail -= LevelEnd;
            CoreGameSignals.Instance.OnLevelWin -= LevelEnd;
        }
        #endregion

        private void Start()
        {
           // _source.Play();
            _damage = specs.damage;
            isArmored = _damage >= 40;
            _hasSiren = specs.hasSiren;
        }

        private void Update()
        {
            if(!onOil)
                return;
            _rb.velocity = _onOilHeading * 16;
        }

        public void Stop()
        {
            _aiController.StopChasing();
        }

        public void Continue()
        {
            if(_isEliminated)
                return;
            _aiController.ContinueChasing();
            Mute(false);
        }

        private void LevelEnd()
        {
            Mute(true);
        }
        private void Mute(bool isMuted)
        {
            if(!_source)
                return;
            
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
            switch (_hasSiren)
            {
                case true:
                    siren.SetActive(false);
                    break;
                case false:
                    turretController.Eliminate();
                    break;
            }

            onOil = false;
            GameManager.Instance.IncreaseXp(_damage);
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
            if (!_isEliminated)
            {
                switch (_hasSiren)
                {
                    case true:
                        siren.SetActive(false);
                        break;
                    case false:
                        turretController.Eliminate();
                        break;
                }

                onOil = false;
                GameManager.Instance.IncreaseXp(_damage);
                HapticManager.Instance.PlayHeavyHaptic();
                LevelSignals.Instance.OnPoliceEliminated?.Invoke();
                _isEliminated = true;
                PoolManager.Instance.GetEliminatedParticle(transform.position+Vector3.up*3);
                Invoke(nameof(SetSmokeParticle),2);
                if (!explosionSound)
                {
                    Debug.LogWarning("There Is No Explosion Sound Attached!");
                    return;
                }
                AudioSource.PlayClipAtPoint(explosionSound, transform.position + Vector3.up);
                _aiController.StopChasing();
                _source.DOPitch(0, 2f).SetEase(Ease.InSine);
                Destroy(gameObject,5);
            }
            _rb.AddExplosionForce(explosionForce+50,impactPoint,radius,upwardsModifier+2,forceMode);
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

        public void OnOilTrap()
        {
            if(_isEliminated)
                return;
            _aiController.StopChasing();
            onOil = true;
            _onOilHeading = transform.forward;
            transform.DOShakeRotation(2, Vector3.up * 70, 1, 100, true)
                .OnComplete(() => Eliminate(transform.position - Vector3.up));
            
        }

        public int GetDamage()
        {
            return _isEliminated ? 0 : _damage;
        }
        
    }
}

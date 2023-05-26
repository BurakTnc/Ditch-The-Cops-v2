using System;
using System.Collections;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _YabuGames.Scripts.Controllers
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] private CarSpecs carSpecs;
        [SerializeField] private TrailRenderer[] trails = new TrailRenderer[2];
        [SerializeField] private ParticleSystem[] smokeParticle = new ParticleSystem[2];
        [SerializeField] private Transform mesh;

        private VehicleAudioController _audioController;
        private Rigidbody _rb;
        private PlayerInputController _input;
        private Vector3 _direction;
        private float _angularSpeed, _forwardSpeed, _skidLimit, _topSpeed;
        private bool _isAccelerating, _onTrap;

        private void Awake()
        {
            var root = transform.root;
            _input = root.GetComponent<PlayerInputController>();
            _rb = root.GetComponent<Rigidbody>();
            _audioController = root.GetComponent<VehicleAudioController>();
            
            GetCarSpecs();
        }

        #region Subscribtons
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Start()
        {
            _audioController.EndSkidding();
        }

        private void Subscribe()
        {
            InputSignals.Instance.OnTouch += CheckTheTouch;
            InputSignals.Instance.CanMove += AllowToMove;
            SkillSignals.Instance.OnNitro += GainSpeed;
        }

        private void UnSubscribe()
        {
            InputSignals.Instance.OnTouch -= CheckTheTouch;
            InputSignals.Instance.CanMove -= AllowToMove;
            SkillSignals.Instance.OnNitro -= GainSpeed;
        }
        #endregion

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var canMove =  !_onTrap;
            var direction = _input.direction;
            
            if (!canMove)
                return;
            
            if (_rb.velocity.magnitude < _topSpeed)
            {
                _rb.AddForce(transform.forward * (_forwardSpeed * Time.fixedDeltaTime), ForceMode.Acceleration);
            }

            if (direction != Vector3.zero) 
            {var difference = Mathf.Abs(direction.magnitude);
                Quaternion desiredRotation= Quaternion.Slerp(mesh.rotation, Quaternion.LookRotation(direction), (_angularSpeed-difference*10) * Time.deltaTime);
                Vector3 rot = mesh.rotation.eulerAngles;
                Vector3 des = desiredRotation.eulerAngles;
                
                Debug.Log(direction.magnitude);
                CheckTheSkid((des-rot).magnitude);
                // if (difference<.08f)
                // {
                //     mesh.rotation=Quaternion.Slerp(mesh.rotation, Quaternion.LookRotation(direction), 1* Time.deltaTime);
                //     Debug.LogWarning("detected");
                // }
                //
                // else
                // {
                //     mesh.rotation = desiredRotation;
                // }
                //Quaternion calculatedRotation= Quaternion.Slerp(mesh.rotation, desiredRotation, (_angularSpeed-difference*10) * Time.deltaTime);
                mesh.rotation = desiredRotation;
            }

            if (direction != Vector3.zero)
                return;
            foreach (var t in trails)
            {
                t.emitting = false;
            }

        }

        private void CheckTheSkid(float magnitude)
        {
            if(!_isAccelerating)
                return;
            if (magnitude >= _skidLimit)
            {
                _audioController.BeginSkidding();
                for (var i = 0; i < trails.Length; i++)
                {
                    trails[i].emitting = true;
                    smokeParticle[i].Play();            
                }
            }
            else
            {
                EndSkidding();
            }
        }

        private void EndSkidding()
        {
            _audioController.EndSkidding();
            foreach (var t in trails)
            {
                t.emitting = false;
            }

            foreach (var t in smokeParticle)
            {
                t.Stop();
            }
        }

        private void GetCarSpecs()
        {
            _angularSpeed = carSpecs.angularSpeed;
            _forwardSpeed = carSpecs.forwardSpeed;
            _skidLimit = carSpecs.skidLimit;
            _topSpeed = carSpecs.topSpeed;
        }

        #region Subsribed Functions

        private IEnumerator SpeedRoutine(float duration)
        {
            _topSpeed += _forwardSpeed * .1f;
            yield return new WaitForSeconds(duration);
            _topSpeed -= _forwardSpeed * .1f;
        }
        private void GainSpeed(float duration)
        {
            StartCoroutine(SpeedRoutine(duration));
        }
        private void CheckTheTouch(bool onTouch)
        {
            _isAccelerating = onTouch;
            if (!onTouch)
            {
                EndSkidding();
            }
        }

        private void AllowToMove(bool isBlocked)
        {
            _onTrap = isBlocked; 
        }
        #endregion
    }
}
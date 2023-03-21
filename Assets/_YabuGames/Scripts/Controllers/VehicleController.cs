using System;
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
        
        private void Subscribe()
        {
            InputSignals.Instance.OnTouch += CheckTheTouch;
            InputSignals.Instance.CanMove += AllowToMove;
        }

        private void UnSubscribe()
        {
            InputSignals.Instance.OnTouch -= CheckTheTouch;
            InputSignals.Instance.CanMove -= AllowToMove;
        }
        #endregion

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var canMove = _isAccelerating && !_onTrap;
            var direction = _input.direction;
            
            if (!canMove)
                return;
            
            if (_rb.velocity.magnitude < _topSpeed)
            {
                _rb.AddForce(transform.forward * (_forwardSpeed * Time.fixedDeltaTime), ForceMode.Acceleration);
            }

            if (direction != Vector3.zero) 
            {
                Quaternion desiredRotation= Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _angularSpeed * Time.deltaTime);
                Vector3 rot = transform.root.rotation.eulerAngles;
                Vector3 des = desiredRotation.eulerAngles;
                CheckTheSkid((des-rot).magnitude);
                transform.root.rotation = desiredRotation;
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
            if (magnitude >= _skidLimit)
            {
                for (var i = 0; i < trails.Length; i++)
                {
                    trails[i].emitting = true;
                    smokeParticle[i].Play();            
                }
            }
            else
            {
                foreach (var t in trails)
                {
                    t.emitting = false;
                }

                foreach (var t in smokeParticle)
                {
                    t.Pause();
                }
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
        private void CheckTheTouch(bool onTouch)
        {
            _isAccelerating = onTouch;
        }

        private void AllowToMove(bool isBlocked)
        {
            _onTrap = isBlocked; 
        }
        #endregion
    }
}
using System;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.AI;

namespace _YabuGames.Scripts.Controllers
{
    public class PoliceAIController : MonoBehaviour
    {
        [SerializeField] private PoliceSpecs specs;

        private NavMeshAgent _agent;
        private Transform _player;
        private bool _onChase;
        private float _rookieLevel;
        private float _delayer;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _player = GameObject.Find("Player").transform;
        }

        private void OnEnable()
        {
            LevelSignals.Instance.OnPlayerDestroyed += StopChasing;
        }

        private void OnDisable()
        {
            LevelSignals.Instance.OnPlayerDestroyed -= StopChasing;
        }

        private void Start()
        {
            _agent.speed = specs.speed;
            _agent.angularSpeed = specs.angularSpeed;
            _agent.acceleration = specs.acceleration;
            _rookieLevel = specs.chaseSkill;
            _onChase = true;
            _agent.speed *= 1.4f;
        }


        private void SetTarget()
        {
            _delayer -= Time.deltaTime;
            _delayer = Mathf.Clamp(_delayer, 0, _rookieLevel);
            if(!_onChase)
                return;
            if(_delayer>0)
                return;
            _delayer += _rookieLevel;
            if (_agent.isActiveAndEnabled)
            {
                _agent.SetDestination(_player.position);
            }
        }
        
        void Update()
        {
            SetTarget();
        }

        public void StopChasing()
        {
            if(!_agent.isActiveAndEnabled)
                return;
            _onChase = false;
            _agent.isStopped = true;
            _agent.enabled = false;
        }

        public void EnableAgent()
        {
            _agent.enabled = true;
        }

        public void ContinueChasing()
        {
            _agent.enabled = true;
            
            if (_agent.isActiveAndEnabled)
            {
                _onChase = true;
                _agent.isStopped = false;
                
            }
        }
    }
}

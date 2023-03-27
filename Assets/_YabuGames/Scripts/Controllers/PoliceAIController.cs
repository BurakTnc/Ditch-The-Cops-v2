using System;
using _YabuGames.Scripts.ScriptableObjects;
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

        private void Start()
        {
            _agent.speed = specs.speed;
            _agent.angularSpeed = specs.angularSpeed;
            _agent.acceleration = specs.acceleration;
            _rookieLevel = specs.chaseSkill;
            _onChase = true;
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
            _agent.SetDestination(_player.position);
        }
        
        void Update()
        {
            SetTarget();
        }

        public void StopChasing()
        {
            _onChase = false;
            _agent.isStopped = true;
            _agent.enabled = false;
        }
    }
}

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
            _onChase = true;
        }


        private void SetTarget()
        {
            if(!_onChase)
                return;
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

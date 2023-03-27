using System;
using _YabuGames.Scripts.Managers;
using UnityEngine;

namespace _YabuGames.Scripts.Spawners
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private LayerMask layer;
        [SerializeField] private float scanRadius = 5f;

        private SpawnManager _manager;
        private Collider[] _blockingColliders = new Collider[3];
        private int _blockingObjectsCount;
        private bool _canSpawn = false;

        private void Awake()
        {
            _manager = transform.parent.GetComponent<SpawnManager>();
        }

        private void Update()
        {
            ScanArea();
        }

        public void ReadyToSpawn(string item,bool isScanNeeded)
        {
            
            if (isScanNeeded)
            {
                if (!_canSpawn) return;
                Spawn(item);
            }
            else 
            {
                Spawn(item);
            }
        }

        private void Spawn(string item)
        {
            var temp = Instantiate(Resources.Load<GameObject>(path: $"Spawnables/{item}"));
            temp.transform.position = transform.position;
            _manager.SpawnFeedBack();
        }

        private void ScanArea()
        {
            _blockingObjectsCount =
                Physics.OverlapSphereNonAlloc(transform.position, scanRadius, _blockingColliders, layer);
            _canSpawn = _blockingObjectsCount < 1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color=Color.green;
            Gizmos.DrawWireSphere(transform.position,scanRadius);
        }


        public bool CanSpawn()
        {
            return _canSpawn;
        }
    }
}
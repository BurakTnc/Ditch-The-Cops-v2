using System;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Managers;
using UnityEngine;

namespace _YabuGames.Scripts.Spawners
{
    public class Spawner : MonoBehaviour
    {
        [HideInInspector] public bool hasCollectible;
        
        [SerializeField] private LayerMask layer;
        [SerializeField] private float scanRadius = 5f;

        private SpawnManager _manager;
        private Collider[] _blockingColliders = new Collider[3];
        private int _blockingObjectsCount;
        private bool _canSpawn;
        

        private void Awake()
        {
            _manager = transform.parent.GetComponent<SpawnManager>();
        }

        private void Update()
        {
            ScanArea();
        }

        public void ReadyToSpawn(string item,bool isScanNeeded,bool isCollectible)
        {
            
            if (isScanNeeded)
            {
                if (!_canSpawn) return;
                Spawn(item,isCollectible);
            }
            else 
            {
                Spawn(item,isCollectible);
            }
        }

        private void Spawn(string item,bool isCollectible)
        {
            var temp = Instantiate(Resources.Load<GameObject>(path: $"Spawnables/{item}"),transform);
            temp.transform.position = transform.position;

            if (temp.TryGetComponent(out PoliceAIController aiController))
            {
                aiController.EnableAgent();
            }
            _manager.SpawnFeedBack();
            if(!isCollectible)
                return;
            temp.transform.position+=Vector3.up*2;
            hasCollectible = true;
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

        public bool CanCollectibleSpawn()
        {
            return !hasCollectible;
        }
    }
}
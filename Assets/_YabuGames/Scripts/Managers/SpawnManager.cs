using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Spawners;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        
        [HideInInspector] public int currentCollectibleCount;
        [HideInInspector] public float collectibleDelayer;
        
        [SerializeField] private bool isScanNeeded;
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();
        [SerializeField] private float spawnCooldown = 1f;
        [SerializeField] private float collectibleSpawnCooldown;
        [SerializeField] private int maxCollectibleCount;
        
        private float _time;
        private int _queuedItems;

        private void Awake()
        {
            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void SpawnCollectible()
        {
            collectibleDelayer = collectibleSpawnCooldown;
            currentCollectibleCount++;
            
            var r = Random.Range(0, spawners.Count);
            if (isScanNeeded)
            {
                if(!spawners[r].CanCollectibleSpawn()) return;
            }
            
            var item = "coin";
            var chosenItem = Random.Range(0, 2);

            switch (chosenItem)
            {
                case 0:
                    item = "Coin3D";
                    break;
                case 1:
                    item = "Health";
                    break;
                default:
                    break;
            }

            spawners[r].ReadyToSpawn(item,isScanNeeded,true);
        }
        private void SpawnRandom()
        {
            var r = Random.Range(0, spawners.Count);
            if (isScanNeeded)
            {
                if(!spawners[r].CanSpawn()) return;
            }

            var wantedLevel = LevelManager.Instance.GetWantedLevel();
            var chosenPrefab = Random.Range(1, wantedLevel+1);
            spawners[r].ReadyToSpawn($"Police-{chosenPrefab}",isScanNeeded,false);
        }

        private void Update()
        {
            CarSpawnProcess();

            collectibleDelayer -= Time.deltaTime;
            collectibleDelayer = Mathf.Clamp(collectibleDelayer, 0, collectibleSpawnCooldown);
            if (collectibleDelayer <= 0)
            {
                if (currentCollectibleCount >= maxCollectibleCount) 
                    return;
                SpawnCollectible();
            }
        }

        private void CarSpawnProcess()
        {
            _time -= Time.deltaTime;
            _time = Math.Clamp(_time, 0, spawnCooldown);

            if (_time <= 0)
            {
                _queuedItems++;
            }

            if (_queuedItems > 0)
            {
                SpawnRandom();
            }
        }

        public void SpawnFeedBack()
        {
            _queuedItems--;
            _queuedItems = Mathf.Clamp(_queuedItems, 0, 10); 
            _time += spawnCooldown;
            
        }
    }
}

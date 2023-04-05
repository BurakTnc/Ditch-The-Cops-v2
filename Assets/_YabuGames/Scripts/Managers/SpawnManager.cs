using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Spawners;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [HideInInspector] public int currentCollectibleCount;
        
        [SerializeField] private bool isScanNeeded;
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();
        [SerializeField] private float spawnCooldown = 1f; 
        [SerializeField] private string spawnItemName;
        [SerializeField] private float collectibleSpawnCooldown;
        [SerializeField] private int maxCollectibleCount;
        
        private float _time;
        private float _collectibleDelayer;
        
        private int _queuedItems;
        


        private void SpawnCollectible()
        {
            _collectibleDelayer = collectibleSpawnCooldown;
            currentCollectibleCount++;
            
            var r = Random.Range(0, spawners.Count);
            if (isScanNeeded)
            {
                if(!spawners[r].CanSpawn()) return;
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

            spawners[r].ReadyToSpawn(item,isScanNeeded);
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
            spawners[r].ReadyToSpawn($"Police-{chosenPrefab}",isScanNeeded);
        }

        private void Update()
        {
            CarSpawnProcess();

            _collectibleDelayer -= Time.deltaTime;
            _collectibleDelayer = Mathf.Clamp(_collectibleDelayer, 0, collectibleSpawnCooldown);
            if (_collectibleDelayer <= 0)
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

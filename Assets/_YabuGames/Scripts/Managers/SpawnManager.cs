using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Objects;
using _YabuGames.Scripts.Spawners;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private bool isScanNeeded;
        [SerializeField] private List<Spawner> spawners = new List<Spawner>();
        [SerializeField] private float spawnCooldown = 1f; 
        [SerializeField] private string spawnItemName;

        private float _time;
        private int _queuedItems;

        private void SpawnRandom()
        {
            var r = Random.Range(0, spawners.Count);
            if (isScanNeeded)
            {
                if(!spawners[r].CanSpawn()) return;
            }

            var wantedLevel = LevelManager.Instance.GetWantedLevel();
            var chosenPrefab = Random.Range(1, wantedLevel);
            spawners[r].ReadyToSpawn($"Police-{chosenPrefab}",isScanNeeded);
        }

        private void Update()
        {
            _time -= Time.deltaTime;
            _time = Math.Clamp(_time, 0, spawnCooldown);
            
            if(_time <= 0)
            {
                _queuedItems++;
            }
            if (_queuedItems > 0 ) 
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

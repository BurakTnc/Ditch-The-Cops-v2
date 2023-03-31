using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance;
        
        public float missileSpawnPeriod;

        // 0-Missile / 1-God Mode / 2-Nitro / 3-Max HP / 4-Reduce Damage / 5-Heal / 6-Bonus Earning 
        private readonly bool[] _skillIDList = new bool[7];
        private Transform _player;
        private float _missileDelayer;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        #region Subscribtions

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
            LevelSignals.Instance.OnSkillActive += OpenASkill;
        }

        private void UnSubscribe()
        {
            LevelSignals.Instance.OnSkillActive -= OpenASkill;
        }

        #endregion

        private void Update()
        {
            ApplyMissileSkill();
        }

        private void OpenASkill(int skillID)
        {
            _skillIDList[skillID] = true;
        }

        #region Missile
        private void ApplyMissileSkill()
        {
            _missileDelayer -= Time.deltaTime;
            _missileDelayer = Mathf.Clamp(_missileDelayer, 0, missileSpawnPeriod);
            
            var isAble = _skillIDList[0] && _missileDelayer <= 0;
            
            if(!isAble)
                return;
            SpawnAMissile(); 
            
            void SpawnAMissile()
            {
                _missileDelayer += missileSpawnPeriod;
                Instantiate(Resources.Load<GameObject>("Spawnables/Missile"));
            }
        }
        #endregion
        

        
    }
}
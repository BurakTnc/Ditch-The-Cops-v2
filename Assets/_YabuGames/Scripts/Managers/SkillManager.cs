using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class SkillManager : MonoBehaviour
    {
        //skill timeri resetlenmiyor
        
        public static SkillManager Instance;
        
       [HideInInspector] public readonly int[] ChosenSkills = new int[7];
        
        public float missileSpawnPeriod;
        public float godModePeriod;
        public float nitroPeriod;
        public float healPeriod;

        [SerializeField] private float godModeDuration;
        [SerializeField] private float nitroDuration;
        [SerializeField] private float healDuration;
        
        // 0-Missile / 1-God Mode / 2-Nitro / 3-Max HP / 4-Reduce Damage / 5-Heal / 6-Bonus Earning 
        private readonly bool[] _skillIDList = new bool[7];
        
        private Transform _player;
        private float _missileDelayer, _godModeDelayer, _nitroDelayer, _healDelayer;

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
            ApplyGodMode();
            ApplyNitro();
            ApplyMaxHp();
            ApplyReduceDamage();
            ApplyHeal();
        }

        private void OpenASkill(int skillID)
        {
            SetChosenSkills(skillID);
            _skillIDList[skillID] = true;
            switch (skillID)
            {
                case 0:
                    _missileDelayer = 0;
                    break;
                case 1:
                    _godModeDelayer = 0;
                    break;
                case 2:
                    _nitroDelayer = 0;
                    break;
                case 5:
                    _healDelayer = 0;
                    break;
                default:
                    break;
            }
        }

        private void ApplyHeal()
        {
            _healDelayer -= Time.deltaTime;
            _healDelayer = Mathf.Clamp(_healDelayer, 0, healPeriod);
            
            var isAble = _skillIDList[5] && _healDelayer <= 0;
            if(!isAble)
                return;
            
            SetChosenSkills(5);
            SkillSignals.Instance.OnHealing?.Invoke(healDuration);
            _healDelayer += healPeriod;
        }
        private void ApplyReduceDamage()
        {
            var isAble = _skillIDList[4];
            if(!isAble)
                return;
            
            SetChosenSkills(4);
            SkillSignals.Instance.OnReduceDamage?.Invoke();
            _skillIDList[4] = false;
        }
        private void ApplyMaxHp()
        {
            var isAble = _skillIDList[3];
            if(!isAble)
                return;
            
            SetChosenSkills(3);
            SkillSignals.Instance.OnMaxHealth?.Invoke();
            _skillIDList[3] = false;
        }
        private void ApplyNitro()
        {
            _nitroDelayer -= Time.deltaTime;
            _nitroDelayer = Mathf.Clamp(_nitroDelayer, 0, nitroPeriod);
            
            var isAble = _skillIDList[2] && _nitroDelayer <= 0;
            if(!isAble)
                return;
            
            SetChosenSkills(2);
            SkillSignals.Instance.OnNitro?.Invoke(nitroDuration);
            _nitroDelayer += nitroPeriod;
        }
        
        #region GodMode

        private void ApplyGodMode()
        {
            _godModeDelayer -= Time.deltaTime;
            _godModeDelayer = Mathf.Clamp(_godModeDelayer, 0, godModePeriod);
            
            var isAble = _skillIDList[1] && _godModeDelayer <= 0;
            if(!isAble)
                return;
            
            SetChosenSkills(1);
            SkillSignals.Instance.OnGodMode?.Invoke(godModeDuration);
            _godModeDelayer += godModePeriod;


        }

        #endregion

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

        private void SetChosenSkills(int id)
        {
            switch (ChosenSkills[id])
            {
                case 0:
                    ChosenSkills[id] = 1;
                    break;
                case 1:
                    ChosenSkills[id] = 2;
                    break;
                case 2:
                    ChosenSkills[id] = 3;
                    break;
                default:
                    break;
            }
        }

        
    }
}
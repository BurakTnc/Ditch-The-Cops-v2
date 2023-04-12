using System;
using System.Collections.Generic;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class SkillManager : MonoBehaviour
    {

        public static SkillManager Instance;
        
       [HideInInspector] public readonly int[] ChosenSkills = new int[9];
       [HideInInspector] public int earningLevel = 1;
        
        public float missileSpawnPeriod;
        public float godModePeriod;
        public float nitroPeriod;
        public float healPeriod;
        public float spikePeriod;
        public float oilPeriod;

        [SerializeField] private List<SkillSpecs> instanceSpecs = new List<SkillSpecs>();
        [SerializeField] private float godModeDuration;
        [SerializeField] private float nitroDuration;
        [SerializeField] private float healDuration;
        
        // 0-Missile / 1-God Mode / 2-Nitro / 3-Max HP / 4-Reduce Damage / 5-Heal / 6-Bonus Earning / 7-Spike Trap / 8-Oil Trap
        private readonly bool[] _skillIDList = new bool[9];
        
        private Transform _player;
        private float _missileDelayer, _godModeDelayer, _nitroDelayer, _healDelayer, _spikeDelayer, _oilDelayer;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            {
                Destroy(this);
                return;
            }

            Instance = this;
            _player = GameObject.Find("Player").transform;
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
            ApplySpikeTrap();
            ApplyOilTrap();
        }

        private void OpenASkill(int skillID)
        {
            SetChosenSkills(skillID);
            _skillIDList[skillID] = true;
            switch (skillID)
            {
                case 0:
                    _missileDelayer = 0;
                    missileSpawnPeriod -= 5;
                    break;
                case 1:
                    _godModeDelayer = 0;
                    godModePeriod -= 5;
                    break;
                case 2:
                    _nitroDelayer = 0;
                    nitroPeriod -= 5;
                    break;
                case 5:
                    _healDelayer = 0;
                    healPeriod -= 5;
                    break;
                case 6:
                    ApplyBonusEarning();
                    break;
                case 7:
                    _spikeDelayer = 0;
                    spikePeriod -= 5;
                    break;
                case 8:
                    _oilDelayer = 0;
                    oilPeriod -= 5;
                    break;
                default:
                    break;
            }
        }

        private void ApplyBonusEarning()
        {
            SkillSignals.Instance.OnBonusIncome?.Invoke();
        }
        private void ApplyOilTrap()
        {
            _oilDelayer -= Time.deltaTime;
            _oilDelayer = Mathf.Clamp(_oilDelayer, 0, oilPeriod);

            var isAble = _skillIDList[8] && _oilDelayer <= 0;
            if(!isAble)
                return;

            _oilDelayer += oilPeriod;
            var oil = Instantiate(Resources.Load<GameObject>("Spawnables/Oil"));
            var targetScale = oil.transform.localScale;
            oil.transform.localScale=Vector3.zero;
            var rot = _player.GetChild(0).transform.rotation;
            oil.transform.SetPositionAndRotation(_player.position - _player.forward, rot);
            oil.transform.DOScale(targetScale, 2f).SetEase(Ease.OutBack);
            Destroy(oil,5);
        }
        private void ApplySpikeTrap()
        {
            _spikeDelayer -= Time.deltaTime;
            _spikeDelayer = Mathf.Clamp(_spikeDelayer, 0, spikePeriod);

            var isAble = _skillIDList[7] && _spikeDelayer <= 0;
            if(!isAble)
                return;

            _spikeDelayer += spikePeriod;
            var spike=Instantiate(Resources.Load<GameObject>("Spawnables/Spike"));
            var targetScale = spike.transform.localScale;
            spike.transform.localScale = new Vector3(0, targetScale.y, targetScale.z);
            var rot = _player.GetChild(0).transform.rotation;
            spike.transform.SetPositionAndRotation(_player.position - _player.forward * 3, rot);
            spike.transform.DOScaleX(targetScale.x, 2f).SetEase(Ease.OutBack);
            Destroy(spike,5);
        }
    
        private void ApplyHeal()
        {
            _healDelayer -= Time.deltaTime;
            _healDelayer = Mathf.Clamp(_healDelayer, 0, healPeriod);
            
            var isAble = _skillIDList[5] && _healDelayer <= 0;
            if(!isAble)
                return;
            
            SkillSignals.Instance.OnHealing?.Invoke(healDuration);
            _healDelayer += healPeriod;
        }
        private void ApplyReduceDamage()
        {
            var isAble = _skillIDList[4];
            if(!isAble)
                return;
            
            SkillSignals.Instance.OnReduceDamage?.Invoke();
            _skillIDList[4] = false;
        }
        private void ApplyMaxHp()
        {
            var isAble = _skillIDList[3];
            if(!isAble)
                return;
            
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
                    switch (id)
                    {
                        case 4:
                            SkillSignals.Instance.OnReduceDamageLevelIncrease?.Invoke();
                            break;
                        case 5:
                            SkillSignals.Instance.OnHealLevelIncrease?.Invoke();
                            break;
                        default:
                            break;

                    }
                    break;
                case 2:
                    ChosenSkills[id] = 3;
                    switch (id)
                    {
                        case 4:
                            SkillSignals.Instance.OnReduceDamageLevelIncrease?.Invoke();
                            break;
                        case 5:
                            SkillSignals.Instance.OnHealLevelIncrease?.Invoke();
                            break;
                        default:
                            break;

                    }
                    if (LevelManager.Instance.skillSpecsList.Contains(instanceSpecs[id]))
                    {
                        LevelManager.Instance.skillSpecsList.Remove(instanceSpecs[id]);
                    }

                    if (LevelManager.Instance.chosenSkills.Contains(instanceSpecs[id]))
                    {
                        LevelManager.Instance.chosenSkills.Remove(instanceSpecs[id]);
                    }

                    if (LevelManager.Instance.chosenSkills.Count == 0 &&
                        LevelManager.Instance.skillSpecsList.Count == 0)
                    {
                        LevelManager.Instance.onLose = true;
                    }
                    
                    break;
                default:
                    break;
            }
        }

        
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.ScriptableObjects;
using _YabuGames.Scripts.Signals;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {

        
        [SerializeField] private CarSpecs specs;
        [SerializeField] private List<GameObject> damageEffects = new List<GameObject>();
        [SerializeField] private PlayerPhysicsController physicsController;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private GameObject eliminateEffect;

        private float _maxHealth;
        private float _health;
        private List<GameObject> _activeEffects = new List<GameObject>();
        private int _takenDamageLevel;
        private bool _onHeal;
        private bool _onLose;
        private bool _isRevived;
        private int _healLevel = 1;

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
            SkillSignals.Instance.OnMaxHealth += GainMaxHp;
            SkillSignals.Instance.OnHealing += ActivateHealMode;
            SkillSignals.Instance.OnHealLevelIncrease += IncreaseHealLevel;
            LevelSignals.Instance.OnRevive += Revive;
            LevelSignals.Instance.OnBonusHealing += GetBonusHeal;
        }

        private void UnSubscribe()
        {
            SkillSignals.Instance.OnMaxHealth -= GainMaxHp;
            SkillSignals.Instance.OnHealing -= ActivateHealMode;
            SkillSignals.Instance.OnHealLevelIncrease -= IncreaseHealLevel;
            LevelSignals.Instance.OnRevive -= Revive;
            LevelSignals.Instance.OnBonusHealing -= GetBonusHeal;
        }

        private void Start()
        {
            specs = Resources.Load<CarSpecs>($"Car Data/Car-{PlayerVehicleManager.Instance.carID+1}");
            _health = specs.health;
            _maxHealth = _health;
        }

        private void Update()
        {
            Heal();
        }

        private void IncreaseHealLevel()
        {
            _healLevel++;
        }
        private void Revive()
        {
            eliminateEffect.SetActive(false);
            _isRevived = true;
            _onLose = false;
            _health = _maxHealth;
            foreach (var effect in damageEffects)
            {
                effect.SetActive(false);
            }
            InputSignals.Instance.CanMove?.Invoke(false);
            var amount = _health / _maxHealth;
            UIManager.Instance.UpdateHealthBar(amount);
            _takenDamageLevel = 0;
            foreach (var effect in _activeEffects)
            {
                effect.SetActive(false);
                damageEffects.Add(effect);
            }
            _activeEffects.Clear();
        }
        private void Heal()
        {
            if (!_onHeal) 
                return;

            _health += (2 + _healLevel) * Time.deltaTime;
            var amount = _health / _maxHealth;
            UIManager.Instance.UpdateHealthBar(amount);
            _health = Mathf.Clamp(_health, 0, _maxHealth);
            
            if (!(_health > .3f))
                return;
            
            foreach (var effect in damageEffects)
            {
                effect.SetActive(false);
            }
            _takenDamageLevel = 0;
            foreach (var effect in _activeEffects)
            {
                effect.SetActive(false);
                damageEffects.Add(effect);
            }
            _activeEffects.Clear();
        }

        private void ActivateHealMode(float duration)
        {
            StartCoroutine(HealRoutine(duration));
        }

        private IEnumerator HealRoutine(float duration)
        {
            _onHeal = true;
            yield return new WaitForSeconds(duration);
            _onHeal = false;
        }
        private void SetHealthBar()
        {
            var amount = _health / _maxHealth;
            UIManager.Instance.UpdateHealthBar(amount);
            SetDamageEffects(amount);
        }

        private void GainMaxHp()
        {
            _maxHealth += _maxHealth * .1f;
            var amount = _health / _maxHealth;
            UIManager.Instance.PlayMaxHpAnimation();
            UIManager.Instance.UpdateHealthBar(amount);
        }

        private void SetDamageEffects(float healthAmount)
        {
            switch (healthAmount)
            {
                case <= .10f:
                {
                    if(_takenDamageLevel>1)
                        return;
                    _takenDamageLevel++;
                    var r = Random.Range(0, damageEffects.Count);
                    var effect = damageEffects[r];
                    effect.SetActive(true);
                    damageEffects.Remove(effect);
                    _activeEffects.Add(effect);
                    return;
                }
                case <= .3f:
                {
                    if(_takenDamageLevel>0)
                        return;
                    LevelManager.Instance.ShowHealOffer();
                    _takenDamageLevel++;
                    var r = Random.Range(0, damageEffects.Count);
                    var effect = damageEffects[r];
                    effect.SetActive(true);
                    damageEffects.Remove(effect);
                    _activeEffects.Add(effect);
                    return;
                }

            }
        }
        public void TakeDamage(int damage)
        {
            if (_health<damage && !_onLose)
            {
                _health = 0;
                PoolManager.Instance.GetEliminatedParticle(transform.position+Vector3.up);
                SetHealthBar();
                physicsController.Eliminate();
                Invoke(nameof(Lose), 2);
                eliminateEffect.SetActive(true);
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
                HapticManager.Instance.PlaySoftHaptic();
                InputSignals.Instance.CanMove?.Invoke(true);
                _onLose = true;
                return;
            }
            _health -= damage;
            SetHealthBar();
        }

        private void Lose()
        {
            
            if (_isRevived)
            {
                Invoke(nameof(CallLose), 1);
                return;
            }
            LevelSignals.Instance.OnPlayerDestroyed?.Invoke();
        }

        private void CallLose()
        {
            CoreGameSignals.Instance.OnLevelFail?.Invoke();
        }

        private void GetBonusHeal()
        {
            _health += _maxHealth * .5f;
            foreach (var effect in damageEffects)
            {
                effect.SetActive(false);
            }
            _takenDamageLevel = 0;
            foreach (var effect in _activeEffects)
            {
                effect.SetActive(false);
                damageEffects.Add(effect);
            }
            _activeEffects.Clear();
            SkillSignals.Instance.OnHealing?.Invoke(1);
        }
        public void GetHeal()
        {
            _health += _maxHealth * .2f;
            foreach (var effect in damageEffects)
            {
                effect.SetActive(false);
            }
            _takenDamageLevel = 0;
            foreach (var effect in _activeEffects)
            {
                effect.SetActive(false);
                damageEffects.Add(effect);
            }
            _activeEffects.Clear();
            SkillSignals.Instance.OnHealing?.Invoke(1);
        }
    }
}
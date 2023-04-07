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
        [SerializeField] private int collectibleHealValue;

        private float _maxHealth;
        private float _health;
        private List<GameObject> _activeEffects = new List<GameObject>();
        private int _takenDamageLevel;
        private bool _onHeal;
        private bool _onLose;

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
        }

        private void UnSubscribe()
        {
            SkillSignals.Instance.OnMaxHealth -= GainMaxHp;
            SkillSignals.Instance.OnHealing -= ActivateHealMode;
        }

        private void Start()
        {
            _health = specs.health;
            _maxHealth = _health;
        }

        private void Update()
        {
            Heal();
        }

        private void Heal()
        {
            if (!_onHeal) 
                return;

            _health += 2 * Time.deltaTime;
            var amount = _health / _maxHealth;
            UIManager.Instance.UpdateHealthBar(amount);
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
                    if(_takenDamageLevel>3)
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
                    if(_takenDamageLevel>2)
                        return;
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
                SetHealthBar();
                Invoke(nameof(Lose),2);
                physicsController.Eliminate();
                LevelSignals.Instance.OnPlayerDestroyed?.Invoke();
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
            CoreGameSignals.Instance.OnLevelFail?.Invoke();
        }

        public void GetHeal()
        {
            _health += collectibleHealValue;
            SkillSignals.Instance.OnHealing?.Invoke(1);
        }
    }
}
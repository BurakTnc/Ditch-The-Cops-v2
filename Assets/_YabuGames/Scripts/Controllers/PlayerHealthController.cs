using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.ScriptableObjects;
using UnityEngine.UI;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {

        [SerializeField] private CarSpecs specs;
        [SerializeField] private List<GameObject> damageEffects = new List<GameObject>();

        private float _maxHealth;
        private float _health;
        private List<GameObject> _activeEffects = new List<GameObject>();
        private int _takenDamageLevel;

        private void Start()
        {
            _health = specs.health;
            _maxHealth = _health;
        }

        private void SetHealthBar()
        {
            var amount = _health / _maxHealth;
            UIManager.Instance.UpdateHealthBar(amount);
            SetDamageEffects(amount);
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
                case <= .25f:
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
                case <= .50f:
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
                case <= .75f:
                {
                    if(_takenDamageLevel>0)
                        return;
                    _takenDamageLevel++;
                    var r = Random.Range(0, damageEffects.Count);
                    var effect = damageEffects[r];
                    effect.SetActive(true);
                    damageEffects.Remove(effect);
                    _activeEffects.Add(effect);
                    break;
                }
            }
        }
        public void TakeDamage(int damage)
        {
            if (_health<damage)
            {
                _health = 0;
                SetHealthBar();
                //Lose
                return;
            }
            _health -= damage;
            SetHealthBar();
        }
        
    }
}
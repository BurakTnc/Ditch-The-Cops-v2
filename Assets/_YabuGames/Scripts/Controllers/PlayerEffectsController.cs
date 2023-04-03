using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.Serialization;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerEffectsController : MonoBehaviour
    {
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private GameObject healEffect;
        [SerializeField] private GameObject reduceDamageEffect;
        [SerializeField] private GameObject nitroEffect;

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
            SkillSignals.Instance.OnGodMode += ApplyGodMode;
            SkillSignals.Instance.OnHealing += ApplyHealingEffect;
            SkillSignals.Instance.OnNitro += ApplySpeedEffect;
            SkillSignals.Instance.OnReduceDamage += ApplyReduceDamage;
        }

        private void UnSubscribe()
        {
            SkillSignals.Instance.OnGodMode -= ApplyGodMode;
            SkillSignals.Instance.OnHealing -= ApplyHealingEffect;
            SkillSignals.Instance.OnNitro -= ApplySpeedEffect;
            SkillSignals.Instance.OnReduceDamage -= ApplyReduceDamage;
        }

        #endregion

        private void ApplyGodMode(float duration)
        {
            StartCoroutine(ApplyCoolDown(shieldEffect, duration));
        }

        private void ApplyReduceDamage()
        {
            reduceDamageEffect.SetActive(true);
        }

        private void ApplyHealingEffect(float duration)
        {
            StartCoroutine(ApplyCoolDown(healEffect, duration));
        }

        private void ApplySpeedEffect(float duration)
        {
            StartCoroutine(ApplyCoolDown(nitroEffect, duration));
        }

        private IEnumerator ApplyCoolDown(GameObject effect,float time)
        {
            effect.SetActive(true);
            yield return new WaitForSeconds(time);
            effect.SetActive(false);
        }
    }
    
}
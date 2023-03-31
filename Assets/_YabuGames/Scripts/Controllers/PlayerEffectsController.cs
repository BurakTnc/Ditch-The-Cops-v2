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

        private void ApplyGodMode()
        {
            StartCoroutine(ApplyCoolDown(shieldEffect, 5));
        }

        private void ApplyReduceDamage()
        {
            reduceDamageEffect.SetActive(true);
        }

        private void ApplyHealingEffect()
        {
            StartCoroutine(ApplyCoolDown(healEffect, 5));
        }

        private void ApplySpeedEffect()
        {
            StartCoroutine(ApplyCoolDown(nitroEffect, 5));
        }

        private IEnumerator ApplyCoolDown(GameObject effect,float time)
        {
            effect.SetActive(true);
            yield return new WaitForSeconds(time);
            effect.SetActive(false);
        }
    }
    
}
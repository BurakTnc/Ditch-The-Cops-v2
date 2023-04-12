using System;
using System.Collections;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using _YabuGames.Scripts.Spawners;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {
        [SerializeField] private PlayerHealthController healthController;
        [SerializeField] private PlayerPhysicsController physicsController;
        [SerializeField] private AudioClip hitSound;

        private bool _onGodMode;
        private bool _onReduceDamage;
        private bool _isEliminated;
        private float _reduceDamageLevel = .1f;
        private int _coinIncome = 10;

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
           SkillSignals.Instance.OnReduceDamage += ApplyReducedDamage;
           SkillSignals.Instance.OnReduceDamageLevelIncrease += ReduceDamageLevelIncrease;
           SkillSignals.Instance.OnBonusIncome += IncreaseCoinIncome;
           LevelSignals.Instance.OnPlayerDestroyed += Eliminate;
           LevelSignals.Instance.OnRevive += Revive;
       }

       private void UnSubscribe()
       {
           SkillSignals.Instance.OnGodMode -= ApplyGodMode;
           SkillSignals.Instance.OnReduceDamage -= ApplyReducedDamage;
           SkillSignals.Instance.OnReduceDamageLevelIncrease -= ReduceDamageLevelIncrease;
           SkillSignals.Instance.OnBonusIncome -= IncreaseCoinIncome;
           LevelSignals.Instance.OnPlayerDestroyed -= Eliminate;
           LevelSignals.Instance.OnRevive -= Revive;
       }

       #endregion

       private void ReduceDamageLevelIncrease()
       {
           _reduceDamageLevel += .1f;
       }

       private void IncreaseCoinIncome()
       {
           _coinIncome *= 2;
       }
       private void Eliminate()
       {
           _isEliminated = true;
       }

       private void Revive()
       {
           _isEliminated = false;
       }

       private void ApplyReducedDamage()
       {
           _onReduceDamage = true;
       }
       private void ApplyGodMode(float duration)
       {
           StartCoroutine(Delayer(duration));
       }

       private IEnumerator Delayer(float duration)
       {
           _onGodMode = true;
           yield return new WaitForSeconds(duration);
           _onGodMode = false;
       }
        private void OnCollisionEnter(Collision collision)
        {
            if(_isEliminated)
                return;
            if (collision.gameObject.TryGetComponent(out PoliceCarController component))
            {
                if (!_onGodMode)
                {
                    HapticManager.Instance.PlayWarningHaptic();
                    physicsController.PoliceCollision(collision.contacts[0].point);
                    if (_onReduceDamage)
                    {
                        var reducedDamage = (int)(component.GetDamage() - component.GetDamage() * _reduceDamageLevel);
                        healthController.TakeDamage(reducedDamage);
                    }
                    else
                    {
                        healthController.TakeDamage(component.GetDamage());
                    }
                    
                }
                else
                {
                    HapticManager.Instance.PlayHeavyHaptic();
                    if(component.isArmored)
                        return;
                    component.MissileExplosion(collision.contacts[0].point,10);
                }
                return;
            }

            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                HapticManager.Instance.PlaySelectionHaptic();
                physicsController.ObstacleCollision(collision.contacts[0].point, rb);
                AudioSource.PlayClipAtPoint(hitSound,transform.position);

                if (collision.transform.childCount <= 0) 
                    return;
                
                var obj = collision.transform.GetChild(0).gameObject;
                
                if (!obj.CompareTag("HydrantWater")) 
                    return;
                obj.transform.SetParent(null);
                obj.SetActive(true);
                Destroy(obj,5);

            }
            else
            {
                physicsController.PoliceCollision(collision.contacts[0].point);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                if (other.transform.parent.TryGetComponent(out Spawner spawner))
                {
                    spawner.hasCollectible = false;
                }

                SpawnManager.Instance.currentCollectibleCount--;
                SpawnManager.Instance.collectibleDelayer += 2;
                var r = Random.Range(SkillManager.Instance.earningLevel, 6);
                CoreGameSignals.Instance.OnSpawnCoins?.Invoke(r, 0, _coinIncome,false);
                Destroy(other.gameObject);
            }
            if (other.gameObject.CompareTag("Health"))
            {
                if (other.transform.parent.TryGetComponent(out Spawner spawner))
                {
                    spawner.hasCollectible = false;
                }
                SpawnManager.Instance.currentCollectibleCount--;
                SpawnManager.Instance.collectibleDelayer += 2;
                healthController.GetHeal();
                Destroy(other.gameObject);
            }
        }
    }
}
